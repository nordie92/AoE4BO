# AoE4BO
Age of Empires 4 build order overlay for windows

## Introduction
This tool will make it easier for you to learn buildorders. The next steps of the build order are conveniently displayed in the game and update automatically according to the progress.

## Installation
- Download latest release
- Unzip
- Execute "AoE4BO.exe" to start

## Usage
- Execute "AoE4BO.exe"
- Open a build order. There is a example build order "2-base-default.aoe4bo"
- Start Match

## Build orders
Please share your build order! https://steamcommunity.com/groups/Aoe4BO

## Create your own build order
Creating your own build order is easy. Create a text file and write down one line for one instruction.
At first you have to define the requirements. For example "7s" (when reaches 7 supply).Or "7s,100f" (when reaches 7 supply and 100 food). Next close this requirement section by typing ":". Finally write down the instruction, "build house" for example.
There are 7 possible requirements implemented: Supply(s), Supply cap(sc), food(f), wood(w), gold(g), stone(st) and time(t, in seconds). If you want to write more then one instruction just repeat the same requirements on the next line.

## Text recognition failed
Text recognition (to parse supply and resources) will fail if your resolution isn't 1920x1080 or if you change the ui scale. But you can solve this issue by setting up position and size of depending on the resolutions. It can help to take a screenshot from your game and find out the position of supply and resources.
