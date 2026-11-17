# PROJECT ALL UNITS

Asymmetric co-op police game for PC (Steam): one Dispatcher, the rest Officers. Unity 6 LTS (6000.0.85f2), URP, Netcode for GameObjects (host-client). README.md has the stack, roadmap, setup and the current step's editor checklist; keep it up to date when a step changes.

## Working with James
- James is new to Unity (ex-Roblox, knows C#, uses Rider). He does editor work himself: explain editor steps as short numbered lists with exact menu paths, and use Roblox analogies where they help.
- Write C# and editor tooling; don't hand-edit `.unity`, `.prefab` or `.asset` YAML unless there is no other way. Prefer an editor script under `Scripts/Editor` (menu: *Tools > Project All Units*) for repeatable scene setup.
- Scripts are not compiled in CI. After code changes, ask James to check the Unity Console for red errors.

## Architecture
- Everyone uses one shared first-person `Player` prefab. Roles differ by spawn point and what they can do; role-specific features hang off the shared player, never a separate controller.
- `RoleSpawner` spawns players (first = Dispatcher, rest = Officers) and sets `PlayerRoleState` (server-written NetworkVariable, `RoleChanged` event for clients).
- Movement is owner-authoritative via `ClientNetworkTransform`. Visual-only things (animation, which model shows) are derived locally on each client, not synced.
- Interactables implement `IInteractable`; `Seat` and `DispatchDesk : Seat` are the examples. Keep the gameplay object and swap only its visual child when dressing with art.

## Repo layout
- Our code and content: `Assets/_Project/{Scripts,Prefabs,Scenes,Art,Audio,Settings}`. Main scene: `Scenes/Greybox.unity`. URP asset and renderer: `_Project/Settings`.
- Runtime asmdef `ProjectAllUnits` in `Scripts/`; editor-only asmdef `ProjectAllUnits.Editor` in `Scripts/Editor/`.

## Git
- Always commit `.meta` files alongside their assets and scripts (missing metas break references on other machines).
- Never commit the paid asset folders (`Assets/Synty`, NWH, Feel, Dissonance, `Plugins/Demigiant`); they are gitignored. If `git status` shows them, fix `.gitignore` rather than committing.
- Art, audio and models go through Git LFS (`.gitattributes`).

## Gotchas
- Multiplayer Play Mode virtual players load scenes from disk: save (Ctrl+S) before pressing Play. If a virtual player throws "Could not read file ... InternalAPIEditorBridge", close Unity and delete `Library/VP`.
- Switching git branches with Unity open can delete or revert scenes underneath it; close Unity first.


## Code style
- Comments are minimal, following common C# industry practice:
  - One or two sentences of `/// <summary>` per class. No setup instructions, roadmap notes or rationale essays in code; those belong in the README or PR description.
  - Code should be self-documenting: clear names and small, well-named methods instead of `// Look`, `// Move` style section comments.
  - Inline comments only where the code is genuinely hard to follow or does something non-obvious (a workaround, a networking subtlety, a magic number).
- Namespaces mirror folders under `Assets/_Project/Scripts` (`ProjectAllUnits.<Folder>`).

## Owned assets (use these before writing or buying alternatives)
Paid, imported from the Asset Store into their own folders under `Assets/` (not `_Project`). Not committed to git (size and licence); re-import from Package Manager > My Assets on a fresh clone:
- **Dissonance Voice Chat**: all voice and radio.
- **NWH Vehicle Physics 2**: all driveable vehicles.
- **DOTween Pro**: tweens and UI motion.
- **Feel** (More Mountains): camera shake, impacts and other game feel.
- **Synty POLYGON** Police Station, City, City Characters and Town packs: environment and characters for prototyping. Before shipping: palette recolour, a signature shader/post look, and custom hero assets (officer, police car, dispatch desk, UI).

Free: ProBuilder (greyboxing), Cinemachine, AI Navigation (NPC pathing), Input System, Netcode for GameObjects, Multiplayer Play Mode, Mixamo animations (Humanoid rig), Facepunch.Steamworks (later).
