# 3D ART ASSET SOURCES

The final game uses real external 3D assets, not cube/capsule placeholders.

## Primary pack — Quaternius Toon Shooter Game Kit
- Official source: https://quaternius.com/packs/toonshootergamekit.html
- 74 models; FBX, OBJ, Blend and glTF.
- Includes animated characters, enemies, guns and environment props.
- License: Quaternius asset license / CC0-era pack terms; verify the pack license when downloading.

## Mirror — Poly Pizza
- https://poly.pizza/bundle/Toon-Shooter-Game-Kit-qraiSXoAru
- 73 listed models including Character Soldier, Character Enemy, shotgun, rifles, crates, containers and environment props.
- Listed as CC0.

## Secondary weapon/prop source — Kenney Blaster Kit
- Official source: https://kenney.nl/assets/blaster-kit
- 40 models, including weapons, targets, crates and smoke-related props.
- License: CC0.

## Unity importer
The project already declares `com.unity.cloud.gltfast` 6.17.0 in `Packages/manifest.json` for proper glTF/GLB importing.

## Important
Binary model files are not fabricated or represented by tiny placeholder GLBs. They must be imported from the verified source packs. The repository automation can track and validate those files, but this GitHub connector cannot directly upload arbitrary external binary ZIP/GLB bytes.