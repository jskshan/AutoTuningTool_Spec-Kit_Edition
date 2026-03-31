adb root
adb forward tcp:9344 tcp:9344
adb push touch_data_agent /data/local/tmp/
adb shell "chmod 777 /data/local/tmp/touch_data_agent"
adb shell "chmod 777 /dev/elan-iap"
::-t interface: 3(I2C), 4(I2CHID), 5(I2C_CHROME),6(ELAN_I2CHID),8(ELAN_I2C_CHROME),9(SPI),10(AFE_SPI)
adb shell "/data/local/tmp/touch_data_agent -t 6 -s 1 -a 127.0.0.1 -n 9344 -i 0 -b 0 -d
pause