#!/usr/bin/python
# encoding: utf-8

import socket
import sys
import time
import struct
import os
import configparser
import subprocess
import getopt
import threading
from ctypes import *
from _thread import *
HOST = ''   # Symbolic name meaning all available interfaces
PORT = 9344 # Arbitrary non-privileged port

g_Version = 'v1.1.0(2022/01/10)'

ELAN_DEFAULT_VID = 0x04F3

ELAN_HEADER = 0x78
ELAN_TYPE_TPREPORT = 0x01
ELAN_TYPE_SENDDATA = 0x10
ELAN_TYPE_RECVRESP = 0x11
ELAN_TYPE_FILEDATA = 0x12
ELAN_TYPE_DOIAP = 0x13
ELAN_TYPE_STATUS = 0x14

ELAN_DATAFMT_IAPARAMS = 0x00
ELAN_DATAFMT_BOOTCODE_EKT = 0x10
ELAN_DATAFMT_MASTER_EKT = 0x11
ELAN_DATAFMT_SLAVE_EKT = 0x12

ELAN_MAX_RECV_RAWDATA_BYTES = 1024
ELAN_MAX_RECV_TPDATA_BYTES = 0x3F
#The max recv data length for HIDI2C
ELAN_MAX_RECV_TPDATA_BYTES_HIDI2C = 0x43

ELAN_SOCKET_STATUS_SUCCESS = 0x01
ELAN_SOCKET_STATUS_FAILED = 0x10

ELAN_FW_UPDATE_SUCCESSFUL = 0x01
ELAN_FW_UPDATE_RESULT_OFFSET = 1024

ELAN_HEADER_LENGTH = 0x04

g_bSocketServerRun = False
g_bSocketSendThreadRun = False
ServerSocket = None
g_ElanTouchdll = None

strExeFullDir = os.getcwd()

# declare the receive data count and buffer
g_RecvPacketCount = 0
g_SendDataBuffer = bytearray()

# Declare the flag to show debug informaiton
g_Debug = False

# declare the global socket object to store the socket current connected
g_ConnectedSocket = None

# declare the flag to show how to run socket
g_bAndroidViaUsb = False

# declare to store the specific interface
g_iInterface = 0
g_iMode = -1

# declare flag to receive the tp data
g_bReadTPData = False

# declare flag to switch the send data mode
g_bSendAll = False

# Linux Library?
g_bLinux = False

# Declare global lock
g_lock = threading.RLock()
g_TPLock = threading.RLock()

# Declare to store the start clock when send command
g_Start = 0.0


#Start a socket server
def MainProcess():
    #set global variable
    global g_bSocketServerRun
    global g_ElanTouchdll
    global g_Debug
    global g_ConnectedSocket
    global g_iInterface
    global g_iMode
    global g_bAndroidViaUsb
    global g_Version

    # Connect the socket to the port where the server is listening
    szIPAddr = 'localhost'
    iPort = 9344
    
    #process the argument
    opts, args = getopt.getopt(sys.argv[1:], 'i:p:s:t:ad')
    '''
    Process the argument
    -i : IPAddress
    -p : Port Number
    -d : Debug Mode
    -a : android via usb cable
    -t : Run Remote AP with specific interface
         1:HID
         2:HID Android
         3:I2C Android
         4:I2CHID Android
         5:I2C Chrome
         6:I2CHID Android(Elan)
         9:SPI Android
    -s : SYSFS
         bus number
    '''

    # Output the version and build date
    print("Version:", g_Version)
    sys.stdout.flush()

    for o, a in opts:
        if o == '-i':
            szIPAddr = a
        elif o == '-p':
            iPort = int(a)
        elif o == '-d':
            print("Enable Debug Mode")
            sys.stdout.flush()
            g_Debug = True
        elif o == '-a':
            g_bAndroidViaUsb = True
        elif o == '-t':
            g_iInterface = int(a)
            print('g_iInterface=', g_iInterface)
            sys.stdout.flush()
        elif o == '-s':
            g_iMode = int(a)
            print('Enable SYSFS and bus number is {0}'.format(g_iMode))
            sys.stdout.flush()

    # Create a TCP/IP socket
    objSocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    if g_bAndroidViaUsb:
        RunSocketServer(objSocket, iPort)
    else:
        RunSocketClient(objSocket, szIPAddr, iPort)

