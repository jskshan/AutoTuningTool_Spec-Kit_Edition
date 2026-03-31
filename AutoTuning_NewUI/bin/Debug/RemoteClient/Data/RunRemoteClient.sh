#!/system/bin/sh
# Run the socket server in android
# Set the qpython path
PATH=$PATH:/data/data/org.qpython.qpy3/files/bin

# Add permission to executable binary
chmod 777 /data/local/tmp/RemoteAP.py
chmod 777 /data/local/tmp/libTouch.so
chmod 777 /data/local/tmp/libusb-1.0.so

# Run remote ap in android with socket server
echo "Run the remote ap(Socket Server)..."
# Parameter define
# -p : port number, must be the same as remote client
# -t : Run Remote AP with specific interface
#       1:HID
#       2:HID Android
#       3:I2C Android
#       4:I2CHID Android
#       5:I2C Chrome
#       6:I2CHID Android(Elan)
#	-a : android via usb cable
# -d : Debug Mode
# -s : SYSFS
#        bus number

qpython-android5.sh /data/local/tmp/RemoteAP.py -i 127.0.0.1 -p 9344 -t 6

exit 0
