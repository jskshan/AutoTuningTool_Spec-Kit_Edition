@ECHO OFF
rem Setup the TCP port mapping
rem first  tcp:PC port
rem second tcp:Android Port
echo Setup the port mapping...
cd RemoteClient


rem enter adb shell
rem set adb to root
adb root
adb forward tcp:9344 tcp:9344
adb forward tcp:9345 tcp:9345
rem modify the access right
adb shell "chmod 777 /data/local/tmp/spi_socket_server"
rem execute the native ap
adb shell -t "cd /data/local/tmp/ && ./spi_socket_server -t 1 -d"