#Run socket server
def RunSocketServer(ServerSocket, iPort):
    #set global variable
    global g_bSocketServerRun
    global g_ElanTouchdll
    global g_ConnectedSocket
    global g_bReadTPData
    global g_bSocketSendThreadRun

    try:
        ServerSocket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        ServerSocket.bind((HOST, iPort))
    except socket.error as msg:
        print('Bind failed. Error Code : ' + str(msg))
        sys.stdout.flush()
        return

    ServerSocket.listen(1)

    #now keep talking with the client
    while g_bSocketServerRun == True:
        print('Waiting for remote connect...')
        sys.stdout.flush()

        # wait to accept a connection - blocking call
        (ClientSocket, addr) = ServerSocket.accept()

        g_ConnectedSocket = ClientSocket
        g_bSocketSendThreadRun = True

        #display client information
        print('Connected with ' + addr[0] + ':' + str(addr[1]))
        sys.stdout.flush()

        #Contact TP
        if Contact2TP() == False :
            break

        start_new_thread(HandleTPDataRecv, (ClientSocket,))
        start_new_thread(SendDataThread, (ClientSocket,))
        HandleSocketReceive(ClientSocket)
        g_bSocketSendThreadRun = False
        print("Disconnect TP")
        sys.stdout.flush()

        g_ElanTouchdll.Disconnect()

#Run the socket client
def RunSocketClient(ClientSocket, szIPAddr, iPort):
    #set global variable
    global g_bSocketServerRun
    global g_ElanTouchdll
    global g_ConnectedSocket
    global g_bSocketSendThreadRun

    print('[Info] Connect to Server. IPAddress:', szIPAddr, ',Port:', iPort)
    sys.stdout.flush()
    server_address = (szIPAddr, iPort)

    nTryConnect2ServerCount = 10

    while(nTryConnect2ServerCount > 0):
        print('[Info] Try to connecting...')
        sys.stdout.flush()

        try:
            ClientSocket.connect(server_address)
        except Exception as e:
            print('[Info] Connect to server fail. Please make sure the server is run')
            sys.stdout.flush()
            nTryConnect2ServerCount -= 1
            if nTryConnect2ServerCount <= 0:
                print('[Info] Connect to server fail..., Please enter q to exit.')
                sys.stdout.flush()
                return
            time.sleep(5)
            continue

        print('[Info] Connect to server success.')
        sys.stdout.flush()
        break

    #Store the socket current connected
    g_ConnectedSocket = ClientSocket
    g_bSocketSendThreadRun = True
    #Contact TP
    if Contact2TP() == False :
        return

    start_new_thread(HandleTPDataRecv ,(ClientSocket,))
    start_new_thread(SendDataThread ,(ClientSocket,))
    HandleSocketReceive(ClientSocket)
    g_ConnectedSocket = None
    g_bSocketSendThreadRun = False

    print ("Disconnect TP")
    g_ElanTouchdll.Disconnect()


#connect to TP Device
def Contact2TP():
    # Set the global variable
    global g_ElanTouchdll
    # Interface type (ex:I2C, HIDI2C, ...)
    global g_iInterface
    # The sysfs bus number
    global g_iMode
    global g_Debug
    global g_bLinux

    if g_iInterface != 0 : # Non-windows interface
        if g_iMode == -1 : # IOCTL
            nResult = g_ElanTouchdll.Connect(0, 0, g_iInterface)
        else :#Sysfs
            nResult = g_ElanTouchdll.Connect(1, g_iMode, g_iInterface)
    else: # Windows HID
        nVID = c_uint(0)
        nPID = c_uint(0)
        nResult = g_ElanTouchdll.Connect(ELAN_DEFAULT_VID, 0)
        #Get the VID and PID
        nResult = g_ElanTouchdll.GetID(byref(nVID), byref(nPID), 0)
        if nResult != 0:
            return False

        #HID Over I2C need to set the bridge
        if nPID.value == 0x0b:
            print("I2C Bridge")
        elif nPID.value == 0x07:
            print("HIDOverI2C Bridge")
            nResult = g_ElanTouchdll.ConnectBridge(nVID, nPID, 1)

    if nResult != 0:
        print("Connect TP Fail")
        sys.stdout.flush()
        return False

    # Disable the debug message
    if g_bLinux == True:
        if g_Debug == False:
            g_ElanTouchdll.EnableDebug(False)
            g_ElanTouchdll.EnableErrorMsg(False)

    print("Connect TP Success")
    sys.stdout.flush()
    return True

