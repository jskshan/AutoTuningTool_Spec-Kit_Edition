@ECHO OFF
cd RemoteClient
rem Install qpython
echo Install the QPyhon to android device
adb install -r Data\QPython.apk

rem Run QPython Terminal
echo Setup the ptyhon environment

adb shell am start -n org.qpython.qpy3/jackpal.androidterm.Term

echo Setup QPython finish.