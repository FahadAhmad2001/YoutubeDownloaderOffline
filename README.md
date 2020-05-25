# LightSpeed YouTube Downloader

Free, easy to use YouTube downloader

## Features

- Download videos or MP3
- Variable qualities supported (for both audio and video) when downloading a video
- See the file size before downloading

## Currently under development

- Subtitle support
- Playlist support

## Backend Server Usage

You can use the backend server to host your own YouTube downloading website

### Server configuration

By default, when you start the server, a configuration file (config.ini) is created in the server directory, where you can edit the following variables:

Port: Set to 9000 by default

DownloadDir: Location where videos are downloaded and saved. By default, its set to "Default", which means it'll download videos in the same path as the server executable. You do not need to put double quotes around the path, even if it has spaces

### Usage

For metadata extraction: `http://ServerIP:ServerPort/getmetadata?vidID=(Video ID)`

For downloading an MP3: `http://ServerIP:ServerPort/downloadvid?vidID=(Video ID)&isMP3=true&embedThumbnail=(true/false)`

For downloading as a video: `http://ServerIP:ServerPort/downloadvid?vidID=(Video ID)&isMP3=false&videoID=(Video Quality ID)&audioID=(Audio Quality ID)&videoFormat=(Video Quality Format)&audioFormat=(Audio Quality Format)`

For saving the file to your device: `http://ServerIP:ServerPort/download?fileID=(Downloaded File ID)`

For instance, for the video https://www.youtube.com/watch?v=fFCBJDqMEm8 running the server at port 9000 and connecting from localhost

Step 1: To get the metadata, use `http://127.0.0.1:9000/getmetadata?vidID=fFCBJDqMEm8` (fFCBJDqMEm8 is from the YouTube URL after the watch?v=)

That should provide the following output:
```
<VideoMetadata>
<VideoID>fFCBJDqMEm8</VideoID>
<Title>PUNYASO - Konoha's Kid (Naruto Tribute) </Title>
<ThumbnailURL>
https://i.ytimg.com/vi/fFCBJDqMEm8/maxresdefault.jpg
</ThumbnailURL>
<VideoQualities>
<vidquality format="mp4" size="1.12MiB" resolution="144p" ID="394"/>
<vidquality format="webm" size="1.30MiB" resolution="144p" ID="278"/>
<vidquality format="mp4" size="2.01MiB" resolution="144p" ID="160"/>
<vidquality format="mp4" size="1.97MiB" resolution="240p" ID="395"/>
<vidquality format="webm" size="2.78MiB" resolution="240p" ID="242"/>
<vidquality format="mp4" size="3.13MiB" resolution="240p" ID="133"/>
<vidquality format="mp4" size="3.86MiB" resolution="360p" ID="396"/>
<vidquality format="webm" size="4.97MiB" resolution="360p" ID="243"/>
<vidquality format="mp4" size="6.10MiB" resolution="360p" ID="134"/>
<vidquality format="mp4" size="6.60MiB" resolution="480p" ID="397"/>
<vidquality format="webm" size="8.48MiB" resolution="480p" ID="244"/>
<vidquality format="mp4" size="10.27MiB" resolution="480p" ID="135"/>
<vidquality format="mp4" size="16.84MiB" resolution="720p" ID="398"/>
<vidquality format="webm" size="17.44MiB" resolution="720p" ID="247"/>
<vidquality format="mp4" size="21.89MiB" resolution="720p" ID="298"/>
<vidquality format="webm" size="34.96MiB" resolution="1080p" ID="248"/>
<vidquality format="mp4" size="36.45MiB" resolution="720p" ID="136"/>
<vidquality format="webm" size="30.30MiB" resolution="720p" ID="302"/>
<vidquality format="webm" size="70.49MiB" resolution="1080p" ID="303"/>
<vidquality format="mp4" size="60.58MiB" resolution="1080p" ID="137"/>
<vidquality format="mp4" size="75.81MiB" resolution="1080p" ID="299"/>
</VideoQualities>
<AudioQualities>
<audquality format="webm" size="1015.77KiB" ID="249"/>
<audquality format="webm" size="1.30MiB" ID="250"/>
<audquality format="webm" size="2.55MiB" ID="251"/>
<audquality format="m4a" size="2.68MiB" ID="140"/>
</AudioQualities>
</VideoMetadata>
```
Step 2: Now, for instance, to download with ID 133 for the video quality (MP4, 240p, 3.13 MB) and ID 140 for the audio quality (M4A, 2.68MB), we would use `http://127.0.0.1:9000/downloadvid?vidID=fFCBJDqMEm8&isMP3=false&videoID=133&audioID=140&videoFormat=mp4&audioFormat=m4a`

That could provide a sample output of: 
```
Downloading:   0.0% of 3.13MiB at  7.81KiB/s ETA 06:50
Downloading:   0.1% of 3.13MiB at 23.25KiB/s ETA 02:17
Downloading:   0.2% of 3.13MiB at 54.26KiB/s ETA 00:58
Downloading:   0.5% of 3.13MiB at 116.27KiB/s ETA 00:27
Downloading:   1.0% of 3.13MiB at 122.52KiB/s ETA 00:25
Downloading:   2.0% of 3.13MiB at 123.04KiB/s ETA 00:25
Downloading:   4.0% of 3.13MiB at 99.06KiB/s ETA 00:31
Downloading:   6.6% of 3.13MiB at 115.31KiB/s ETA 00:25
Downloading:  11.4% of 3.13MiB at 116.03KiB/s ETA 00:24
```
(I've removed the middle of the output since its really long and only shows the progress)
```
Downloading:  74.4% of 2.68MiB at 64.10KiB/s ETA 00:10
Downloading:  77.4% of 2.68MiB at 66.52KiB/s ETA 00:09
Downloading:  83.3% of 2.68MiB at 68.79KiB/s ETA 00:06
Downloading:  87.8% of 2.68MiB at 70.02KiB/s ETA 00:04
Downloading:  91.6% of 2.68MiB at 70.70KiB/s ETA 00:03
Downloading:  95.0% of 2.68MiB at 71.66KiB/s ETA 00:01
Downloading:  99.1% of 2.68MiB at 71.63KiB/s ETA 00:00
Downloading: 100.0% of 2.68MiB at 72.11KiB/s ETA 00:00
COMPLETED:PUNYASO*-*Konoha's*Kid*(Naruto*Tribute)-fFCBJDqMEm8.mp4
```
Step 3: Now, if we want to save the video to our device (as its currently on the server, we could use `http://127.0.0.1:9000/download?fileID=PUNYASO*-*Konoha's*Kid*(Naruto*Tribute)-fFCBJDqMEm8.mp4` 

`PUNYASO*-*Konoha's*Kid*(Naruto*Tribute)-fFCBJDqMEm8.mp4` is from the text after `COMPLETED:` when downloading the video in the previous step

## Contributions
Feel free to submit a pull request if you've come up with any improvements/bug fixes!

## Issues
If you need to report any issues/suggest any enhancements, or if you have any questions, please create a new issue in the issues tab