# The function to process the data receiving form socket client. Check the data check sum and send the data to TP.
# input parameter
#   ClientSocket object
def HandleSocketReceive(ClientSocket):
    print("Start the socket receive function.")
    sys.stdout.flush()

    # set global variable
    global g_bSocketServerRun
    global g_Debug
    global g_bReadTPData

    # Infinite loop so that function do not terminate and thread do not end.
    while g_bSocketServerRun == True:
        # Receiving data from client
        try:
            data = ClientSocket.recv(ELAN_MAX_RECV_RAWDATA_BYTES)
        except:
            print('Server stop!')
            sys.stdout.flush()
            break

        # if data is None then the socket is disconnect
        if not data:
            print("Socket disconnect")
            # print("Receive data is not")
            sys.stdout.flush()
            # continue
            break

        # Convert the data to byte array
        byteData = bytearray(data)

        if g_Debug == True:
            sOutput = "[Debug] Receive Data:"
            for i in byteData:
                sOutput += hex(i)
                sOutput += ","
            print(sOutput)

        # 0x78 is a TP command packet
        if byteData[0] != ELAN_HEADER:
            continue

        if byteData[1] == ELAN_TYPE_TPREPORT:
            ProcTPCommand(byteData)
        # Start to receive file
        elif byteData[1] == ELAN_TYPE_SENDDATA:
            # set socket timeout to 10 sec.
            ClientSocket.settimeout(10)
            ProcDataTransfer(ClientSocket, byteData)
            ClientSocket.settimeout(None)
        elif byteData[1] == ELAN_TYPE_DOIAP:
            result = DoIAP()
            returned = 0
            if result != ELAN_FW_UPDATE_SUCCESSFUL:
               returned = ELAN_SOCKET_STATUS_FAILED
               print("Failed to FW Update(Err=%d)" %result)
            else:
               returned = ELAN_SOCKET_STATUS_SUCCESS
               print("FW Update Successfully!!!")
            result += ELAN_FW_UPDATE_RESULT_OFFSET
            result_Low = result &0x00ff
            result_Hi = ((result & 0xff00) >> 8)
            #Return 0x78,0x14,0x00,0x04,0x00,result_hi,result_low,0x01 to motify client
            print("Response Status Packet")
            StatusPacket = bytearray([ELAN_HEADER, ELAN_TYPE_STATUS,0x00,0x04,0x00,result_Hi,result_Low,returned])
            nSentDataLen = ClientSocket.send(StatusPacket)
        else:
          print('do nothing')

    print('Exit HandleSocketReceive')
    sys.stdout.flush()

    #When the socket is disconnected set the Read TP Data
    g_bReadTPData = False
    time.sleep(0.02)
    #ClientSocket.close()
    #ClientSocket = None

