# OfflineChat
A text chat program relying on a network storage device to transmit messages.

![image](https://user-images.githubusercontent.com/26361108/158031321-c2d1482a-b6d7-4b03-8152-aff3f31b3228.png)

## How it works:
Upon starting the program it will try to create a "chat.log" file, if it already exits it will open it instead. This file will store all messages, all users currently using the program and all banned users. Your username will be the username of your computer. 

The program constantly reads the file for any updates.
Sending a chat message will append the following formatted text to the log file:\
`[Time in mm:ss format] <Username of the PC>: message`

Type `-listcommands` for a list of chat commands

If the last person left the room, the chat will clear.

## How to use:
1. Download `OfflineChat.zip` from the ["Releases" tab](https://github.com/NotLe0n/OfflineChat/releases)
2. Extract the zip file
3. Open OfflineChat.exe
4. Start chatting

## Dependencies:
* https://github.com/zaafar/ClickableTransparentOverlay
* https://github.com/mellinoe/ImGui.NET/
* https://github.com/mellinoe/veldrid
