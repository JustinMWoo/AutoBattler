# AutoBattler

## Overview
This repo contains the code used in a fully functioning single player autobattler. The game begins with the player in a buying phase where they are able to purchase units from a selection of random ones for the upcoming battles. Each unit has a set and tribe attribute that provides bonuses to the team depending on how many of that particular set/tribe are present on the player's team. Additionally, if three of the same unit are present they will be replaced with a more powerful version of the unit. Following this phase a randomly generated map for the current game will be shown to the player where they can view the paths available to them and choose their next destination. In combat nodes on the map the player will be faced with an enemy board layout which they can position against before starting combat. The battle will play out automatically when the player's positions are finalized. Eventually the player will arrive at the final  node on the map where they will be put up against a more powerful foe and upon defeating said enemy they will proceed to the next stage (with a new map and more powerful enemies).
<!-- full gameplay demonstration video -->

Note: Models and animations are temporary for demonstration purposes and are not included for download.  

## Gameplay Mechanics  
<!-- tiers, max units, currency -->
Units can be purchased between combat events and reorganized freely on the board. Units will automatically position themselves towards the front row and towards the middle column within that front row.  
<img src="Readme/Board.gif" width = "600">  

### Currency  
Currency can be used to purchase units and multiple other upgrades in the shop. Additional currency can be acquired from being victorious in combat events on the map.

### Tiers  
Units are sorted into tiers with the player able to purchase units from their player tier and below. Additional player tiers can be purchased from the shop allowing more powerful units to be available from that point forward.

### Unit Capacity  
Players are restricted in the number of units they can have on the board at one time. Additional capacity can be purchased from the shop and capacity will also increase when upgrading the player's tier.

## UI  
<!-- hover to show node contents on map, right click to see unit description/stats -->
**Bonuses**  
Bonuses (tribes and sets) that currently have one or more memeber are shown on the left of the screen. The UI is updated as units are added and removed from the board when out of combat to show which bonuses are active.  
<img src="Readme/Bonuses_UI.gif" width = "600">  
**Map**  
The map can be opened using the button on screen. When the map is active events can be hovered over to see details about them. Combat events will show the sets and tribes as well as the number of those types of units that the computer will have on their board if that path is chosen.  
<img src="Readme/Map_UI.gif" width = "600">  
**Unit**
Information about a unit can be seen by right clicking on it. The name of the unit along with it's current level is shown at the top of the display (level is shown with the number of stars). A brief description is also displayed with the stats of the unit on the right side. Additionally the unit's set and tribe is shown on the bottom and can be hovered over to display additional information about the bonus provided.  
<img src="Readme/Unit_UI.gif" width = "600">  

## Map  
The map is randomly generated at the start of each playthrough and follows a series of rules to determine the layout. Each path is guarenteed to have a path to the end node. Nodes each contain an event that is also randomly determined, with the chance of that event spawning being tied to a weight value of the node type.  
<details>
  <summary>Click to view a detailed explanation of how the map is generated</summary>  
  
</details>
Parameters that can be set include, the minimum/maximum number of nodes per level, the maximum of nodes that can be the destination of one node (number of forward connections) and the number of levels (nodes in a path from start to end).  
<p float="left">
  <img src="Readme/Map/map_1.png" width = "300"> 
  <img src="Readme/Map/map_2.png" width = "300"> 
  <img src="Readme/Map/map_3.png" width = "300">
</p>

### Shop 

## Combat  

### Bonuses (Tribes/Sets)
Note: For demonstration purposes some tribes and sets may be disabled on units when showcasing other bonuses and some unit's stats may be changed  
<details>
  <summary>Click to view implemented sets and tribes</summary>  
  
  ### Set 1  
</details>

### Units
<details>
  <summary>Click to view implemented units</summary>  
  
  ### Unit 1  
</details>

### Tripling
