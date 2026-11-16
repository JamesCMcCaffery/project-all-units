# PROJECT ALL UNITS

Asymmetric co-op police game for PC (Steam). One player is the **Dispatcher**: 911 calls, plate and licence checks, routing units, CCTV and bodycam feeds, radio. Everyone else is an **Officer** in the field.

## Stack
- **Unity 6 LTS**, URP
- **Netcode for GameObjects** (host-client, no dedicated servers)
- **Unity Transport** for LAN/IP testing now; **Facepunch.Steamworks** transport + Steam lobbies later
- **Dissonance** voice chat for the radio (step 3)
- **Multiplayer Play Mode** to test several players from one editor

## Roadmap: one-call vertical slice
1. **Foundations** - project, greybox street, shared first-person player, sitting, Dispatcher and Officer spawn over the network.
1.5. **First look** - Synty art in the dispatch room and one street corner, officer and dispatcher characters, warm stylised lighting and post. ← *we are here*
2. **Dispatcher desk MVP** - incoming call, plate/licence lookup, map with units, assign + waypoint, one CCTV and a bodycam feed.
3. **Officer field MVP** - radio (push-to-talk), simple car, fleeing suspect NPC, taser, handcuffs.
4. **The call, start to finish** - stolen vehicle scenario, playtest, decide the next slice.

## Opening the project (first time)
1. Install **Unity Hub**, then **Unity 6 LTS (6000.0.x)** with *Windows Build Support*.
2. Install **Git LFS** (`git lfs install`) before cloning, so art and audio come down properly.
3. In Unity Hub: **Add > Add project from disk** and pick this folder. Unity generates `ProjectSettings/` and `Library/` on first open (a few minutes). If it asks to enable the new Input System, say **Yes** (it restarts).
4. Commit the generated `ProjectSettings/` folder.

## Step 1 checklist (do this in the editor)
Code is in `Assets/_Project/Scripts`. You build the scene and prefabs:

Everyone, Dispatcher and Officer alike, uses the same first-person **Player** prefab. Roles differ by where you spawn and what you can do: the Dispatcher starts in the dispatch room, and sitting at the dispatch desk switches them to the desk screens. Shared mechanics (walking, sitting, and later the radio) live on the Player.

**Scene** `Assets/_Project/Scenes/Greybox.unity`
- A big Plane for the ground, a few Cubes as buildings and roads, and a cube "dispatch room" with a desk.
- Empty `NetworkManager` object: add **NetworkManager** + **Unity Transport**, plus **ConnectionMenu** and **RoleSpawner**.
- Empty `DispatchRoomSpawn` inside the room and 2-4 `OfficerSpawn` empties on the street. Assign them on RoleSpawner.
- **Dispatch desk chair:** a Cube with a Collider plus **DispatchDesk**. Give it two child empties, `SeatPoint` (where you sit, facing the monitors) and `StandPoint` (beside the chair), and a child **Camera** framing the monitors, assigned to `deskCamera`. Add more plain chairs with **Seat** if you like.

**Player prefab** `Prefabs/Player.prefab`
- Capsule with **CharacterController**, **NetworkObject**, **ClientNetworkTransform**, **PlayerController**, **PlayerRoleState**, **PlayerInteractor**.
- Child **Camera** at ~1.6 m height, assigned to `playerCamera` on PlayerController. Assign PlayerController to `controller` on PlayerInteractor.

**Wire up**
- NetworkManager: leave *Player Prefab* **empty**; add `Player.prefab` to the *Network Prefabs* list.
- RoleSpawner: assign `Player.prefab` and the spawn points.
- Delete the default Main Camera in the scene (each player brings their own).

**Test**
- *Window > Multiplayer > Multiplayer Play Mode*, enable 1-2 virtual players, press Play.
- Main editor: **Host** (you become Dispatch). Virtual player: **Join** (you become an Officer).
- Controls: WASD + mouse, Shift to sprint, E to sit or stand, Esc to free the cursor.
- Success = both players can walk around and see each other move, and the dispatcher pressing E on the desk chair switches to the desk camera with a free cursor.

## Step 1.5 checklist: first look
Goal: the greybox turns into a place. One dispatch room inside a police station, the street corner out front where officers spawn, real characters, and a warm, colourful-but-grounded look (reference: *RV There Yet?*). Recolouring and hero assets come later, before the Steam page.

**Golden rule: keep the gameplay object, swap the visual.** The objects with scripts (DispatchDesk, Seat, spawn points) stay where they are. Put the Synty model *inside* them as a child and turn off the grey cube's Mesh Renderer, keeping its Collider. Like welding a MeshPart to the scripted Part in Roblox.

**1. Import the art**
- *Window > Package Manager > My Assets*: download and import POLYGON Police Station, City and City Characters (Town later if needed). They land in `Assets/Synty/...` (or `Assets/Polygon...`), which git ignores.
- Pink materials mean they are still Built-in. First look inside each pack's folder for a `.unitypackage` with **URP** in the name and double-click it. If there isn't one: *Window > Rendering > Render Pipeline Converter*, pick *Built-in to URP*, tick **Material Upgrade**, *Initialize And Convert*. Anything still pink (often glass or water) gets a plain **Universal Render Pipeline/Lit** material.

