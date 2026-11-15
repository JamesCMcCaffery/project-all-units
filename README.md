# PROJECT ALL UNITS

Asymmetric co-op police game for PC (Steam). One player is the **Dispatcher**: 911 calls, plate and licence checks, routing units, CCTV and bodycam feeds, radio. Everyone else is an **Officer** in the field.

## Stack
- **Unity 6 LTS**, URP
- **Netcode for GameObjects** (host-client, no dedicated servers)
- **Unity Transport** for LAN/IP testing now; **Facepunch.Steamworks** transport + Steam lobbies later
- **Dissonance** voice chat for the radio (step 3)
- **Multiplayer Play Mode** to test several players from one editor

## Roadmap: one-call vertical slice
1. **Foundations** - project, greybox street, Dispatcher and Officer spawn over the network. ← *we are here*
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

**Scene** `Assets/_Project/Scenes/Greybox.unity`
- A big Plane for the ground, a few Cubes as buildings/roads.
- Empty `NetworkManager` object: add **NetworkManager** + **Unity Transport**, plus **ConnectionMenu** and **RoleSpawner**.
- Empty `DispatchDeskSpawn` (anywhere, e.g. inside a cube "station") and 2-4 `OfficerSpawn` empties on the street. Assign them on RoleSpawner.

**Officer prefab** `Prefabs/Officer.prefab`
- Capsule with **CharacterController**, **NetworkObject**, **ClientNetworkTransform**, **OfficerController**.
- Child **Camera** at ~1.6 m height, assigned to `playerCamera`.

**Dispatcher prefab** `Prefabs/Dispatcher.prefab`
- Empty with **NetworkObject**, **DispatcherStation**.
- Child **Camera** looking at a desk, assigned to `deskCamera`.

**Wire up**
- NetworkManager: leave *Player Prefab* **empty**; add both prefabs to the *Network Prefabs* list.
- RoleSpawner: assign both prefabs and the spawn points.
- Delete the default Main Camera in the scene (each player brings their own).

**Test**
- *Window > Multiplayer > Multiplayer Play Mode*, enable 1-2 virtual players, press Play.
- Main editor: **Host** (you become Dispatch). Virtual player: **Join** (you become an Officer and can walk around with WASD + mouse, Shift to sprint, Esc to free the cursor).
- Success = the officer's movement is visible from another officer, and the dispatcher sees the desk view.

## Layout
```
Assets/_Project/
  Scripts/  Core, Networking, Officer, Dispatch  (one asmdef: ProjectAllUnits)
  Prefabs/  Scenes/  Art/  Audio/
```
Third-party assets (Dissonance, Steamworks) go in their own top-level folders under `Assets/`, outside `_Project`.
