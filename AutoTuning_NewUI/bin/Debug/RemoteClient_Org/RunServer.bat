@ECHO OFF
rem Setup the TCP port mapping
rem first  tcp:PC port
rem second tcp:Android Port
echo Setup the port mapping...
cd RemoteClient
adb forward tcp:9344 tcp:9344
adb forward tcp:9345 tcp:9345

rem enter adb shell
rem set adb to root
adb root
adb shell setenforce 0
rem modify the access right
adb shell "chmod 777 /data/local/tmp/RunRemoteServer.sh"
rem execute the python
adb shell -t "cd /data/local/tmp/ && ./RunRemoteServer.sh"
