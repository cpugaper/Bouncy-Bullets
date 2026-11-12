# Bouncy Bullets

<img width="929" height="560" alt="bouncybulletsImg" src="https://github.com/user-attachments/assets/7e4db30f-cede-41e2-b13a-badcad695e41" />

![Unity](https://img.shields.io/badge/Unity-6000.3.6f1-black?style=flat&logo=unity)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20WebGL-blue?style=flat)

> **Bouncy Bullets** is an arcade-inspired 2.5D space shooter developed in Unity to practice clean architecture and performance optimization. Players control a ship, dodging and destroying diverse enemy waves in increasingly frenetic combat.

**[Play on Itch.io](https://pupecarla.itch.io/bouncy-bullets)** | **[View on Portfolio](https://cpugaper.github.io/)**

---

## Controls

The game supports dynamic input detection for both **Keyboard & Mouse** and **Gamepad**:

| Action | Keyboard & Mouse | Gamepad |
| :--- | :--- | :--- |
| **Move** | `W` `A` `S` `D` | Left Stick |
| **Aim** | Mouse Cursor | Right Stick |
| **Shoot** | `Left Click` | Right Trigger |
| **Shield** *(Temporary invincibility)* | `Q` | Left Bumper |
| **Turbo** *(Propulsion boost)* | `Left Shift` | Left Trigger |
| **Shockwave** *(Area blast)* | `E` | Right Bumper |
| **Pause** | `ESC` | `Start` |

---

## Architecture Highlights

* **Decoupled Events:** Asset-based Observer Pattern implemented via ScriptableObjects.
* **Memory & Rendering:** Object Pooling for projectiles, enemies, and VFX, alongside `MaterialPropertyBlock` and `Shader.PropertyToID` to avoid runtime material instancing.
* **Code Structure:** Modular organization using Assembly Definitions, namespaces, custom Editor tools, and a custom `SaveSystem`.

---

## Assets & Credits

Some 3D models, VFX, and audio used in this project are royalty-free assets from the **Unity Asset Store** and **Pixabay**. 
