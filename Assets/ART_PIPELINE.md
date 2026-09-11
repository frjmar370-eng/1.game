# Shotgun 3D — Unity 2022.3 Art Pipeline

## Folder layout
- `Assets/Art/Environment/` — city meshes and environment materials.
- `Assets/Art/Characters/` — player/enemy meshes and actor materials.
- `Assets/Art/Weapons/` — weapon meshes and materials.
- `Assets/Resources/` — runtime-loadable compatibility assets only.
- `Assets/Scripts/` — runtime gameplay systems.
- `Assets/Editor/` — editor-only scene/build preparation.
- `Assets/Scenes/` — playable scenes.

## Unity import requirements
For authored OBJ assets:
1. Scale Factor = 1.
2. Mesh Compression = Off for source art that is used for collision or precise placement.
3. Generate Colliders = Off on the imported model.
4. Use a dedicated MeshCollider on the city root for world collision.
5. Keep materials in the same art folder as the OBJ/MTL.
6. Use LOD groups on repeated environment props when the project grows.
7. Keep mobile texture sizes and polygon counts controlled; prefer baked lighting for static scenery.

## Scene construction
`Assets/Editor/FirstPlayableScene.cs` is the editor-side scene constructor. It creates the playable city scene from authored assets, adds world collision, player CharacterController, third-person camera, directional lighting, fog and mobile HUD.

## Build target
- Unity 2022.3 LTS
- Android
- ARM64
- OpenGLES3
- Landscape
- Min SDK 23
- Target SDK 35
- IL2CPP
- Package: `com.ammar.game`

## Important
The city and actors are authored mesh assets. Do not replace them with runtime-generated primitive geometry when adding detail. Add new OBJ/FBX assets under the appropriate `Assets/Art/*` folder and let Unity import them.
