# AoE4BO
Age of Empires 4 build order overlay for windows

## Introduction
This tool will make it easier for you to learn build orders. The next steps of the build order are conveniently displayed in the game and update automatically according to the progress.

## Installation
Video instruction: https://www.youtube.com/watch?v=eDvuo8e2KRk  

- Download latest release (https://github.com/nordie92/AoE4BO/releases)
- Unzip
- Execute "AoE4BO.exe" to start

## Usage
- Execute "AoE4BO.exe"
- Open a build order. There is a example build order "2-base-default.aoe4bo"
- Start Match

## Build orders
Share your build order! https://steamcommunity.com/groups/Aoe4BO

## Create your own build order
Create a text file and write down one line for every instruction.
At first you have to define the requirements. For example "7s" (when reaches 7 supply). Or "7s,100f" (when reaches 7 supply and 100 food). Next close this requirement section by typing ":". Finally write down the instruction, "build house" for example.
There are 7 possible requirements implemented: Supply(s), Supply cap(sc), food(f), wood(w), gold(g), stone(st) and time(t, in seconds). If you want to write more then one instruction just repeat the same requirements on the next line.

### Example:  
7s: When 7 Supply  
20sc: When 20 SupplyCap  
200f: When you reached 200 food  
200w: When you reached 200 wood  
200g: When you reached 200 gold  
200st: When you reached 200 stone  
2t: When 2 seconds pased
400f,200g: Two requirements are possible too

## Text recognition failed
Text recognition (to parse supply and resources) will fail if your resolution isn't 1920x1080 or if you change the ui scale. But you can solve this issue by setting up position and size of depending on the resolutions. It can help to take a screenshot from your game and find out the position of supply and resources.

## Feedback please
I can only test the code on my one mashine. So i would be happy to receive feedback or error messages.
