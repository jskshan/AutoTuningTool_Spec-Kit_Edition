@ECHO OFF
rem Copy the Remote AP to Android Temp folder
cd RemoteClient
echo Put all the data to /data/local/tmp

adb push Data/spi_socket_server /data/local/tmp/
echo Install finish