# PROJECT ALL UNITS

Asymmetric co-op police game for PC (Steam). One player is the **Dispatcher** (911 calls, plate and licence checks, routing units, CCTV and bodycam feeds, radio); everyone else is an **Officer** in the field (driving, taser, handcuffs, chasing suspects). README.md has the full roadmap and the step-by-step editor checklists.

## Working with James
- James is a seasoned Roblox developer and knows C#, but is **new to Unity**. Explain editor steps plainly and concretely: exact menu paths (*Window > Package Manager*), which object to select, which Inspector field to set. Roblox analogies help (prefab ≈ model in ReplicatedStorage, NetworkBehaviour ≈ script with server/client sides).
- You cannot run the Unity editor. Write the code, then give James the editor steps (scene objects, prefab wiring, Inspector values) as a checklist, in the README step section or the PR description. Where wiring is fiddly, prefer an editor menu tool (see `Scripts/Editor/FirstLookSetup.cs`) over long manual instructions.
- Remind him to save the scene (Ctrl+S) before testing with Multiplayer Play Mode: virtual players load scenes from disk.
- James uses Rider and reviews through GitHub PR comments. Work on a branch and open a PR; never push to `main`.

## Stack
- **Unity 6 LTS** (`6000.0.85f2`), **URP**.
- **Netcode for GameObjects**, host-client (no dedicated servers). Unity Transport for LAN/IP testing now; Unity Relay for early online tests; **Facepunch.Steamworks** transport and Steam lobbies later.
- **Dissonance** for voice and radio (step 3).
- **Multiplayer Play Mode** to test several players from one editor.
- **Input System** (new), read directly via `Keyboard.current` / `Mouse.current` for now.

## Roadmap (one-call vertical slice)
1. Foundations: greybox street, shared first-person player, sitting, Dispatcher/Officer spawning over the network. **Done.**
1.5. First look: Synty art in the dispatch room and one street corner, characters, lighting and post. **← current**
2. Dispatcher desk MVP: incoming call, plate/licence lookup, map with units, assign + waypoint, one CCTV and one bodycam feed.
3. Officer field MVP: push-to-talk radio, driveable car, fleeing suspect NPC, taser, handcuffs.
4. The full call start to finish (stolen vehicle), playtest, plan the next slice.

Keep work scoped to the current step unless asked.

## Architecture
- **One shared Player prefab for every role.** Dispatcher and Officers use the same first-person player (walk, sit, interact, later radio). Roles differ by spawn point and what they can do: the Dispatcher spawns in the dispatch room, and sitting at the `DispatchDesk` (a `Seat`) switches to the desk screens. Add role features to the shared player; never write a separate controller per role.
- `RoleSpawner` (server) spawns each client; the first is Dispatcher, the rest Officers. `PlayerRoleState` holds the role in a server-written `NetworkVariable`.
- Movement is owner-authoritative (`ClientNetworkTransform`). Gameplay state that matters (arrests, call status, assignments) should be server-authoritative.
- Interactables implement `IInteractable` (look at it, press E).

## Layout
```
Assets/_Project/
  Scripts/   Core, Player, Interaction, Networking, Dispatch   (asmdef: ProjectAllUnits)
             Editor/                                           (asmdef: ProjectAllUnits.Editor, editor-only)
  Prefabs/   Scenes/ (Greybox.unity)   Art/   Audio/
```
- All project content lives under `Assets/_Project`. Third-party assets go in their own top-level folders under `Assets/`.
- Namespaces mirror folders under `Assets/_Project/Scripts` (`ProjectAllUnits.<Folder>`). Add a folder when a feature needs one (e.g. `Officer`, `Vehicles`, `Radio`).
- Commit `.meta` files alongside their assets. Art and audio go through Git LFS (`.gitattributes`).

## Code style
- Comments are minimal, following common C# industry practice:
  - One or two sentences of `/// <summary>` per class. No setup instructions, roadmap notes or rationale essays in code; those belong in the README or PR description.
  - Code should be self-documenting: clear names and small, well-named methods instead of `// Look`, `// Move` style section comments.
  - Inline comments only where the code is genuinely hard to follow or does something non-obvious (a workaround, a networking subtlety, a magic number).
- Match the existing style: implicit `private`, camelCase fields without underscores, `[SerializeField]` for Inspector references, expression-bodied members for one-liners.

## Art style
Stylised, colourful but grounded: between clean low-poly and semi-realistic. Reference: *RV There Yet?* Not realistic, not cartoony-crazy. James finds greyboxes hard to read, so get real models in early.

Synty POLYGON packs are for prototyping. Before the Steam page: palette recolour, a signature shader/post look, and custom hero assets (officer, police car, dispatch desk and screens, UI). When dressing a scene, keep the scripted gameplay object and put the art model inside it as a child.

## Owned assets (use these before writing or buying alternatives)
Paid, imported from the Asset Store into their own folders under `Assets/` (not `_Project`). Not committed to git (size and licence, see `.gitignore`); re-import from *Package Manager > My Assets* on a fresh clone, so code referencing them won't compile in a clean checkout:
- **Dissonance Voice Chat**: all voice and radio.
- **NWH Vehicle Physics 2**: all driveable vehicles (no hand-written car controller).
- **DOTween Pro**: tweens and UI motion.
- **Feel** (More Mountains): camera shake, impacts and other game feel.
- **Synty POLYGON** Police Station, City, City Characters and Town packs: environment and characters.

Free: ProBuilder (greyboxing), Cinemachine, AI Navigation (NPC pathing), Input System, Netcode for GameObjects, Multiplayer Play Mode, Mixamo animations (Humanoid rig), Facepunch.Steamworks (later).
