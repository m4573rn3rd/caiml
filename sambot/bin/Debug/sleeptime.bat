echo off
C:\Windows\System32\powercfg.exe -Change -monitor-timeout-dc 1
C:\Windows\System32\powercfg.exe -Change -monitor-timeout-ac 1
START /MIN /WAIT C:\Windows\System32\scrnsave.scr -s
C:\Windows\System32\powercfg.exe -Change -monitor-timeout-dc m1
C:\Windows\System32\powercfg.exe -Change -monitor-timeout-ac m2