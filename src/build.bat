echo downloading required files

set outputFolder=%1
set ffplayLocation="https://spooder.be/secret/ffplay.exe"

cd %outputFolder%
if not exist ffplay.exe curl -L %ffplayLocation% > ffplay.exe
exit