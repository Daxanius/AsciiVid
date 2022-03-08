echo downloading required files

set outputFolder=%1
set ffmpegLocation=https://www.dropbox.com/s/bid0phd1ug6u8o2/ffmpeg.exe?dl=1

cd %outputFolder%
if not exist ffmpeg.exe curl -L %ffmpegLocation% > ffmpeg.exe
exit