def ProcDataTransfer(ClientSocket, recvData):
    #get the file length
    nDataLen = (recvData[5] << 24 | recvData[6] << 16 | recvData[7] << 8 | recvData[8])
    print('Data Total length=',nDataLen)
    strINIFile = strExeFullDir + "\\GUI One Button\\IAP\\IAP.ini"
    #print 'strINIFile=', strINIFile

    # Receive the IAP Parameter
    if recvData[4] == ELAN_DATAFMT_IAPARAMS :
        #Return 0x78,0x11,0x00,0x04,0x00,0x00,0x00,0x01 to make sure the IAP procedure start
        print("Response IAP packet")
        ResponsePacket = bytearray([ELAN_HEADER, ELAN_TYPE_RECVRESP,0x00,0x04,0x00,0x00,ELAN_DATAFMT_IAPARAMS,0x01])
        ClientSocket.send(ResponsePacket)

        #Receiving IAP Parameter data from client
        data = ClientSocket.recv(ELAN_MAX_RECV_RAWDATA_BYTES)

        #it data is None then the socket is disconnect
        if not data:
            print("Receive IAP Parameter Data Fail.")
            return False

        #unpack the structure data
        print('Data Length = ',len(data))
        (header, bElanBridge, iVDD, iVIO, iI2C_Addr, iPID, iVID, iInterface, bNewIAP, iMasterAddr,
        iSlave1Addr, iSlave2Addr, bIsBootCodeUpdater, bReK) = struct.unpack("@14i", data)

        """
        print 'header=', hex(header)
        print 'bElanBridge=', bElanBridge
        print 'iVDD=',iVDD
        print 'iVIO=',iVIO
        print 'iI2C_Addr=',hex(iI2C_Addr)
        print 'iPID=',iPID
        print 'iVID=',iVID
        print 'iInterface=',iInterface
        print 'bNewIAP=',bNewIAP
        print 'iMasterAddr=',hex(iMasterAddr)
        print 'iSlave1Addr=',hex(iSlave1Addr)
        print 'iSlave2Addr=',hex(iSlave2Addr)
        print 'bIsBootCodeUpdater=',bIsBootCodeUpdater
        print 'bReK=',bReK
        print 'CurrentWorkDir=', strExeFullDir
        """

        if bElanBridge == 0:
           iElanBridge = 1
        else:
           iElanBridge = 0
        WriteINIFile("IAP", "WIN8_BRIDGE", iElanBridge, strINIFile)
        WriteINIFile("IAP", "VDD",iVDD, strINIFile)
        WriteINIFile("IAP", "VIO",iVIO, strINIFile)
        WriteINIFile("IAP", "I2C_Addr",hex(iI2C_Addr), strINIFile)
        WriteINIFile("IAP", "PID",hex(iPID), strINIFile)
        WriteINIFile("IAP", "VID",hex(iVID), strINIFile)
        WriteINIFile("IAP", "INTERFACE",iInterface, strINIFile)
        WriteINIFile("IAP", "NEW_IAP",bNewIAP, strINIFile)
        WriteINIFile("IAP", "MASTER_ADDR",hex(iMasterAddr), strINIFile)
        WriteINIFile("IAP", "SLAVE1_ADDR",hex(iSlave1Addr), strINIFile)
        WriteINIFile("IAP", "SLAVE2_ADDR",hex(iSlave2Addr), strINIFile)
        WriteINIFile("IAP", "Update_Boot_Code",bIsBootCodeUpdater, strINIFile)
        WriteINIFile("IAP", "REK_DELAY_TIME",bReK, strINIFile)
        WriteINIFile("IAP", "FW_FILE_PATH1", "", strINIFile)
        strEktFile = strExeFullDir + "\\GUI One Button\\IAP\\Master.ekt"
        RemoveFile(strEktFile)
        WriteINIFile("IAP", "FW_FILE_PATH2", "", strINIFile)
        strEktFile = strExeFullDir + "\\GUI One Button\\IAP\\Slave.ekt"
        RemoveFile(strEktFile)
        WriteINIFile("IAP", "BC_FILE_PATH", "", strINIFile)
        strEktFile = strExeFullDir + "\\GUI One Button\\IAP\\BootCode.ekt"
        RemoveFile(strEktFile)


    elif recvData[4] == ELAN_DATAFMT_MASTER_EKT:
        #Return 0x78,0x11,0x00,0x04,0x00,0x00,0x11,0x01 to make master data to be sent
        print ("Response Master packet")
        ResponsePacket = bytearray([ELAN_HEADER, ELAN_TYPE_RECVRESP,0x00,0x04,0x00,0x00,ELAN_DATAFMT_MASTER_EKT,0x01])
        ClientSocket.send(ResponsePacket)

        #Receiving Master ekt file from client(already remove header)
        packet = SocketRecvAll(ClientSocket, nDataLen);

        #it data is None then the socket is disconnect
        if not packet:
            print ("Receive Master ekt file Data Fail.")
            return False;

        #Save Master data into ekt file.Already Remove Header from full data
        print ("Save Master data into ekt file. Data Lenght=",len(packet))
        strMasterFileName = "IAP\\Master.ekt"
        strMEktFile = strExeFullDir + "\\GUI One Button\\"+strMasterFileName
        SaveDataToEktFile(strMEktFile, packet)
        WriteINIFile("IAP", "FW_FILE_PATH1", strMasterFileName, strINIFile)

    elif recvData[4] == ELAN_DATAFMT_SLAVE_EKT:
        #Return 0x78,0x11,0x00,0x04,0x00,0x00,0x12,0x01 to make slave data to be sent
        print ("Response Slave packet")
        ResponsePacket = bytearray([ELAN_HEADER, ELAN_TYPE_RECVRESP,0x00,0x04,0x00,0x00,ELAN_DATAFMT_SLAVE_EKT,0x01])
        ClientSocket.send(ResponsePacket)

        #Receiving Slave ekt file from client(already remove header)
        packet = SocketRecvAll(ClientSocket, nDataLen)

        #it data is None then the socket is disconnect
        if not packet:
            print ("Receive Slave ekt file Data Fail.")
            return False;

        #Save Slave data into ekt file.
        print ("Save Slave data into ekt file. Data Lenght=",len(packet))
        strSlaveFileName = "IAP\\Slave.ekt"
        strSEktFile = strExeFullDir + "\\GUI One Button\\"+strSlaveFileName
        SaveDataToEktFile(strSEktFile, packet)
        WriteINIFile("IAP", "FW_FILE_PATH2", strSlaveFileName, strINIFile)

    elif recvData[4] == ELAN_DATAFMT_BOOTCODE_EKT:
        #Return 0x78,0x11,0x00,0x00,0x00,0x10,0x01 to make bootcode data to be sent
        print ("Response BootCode packet")
        ResponsePacket = bytearray([ELAN_HEADER, ELAN_TYPE_RECVRESP,0x00,0x04,0x00,0x00,ELAN_DATAFMT_BOOTCODE_EKT,0x01])
        ClientSocket.send(ResponsePacket)

        #Receiving BootCode ekt file from client(already remove header)
        packet = SocketRecvAll(ClientSocket, nDataLen)

        #it data is None then the socket is disconnect
        if not packet:
            print ("Receive BootCode ekt file Data Fail.")
            return False;

        #Save BootCode data into ekt file.
        print ("Save BootCode data into ekt file. Data Lenght=",len(packet))
        strBootCodeFileName = "IAP\\BootCode.ekt"
        strBCEktFile = strExeFullDir + "\\GUI One Button\\"+strBootCodeFileName
        SaveDataToEktFile(strBCEktFile, packet)
        WriteINIFile("IAP", "BC_FILE_PATH", strBootCodeFileName, strINIFile)

