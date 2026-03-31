@ECHO OFF
rem Copy the Remote AP to Android Temp folder
echo Remove all the data in /data/local/tmp
adb shell rm /data/local/tmp/RemoteAP.py
adb shell rm /data/local/tmp/libTouch.so
adb shell rm /data/local/tmp/libusb-1.0.so
adb shell rm /data/local/tmp/RunRemoteServer.sh 
echo Remove finish