**2. Lighting and post (do this first, on the greybox: instant payoff)**
- Check URP is actually on: *Edit > Project Settings > Graphics > Default Render Pipeline* must name a URP Asset. If it's empty, *Create > Rendering > URP Asset (with Universal Renderer)* in `Assets/_Project/Settings` and assign it there and in *Quality*. (The tool below refuses to run without one.)
- Open `Greybox.unity` and run *Tools > Project All Units > Apply First Look Lighting*. It creates a Global Volume (`Art/Lighting/FirstLook_Volume.asset`: neutral tonemapping, warm grade, slight bloom and vignette), sets a warm sun, sky-tinted ambient and light haze, and turns on Post Processing for the scene cameras and the Player prefab camera. Running it again won't overwrite your tweaks to the profile.
- Ambient occlusion lives on the renderer, not the volume. *Edit > Project Settings > Graphics* shows your URP Asset; click it, then double-click the Renderer in its *Renderer List* > *Add Renderer Feature > Screen Space Ambient Occlusion*. Start with defaults; lower *Intensity* if corners look dirty.
- On the URP Asset itself: *Shadows > Max Distance* ~80, *Cascade Count* 4, *Soft Shadows* on.
- Sky: if a pack includes a sky material (search `Sky t:Material` in the Project window), set it in *Window > Rendering > Lighting > Environment > Skybox Material*.

**3. Dispatch room (inside the police station)**
- Use the Police Station pack's modular walls, floor, ceiling, doors and windows to rebuild the greybox room around the existing desk. Turn on grid snapping (the magnet button at the top of the Scene view) and set the move increment to one wall piece's width, or hold **V** to vertex-snap pieces together.
- Dress it: desks, monitors, keyboards, office chairs, filing cabinets, a map board, plants, mugs. A busy desk sells "dispatch" more than anything.
- Put a Synty office chair inside `DispatchDesk` (child), position it under `SeatPoint`, and turn off the cube's Mesh Renderer (keep its Box Collider so E still finds it). Re-check that the desk Camera frames the new monitors.
- Light it: a Point Light under each ceiling light model, warm colour (around `#FFD9A6`), Intensity ~2, Range ~6. Let the sun come through a window. Turn the Global Volume's exposure down a touch if the room blows out.

**4. Street corner (out front)**
- Place the Police Station exterior so the dispatch room sits inside it, facing a single crossroads built from City road and sidewalk pieces.
- Three other corners: City buildings, plus props (street lamps, hydrants, bins, benches, signs, trees, a few parked cars as static props for now; driveable cars come with NWH in step 3).
- Move the `OfficerSpawn` points onto the pavement outside the station doors.
- Select all environment pieces and tick **Static** (top right of the Inspector). Synty models often have no collider: if you walk through something in Play mode, add a Box Collider (or Mesh Collider for odd shapes).

**5. Characters**
- Animations from [Mixamo](https://www.mixamo.com): *Idle*, *Walking* and *Running*, ticking **In Place** where offered, *FBX for Unity*, *Without Skin*. Put them in `Art/Animations/Mixamo`. For each FBX: *Rig > Animation Type: Humanoid > Apply*, then *Animation > Loop Time* on.
- *Create > Animator Controller* `Art/Animations/Player.controller`. Add a Float parameter **Speed**. Right-click the graph > *Create State > From New Blend Tree*, double-click it, set it to *1D* on Speed with three motions: Idle at 0, Walking at 4, Running at 7 (untick *Automate Thresholds*; these match the walk and sprint speeds).
- Open `Player.prefab`. Turn off the capsule's Mesh Renderer. Drag in a police officer character from the Police Station pack (search `Chr` in the Project window) as a child named `OfficerModel`, and a civilian office character as `DispatcherModel`. Move both down so their feet touch the bottom of the green CharacterController outline.
- On each model: if the character's FBX rig isn't Humanoid, set it as above. On its **Animator**, set Controller to `Player.controller` and untick *Apply Root Motion*. Add **PlayerAnimator** to each.
- On the Player root add **PlayerAppearance** and assign both models. It shows the right one for the role and hides your own body from your camera (you still cast a shadow).
- Test with Multiplayer Play Mode: the officer should walk and run with animation as seen from the dispatcher, and vice versa.

**Done when:** a screenshot from the officer spawn and one from the dispatch desk would look at home on a Steam page draft. Send both, plus one wide Scene-view shot, for a look-dev pass.

## Layout
```
Assets/_Project/
  Scripts/  Core, Player, Interaction, Networking, Dispatch  (one asmdef: ProjectAllUnits)
  Prefabs/  Scenes/  Art/  Audio/
```
Third-party assets (Dissonance, Steamworks) go in their own top-level folders under `Assets/`, outside `_Project`.
