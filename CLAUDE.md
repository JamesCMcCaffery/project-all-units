# PROJECT ALL UNITS

Asymmetric co-op police game for PC (Steam): one Dispatcher, the rest Officers. Unity 6 LTS (6000.0.85f2), URP, Netcode for GameObjects (host-client; Unity Transport now, Unity Relay for early online tests, Facepunch.Steamworks lobbies later), Dissonance for voice and radio. README.md has the stack, roadmap, setup and the current step's editor checklist; keep it up to date when a step changes.

## Working with James
James is new to Unity (ex-Roblox, knows C#, uses Rider). Keep his part as small as possible: if you can do something yourself, do it.
- **Do it yourself:** C#, editor tooling, project settings, packages (`Packages/manifest.json`), README/CLAUDE.md, and git (branch, commit, push, PR).
- **Scene and prefab setup:** write an idempotent editor menu tool under *Tools > Project All Units* (`Scripts/Editor`, see `FirstLookSetup.cs`) that builds and wires everything, so James clicks one menu item instead of following a checklist. Only hand-edit `.unity` / `.prefab` / `.asset` YAML for trivial changes.
- **Run it yourself when the editor is closed:** `"<Unity Hub>/Editor/6000.0.85f2/Editor/Unity.exe" -batchmode -quit -projectPath . -executeMethod <Class.Method> -logFile -`. Batch mode can't open the project while the editor has it open; then ask James to click the menu item instead.
- **Check compile errors yourself** in `%LOCALAPPDATA%\Unity\Editor\Editor.log` rather than asking James to look at the Console.
- **Only hand James what needs him:** pressing Play and judging how it feels or looks, Asset Store downloads/imports, and anything needing the open editor. Give the fewest steps, exact menu paths, no explanation unless he asks. A Roblox analogy is fine when he's learning a concept.

## Roadmap (one-call vertical slice)
1. Foundations: greybox street, shared first-person player, sitting, Dispatcher/Officer spawning over the network. **Done.**
1.5. First look: Synty art in the dispatch room and one street corner, characters, lighting and post. **Current.**
2. Dispatcher desk MVP: incoming call, plate/licence lookup, map with units, assign + waypoint, one CCTV and one bodycam feed.
3. Officer field MVP: push-to-talk radio, driveable car, fleeing suspect NPC, taser, handcuffs.
4. The full call start to finish (stolen vehicle), playtest, plan the next slice.

Keep work scoped to the current step unless asked.

## Art style
Stylised, colourful but grounded: between clean low-poly and semi-realistic. Reference: *RV There Yet?* Not realistic, not cartoony-crazy. James finds greyboxes hard to read, so get real models in early.

Synty POLYGON packs are for prototyping. Before the Steam page: palette recolour, a signature shader/post look, and custom hero assets (officer, police car, dispatch desk and screens, UI).

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