#declare a function to process the TP command

def ProcTPCommand(byteData):
    #define the global variable
    global g_ElanTouchdll
    global g_SendDataBuffer
    global g_RecvPacketCount
    global g_Debug
    global g_iInterface
    global g_bSendAll
    global g_lock
    global g_TPLock
    global g_Start

    #Get the data length and check the check sum
    while(len(byteData) > 0):
        nDataLen = (byteData[2] << 8 | byteData[3])

        #get the command
        SendData = byteData[ELAN_HEADER_LENGTH:nDataLen+ELAN_HEADER_LENGTH]
        #convert the byte array to c type unsigned byte array
        raw_bytes = (c_ubyte * nDataLen).from_buffer_copy(SendData)

        #print the command data for debug
        if g_Debug == True :
            sOutput = "[Deubg] Command:"
            for i in SendData:
                sOutput += hex(i)
                sOutput += ","
            print(sOutput)

        #Skip the command 0x03, 0x0d, 0x00, 0x00
        if SendData[0] == 0x03 and SendData[1] == 0x0d and g_iInterface == 3:
            print('Skip the command', sOutput)
            return

        #send the command to tp and check the result
        with g_TPLock:
            nRet = g_ElanTouchdll.SendDevCommand(raw_bytes, nDataLen, 1, 0)
            g_Start = time.time()

        if nRet != 0:
            print ("Send command error", nRet)

        #Clear the buffer
        g_SendDataBuffer = bytearray()
        #Get the packet count
        if g_RecvPacketCount <= 0:
            g_RecvPacketCount = 10

        #0x59 command
        #if SendData[0] == 0x59:
        #    g_RecvPacketCount = int(SendData[5] * 2) / 60;
        #    if int(SendData[5] * 2) % 60 > 0:
        #        g_RecvPacketCount += 1;
        #elif SendData[0] == 0x58:
        #    g_RecvPacketCount = int(SendData[4] << 8 | SendData[5]) / 60;
        if g_bSendAll == True:
            if SendData[0] == 0x58:
                g_RecvPacketCount = int((SendData[4] << 8 | SendData[5]) / 60)
                if int(SendData[4] << 8 | SendData[5]) % 60 > 0:
                    g_RecvPacketCount += 1
        byteData = byteData[nDataLen+ELAN_HEADER_LENGTH:len(byteData)]

