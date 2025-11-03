## 🎮 Theme

*A future but not for us* is a videogame that explores the intrusion of artificial intelligences into activities we consider fun, from gaming to socializing.  
It's a **narrative/serious game** with a **cyberpunk aesthetic** that evokes the 80s.  

The objective is to survive waves of robots while getting upgrades, **but your companion, a small AI-powered robot, will increasingly try to control your character.**  
**Who will be the last one playing?**

---

## 🧠 My Role

In this project, I was responsible for the **technical development** as the only programmer.  
The following points are the key highlights of my work:

- **Creation of the main character's AI**  
- **Programming of gameplay mechanics (2D and 3D)**  
- **Implementation of dialogs and localization systems**, including a tool for switching texts and audio to multiple languages using CSV and JSON files  
- **Development of UI programming** (main menu and gameplay interface)

---

## ⚙️ Technical Achievements

### 🧭 Dynamic Pathfinding with Context Steering

At a certain point in the game, your character moves on its own and adopts two behaviors: **fleeing** and **chasing targets** among hundreds of moving enemies.  

To achieve this, I implemented a system inspired by the bot movement in the game *F1 2011*, as explained in the article  
**[Context Steering, Behavior-Driven Steering at the Macro Scale](https://www.gamasutra.com/view/news/128086/Context_Steering_BehaviorDriven_Steering_at_the_Macro_Scale.php)** by *Andrew Fray*.

It consists of building **context maps** by projecting the character in different directions and calculating how dangerous or interesting those positions are, based on distance, angles, and other parameters relative to nearby enemies.  

I mainly developed two types of behaviors: **fleeing** and **chasing enemies**. These are easy to modify, and their logic (similar to weight distribution in a 3D rig) has the potential to produce other behaviors. When combined, they generate very natural movements.

**Performance:** *0.30–0.65 ms per agent* through full math calculations (**without raycast**), recalculating every 3 frames, enabling movement through hundreds of enemies without performance impact.
