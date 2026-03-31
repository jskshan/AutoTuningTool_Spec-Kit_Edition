@ECHO OFF
rem Copy the Remote AP to Android Temp folder
cd RemoteClient
echo Put all the data to /data/local/tmp
adb push Data/RemoteAP.py /data/local/tmp/
adb push Data/libTouch.so /data/local/tmp/
adb push Data/libusb-1.0.so /data/local/tmp/
adb push Data/RunRemoteServer.sh /data/local/tmp/
adb push Data/RunRemoteClient.sh /data/local/tmp/
echo Install finish