# The function to process the data receiving form TP and send the data via socket to remote AP
# input parameter
#   ClientSocket object
def HandleTPDataRecv(ClientSocket):
    print("Start read TP data and send to socket thread.")
    sys.stdout.flush()

    # set global variable
    global g_bSocketServerRun
    global g_ElanTouchdll
    global g_SendDataBuffer
    global g_RecvPacketCount
    global g_Debug
    global g_bReadTPData
    global g_lock
    global g_Start
    global g_iInterface

    # Set the flag to true start a loop to polling and read data from TP
    g_bReadTPData = True

    # Assign the data length of receiving form TP
    if g_iInterface == 6 or g_iInterface == 4 or g_iInterface == 8:
        nReadDataLen = ELAN_MAX_RECV_TPDATA_BYTES_HIDI2C
    else:
        nReadDataLen = ELAN_MAX_RECV_TPDATA_BYTES

    # Use to count the time up to 1000ms
    StartTime = time.time()
    # Count the receive report packet count to compute the report rate of each 1 sec.
    RecvReportCount = 0

    while g_bReadTPData == True:
        # allocate c_type unsigned byte array.
        pRecvData = (c_ubyte * nReadDataLen).from_buffer_copy(bytearray(nReadDataLen))

        # Receiving data form TP
        with g_TPLock:
           nRet = g_ElanTouchdll.ReadDevData(byref(pRecvData), nReadDataLen, 5, 0)

        '''Read the report data fail, set the receive packet count to 0.
           When the g_SendDataBuffer is not empty, send the data to host side via socket.'''
        if nRet != 0:
            # Output Recv Data for debug
            '''if len(g_SendDataBuffer) != 0:
                if g_Debug == True:
                    sOutput = "[Debug]"
                    for i in g_SendDataBuffer:
                        sOutput += hex(i)
                        sOutput += ","
                    print "Send ",sOutput

                if ClientSocket.sendall(g_SendDataBuffer) != None :
                    print '[ERROR] Unable to send data'
                    return

                #Clear the data and packet count
                g_RecvPacketCount = 0
                g_SendDataBuffer = bytearray()'''
            g_RecvPacketCount = 0

            continue

        # Debug information
        if g_Debug:
            sOutput = "[Recv TP Data]"
            for i in pRecvData:
                sOutput += hex(i)
                sOutput += ","
            print(sOutput)

        # When the interface is 6 (HID Over I2C Elan Driver), need to remove the Header data
        if g_iInterface == 6 or g_iInterface == 4 or g_iInterface == 8:
            if pRecvData[0] == 0xff:
                time.sleep(0.001)
                continue
            elif pRecvData[0] == 0x43 and pRecvData[2] == 0x02:
                pRecvData = pRecvData[4:]
            elif pRecvData[0] == 0x43 and pRecvData[2] == 0x07:
                pRecvData = pRecvData[2:]
            elif pRecvData[0] == 0x40:
                '''sOutput = "[0x40]"
                for i in pRecvData:
                    sOutput += hex(i)
                    sOutput += ","
                print(sOutput)'''
                pRecvData = pRecvData[2:]
            elif pRecvData[0] == 0x3f:
                pRecvData = pRecvData[2:0x3f]

        if pRecvData[0] == 0x78 and pRecvData[1] == 0x78 and pRecvData[2] == 0x78 and pRecvData[3] == 0x78:
            print("I am live report...")
            continue

        RecvReportCount += 1

        # Duration = time.time() - g_Start
        # g_Start = time.time()
        # print ('[Python Debug] RecvData:',Duration)
        # convert the c_ubyte array to byte array
        # nRecvDataLen = ELAN_MAX_RECV_TPDATA_BYTES
        RecvData = bytearray(pRecvData)
        nRecvDataLen = len(RecvData)
        data = bytearray(nRecvDataLen + ELAN_HEADER_LENGTH)
        nIdx = ELAN_HEADER_LENGTH

        for i in RecvData:
            data[nIdx] = i
            nIdx += 1
        # Header
        data[0] = ELAN_HEADER
        data[1] = ELAN_TYPE_TPREPORT
        # Packet length
        data[2] = ((nRecvDataLen & 0xff00) >> 8)
        data[3] = (nRecvDataLen & 0xff)

        # Due to the send data code in other thread, so need lock this operation
        # to avoid the g_SendDataBuffer Read/Write at the same time.
        with g_lock:
            g_SendDataBuffer = g_SendDataBuffer + data
            g_RecvPacketCount -= 1

        Duration = time.time() - StartTime
        if Duration >= 1:
            print('Report Rate:', RecvReportCount / Duration)
            RecvReportCount = 0
            StartTime = time.time()

        '''if g_RecvPacketCount > 0 and len(g_SendDataBuffer) < 20000:
            continue

        #Output Recv Data for debug
        if g_Debug == True:
            sOutput = "[Deubg]"
            for i in g_SendDataBuffer:
                sOutput += hex(i)
                sOutput += ","
            print "Send ",sOutput

        if g_Debug == True:
            Duration = time.time() - g_Start
            print '[Python Debug] Recv Data Cost clock:',Duration

        if ClientSocket.sendall(g_SendDataBuffer) != None :
            print '[ERROR] Unable to send data'
            return

        if g_Debug == True:
            Duration = time.time() - g_Start
            print '[Python Debug] Send Data via socket Cost clock:',Duration

        g_SendDataBuffer = bytearray()'''

