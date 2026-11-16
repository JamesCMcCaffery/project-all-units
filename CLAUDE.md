# PROJECT ALL UNITS

Unity 6 LTS, URP, Netcode for GameObjects. See README.md for stack, roadmap and setup.

## Code style
- Comments are minimal, following common C# industry practice:
  - One or two sentences of `/// <summary>` per class. No setup instructions, roadmap notes or rationale essays in code; those belong in the README or PR description.
  - Code should be self-documenting: clear names and small, well-named methods instead of `// Look`, `// Move` style section comments.
  - Inline comments only where the code is genuinely hard to follow or does something non-obvious (a workaround, a networking subtlety, a magic number).
- Namespaces mirror folders under `Assets/_Project/Scripts` (`ProjectAllUnits.<Folder>`).

## Owned assets (use these before writing or buying alternatives)
Paid, imported from the Asset Store into their own folders under `Assets/` (not `_Project`):
- **Dissonance Voice Chat**: all voice and radio.
- **NWH Vehicle Physics 2**: all driveable vehicles.
- **DOTween Pro**: tweens and UI motion.
- **Feel** (More Mountains): camera shake, impacts and other game feel.
- **Synty POLYGON** Police Station, City, City Characters and Town packs: environment and characters for prototyping. Before shipping: palette recolour, a signature shader/post look, and custom hero assets (officer, police car, dispatch desk, UI).

Free: ProBuilder (greyboxing), Cinemachine, AI Navigation (NPC pathing), Input System, Netcode for GameObjects, Multiplayer Play Mode, Mixamo animations (Humanoid rig), Facepunch.Steamworks (later).
