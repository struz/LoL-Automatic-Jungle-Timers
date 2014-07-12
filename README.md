# LoL Automatic Jungle Timers

## About
This is an automatic jungle timer for League of Legends, inspired by Curse Voice's (now removed) jungle timers. I wanted to see if I could replicate the functionality found in Curse Voice, and it led me into learning proper reverse engineering techniques (eventually ending with the creation of another LoL timing project: [LoL-Ability-Timers](http://github.com/struz/LoL-Ability-Timers)).

This program times Dragon, Baron, and the red and blue buffs of both sides of the jungle. The timer for a given neutral monster camp is started automatically when the last monster from that camp dies. The monster must be seen by the player as it is killed for the timer to start correctly.

## Usage instructions
Run the WindowsFormOverlay.exe file after building the solution. This will automatically detect when league of legends is open and show the overlay, hiding it once again when league of legends closes. To close the overlay just close the program's window.

## Notes
I have tried to make this as patch resistant as possible, but as with all multiplayer reverse engineering endeavours it is likely that something will change down the line and break the program. The program definitely works as of patch 4.11.

I don't know if I will be regularly updating this, but if you have any requests or improvements please feel free to email me.

## Libraries / external files used
This project uses [SigScan](http://github.com/aikar/SigScan) by aikar to perform byte pattern scanning and make this as patch resistant as possible.

## Legal disclaimer
Use of this program is done entirely at the user's discretion. I take no responsibility for anyone using this program who gets punished in any way by Riot Games, or for any other problems resulting from the use of this software.