# The function to process the data receiving form TP and send the data via socket to remote AP
# input parameter
#   ClientSocket object
def SendDataThread(ClientSocket):
    print("Start read TP data and send to socket thread.")
    # set global variable
    global g_bSocketServerRun
    global g_SendDataBuffer
    global g_RecvPacketCount
    global g_Debug
    global g_lock
    global g_bSocketSendThreadRun

    # Assign the data length of receiving form TP
    while g_bSocketSendThreadRun == True:
        #Output Recv Data for debug
        if len(g_SendDataBuffer) <= 0:
            time.sleep(0.0001)
            continue

        # Send all mode enable to check the buffer.
        if g_RecvPacketCount > 0 and len(g_SendDataBuffer) < 20000:
            time.sleep(0.0001)
            continue

        with g_lock:
            if g_Debug == True:
                sOutput = "[Debug]"
                for i in g_SendDataBuffer:
                    sOutput += hex(i)
                    sOutput += ","
                print("Send ", sOutput)

            if ClientSocket.sendall(g_SendDataBuffer) != None:
                print('[ERROR] Unable to send data')
                return
            g_RecvPacketCount = 5
            g_SendDataBuffer = bytearray()

    print('Exit send data thread')

#The function to write ini file for IAP parameter
#input parameter: File_Path
def WriteINIFile(Section, Key, value, strFilePath):
    print ("Write %s/%s into ini file." % (Key, value))

    config = ConfigParser.ConfigParser()
    config.optionxform = str
    config.read(strFilePath)
    #update existing value
    config.set(Section, Key, value)
    # save to a file
    config.write(open(strFilePath, 'w'))


# Helper function to recv n bytes or return None if EOF is hit
# Remove Header, only get raw data
def SocketRecvAll(sock, n):
    data = ''
    while len(data) < n:
        packet = sock.recv(ELAN_MAX_RECV_RAWDATA_BYTES*2)
        if not packet:
            return None
        header = packet[:ELAN_HEADER_LENGTH]
        rawdata = packet[ELAN_HEADER_LENGTH:]
        data += rawdata
        print ('recv=',len(data))
        #Return 0x78,0x14,0x00,0x04,0x00,0x00,datalen,0x01 to motify client
        raw_len = len(rawdata)
        LowLen = raw_len & 0x00ff
        HiLen = ((raw_len & 0xff00) >> 8)
        StatusPacket = bytearray([ELAN_HEADER, ELAN_TYPE_STATUS,0x00,0x04,0x00,HiLen,LowLen,ELAN_SOCKET_STATUS_SUCCESS])
        sock.send(StatusPacket)
    return data


#Remove file if it exists
def RemoveFile(strFileName):
    if os.path.exists(strFileName):
       try:
          os.remove(strFileName)
       except OSError as e:
          print ("Error: %s - %s" % (e.filename,e.strerror))
          return

