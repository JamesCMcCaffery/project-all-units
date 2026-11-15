# PROJECT ALL UNITS

Asymmetric co-op police game for PC (Steam). One player is the **Dispatcher**: 911 calls, plate and licence checks, routing units, CCTV and bodycam feeds, radio. Everyone else is an **Officer** in the field.

## Stack
- **Unity 6 LTS**, URP
- **Netcode for GameObjects** (host-client, no dedicated servers)
- **Unity Transport** for LAN/IP testing now; **Facepunch.Steamworks** transport + Steam lobbies later
- **Dissonance** voice chat for the radio (step 3)
- **Multiplayer Play Mode** to test several players from one editor

## Roadmap: one-call vertical slice
1. **Foundations** - project, greybox street, shared first-person player, sitting, Dispatcher and Officer spawn over the network. ← *we are here*
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

## Layout
```
Assets/_Project/
  Scripts/  Core, Player, Interaction, Networking, Dispatch  (one asmdef: ProjectAllUnits)
  Prefabs/  Scenes/  Art/  Audio/
```
Third-party assets (Dissonance, Steamworks) go in their own top-level folders under `Assets/`, outside `_Project`.
