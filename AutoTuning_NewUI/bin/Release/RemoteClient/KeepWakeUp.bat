@ECHO OFF
rem Setup the TCP port mapping
rem first  tcp:PC port
rem second tcp:Android Port
echo Setup the port mapping...
cd RemoteClient
adb forward tcp:9344 tcp:9344

rem enter adb shell
rem set adb to root
adb root
rem modify the access right
adb shell "input keyevent KEYCODE_WINDOW"
adb shell "input keyevent KEYCODE_WINDOW"
adb shell "input keyevent KEYCODE_WINDOW"
echo KeepWakeUp finish