#Save received data to ekt file
def SaveDataToEktFile(strFileName, data):
    print ("Save received data to ekt file.")

    if os.path.exists(strFileName):
       try:
          os.remove(strFileName)
       except OSError as e:
          print ("Error: %s - %s" % (e.filename,e.strerror))
          return

    with open(strFileName, 'wb') as f:
       f.write(data)
       f.close()

#Call External Execute File to DoIAP
def DoIAP():
    global g_ElanTouchdll
    print ("Starting to do DoIAP")
    IIAP_ERR_CONNECT_TO_SPECIFIC_PID_HID_DEVICE_FAILED = -3

    print ("Disconnect TP")
    g_ElanTouchdll.Disconnect()
    result = -1
    strExeFile = strExeFullDir + "\\GUI One Button\\GUI_IAP.exe"
    result = subprocess.call([strExeFile])

    time.sleep(0.01)#wait for 10ms
    #Re-Contact TP
    if Contact2TP() == False :
         return IIAP_ERR_CONNECT_TO_SPECIFIC_PID_HID_DEVICE_FAILED

    return result

class _Getch:
    """Gets a single character from standard input.  Does not echo to the
screen."""
    def __init__(self):
        try:
            self.impl = _GetchWindows()
        except ImportError:
            self.impl = _GetchUnix()

    def __call__(self): return self.impl()


class _GetchUnix:
    def __init__(self):
        import tty, sys

    def __call__(self):
        import sys, tty, termios
        old_settings = None
        ch = None
        try:
            fd = sys.stdin.fileno()
            old_settings = termios.tcgetattr(fd)
            tty.setraw(sys.stdin.fileno())
            ch = sys.stdin.read(1)
        except:
            ch = None
        finally:
            if old_settings != None:
                termios.tcsetattr(fd, termios.TCSADRAIN, old_settings)
        return ch


class _GetchWindows:
    def __init__(self):
        import msvcrt
    def __call__(self):
        import msvcrt
        return msvcrt.getch()

def LoadTPLib(nInterface):
    global g_bLinux

    try:
      ElanTouchdll = windll.LoadLibrary('LibTouch.dll')
      g_bLinux = False
    except:
      ElanTouchdll = None

    if ElanTouchdll == None:
        try:
            g_bLinux = True
            if nInterface == 3 or nInterface == 4 or nInterface == 6:
                os.chdir('/data/local/tmp')
            ElanTouchdll = cdll.LoadLibrary('./libTouch.so')
        except Exception as e:
            print (str(e))
            ElanTouchdll = None

    return ElanTouchdll


########################################################
# Main function
########################################################
def main():
    # Load the elan touch library
    global g_bSocketServerRun
    global g_ConnectedSocket
    global g_ElanTouchdll
    global g_bSendAll
    global g_iInterface

    # Process the argument to get interface
    opts, args = getopt.getopt(sys.argv[1:], 'i:p:s:t:ad')
    for o, a in opts:
        if o == '-t':
            g_iInterface = int(a)
            break

    # Load the library
    g_ElanTouchdll = LoadTPLib(g_iInterface)
    if g_ElanTouchdll == None :
        print('Load library fail...')
        sys.stdout.flush()
        sys.exit()

    g_bSocketServerRun = True
    g_ConnectedSocket = None
    g_TPConnected = False

    # Start a thread to run socket server
    start_new_thread(MainProcess, ())

    # Wait the user key 'q' to exit
    objGetch = _Getch()
    while True:
        pressedKey = objGetch()
        if pressedKey != None:
            if pressedKey.rstrip() == b'q' or pressedKey.rstrip() == 'q':
                g_bSocketServerRun = False
                break
            elif pressedKey == b's':
                if g_bSendAll == True:
                    g_bSendAll = False
                    print('[PYInfo]Send each packet mode.')
                    sys.stdout.flush()
                else:
                    g_bSendAll = True
                    print('[PYInfo]Send all packet mode.')
                    sys.stdout.flush()
        else:
            time.sleep(1000)

    # Close the socket connect
    if g_ConnectedSocket != None:
        print('Shutdown Socket')
        sys.stdout.flush()
        g_ConnectedSocket.shutdown(socket.SHUT_RDWR)
        g_ConnectedSocket.close()

    time.sleep(1)

    print("Application end")
    sys.stdout.flush()
    sys.exit()

if __name__ == "__main__":
    main()
