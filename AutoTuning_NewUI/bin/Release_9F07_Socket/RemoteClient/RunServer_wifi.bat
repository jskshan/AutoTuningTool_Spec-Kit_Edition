@ECHO OFF
adb connect 192.168.0.26
rem Setup the TCP port mapping
rem first  tcp:PC port
rem second tcp:Android Port
echo Setup the port mapping...
cd RemoteClient
adb root
adb forward tcp:12345 tcp:12345
adb shell chmod 777 /data/local/tmp/thp/testafehal-elan
adb shell -t "cd /data/local/tmp/thp && ./testafehal-elan"
