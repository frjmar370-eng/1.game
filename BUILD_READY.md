# Shotgun 3D — Unity Build Ready

## Target
- Unity: 2022.3.62f2
- Platform: Android
- Architecture: ARM64
- Graphics API: OpenGLES3
- Orientation: Landscape
- Minimum SDK: 23
- Target SDK: 35
- Scripting backend: IL2CPP
- Package: com.ammar.game
- Company: Frjmar

## Project structure
- Assets/Art/Environment/ — authored city OBJ/MTL
- Assets/Art/Characters/ — authored character OBJ/MTL
- Assets/Art/Weapons/ — authored shotgun OBJ/MTL
- Assets/Resources/ — runtime car asset
- Assets/Scripts/ — gameplay, camera, mobile controls, health, weapon and traffic
- Assets/Editor/ — automatic scene creation, validation and Android build commands
- Assets/Scenes/ — generated playable Main scene

## One-time Unity setup
1. Open the repository with Unity 2022.3.62f2.
2. Wait for package/import compilation to finish.
3. Run `Shotgun 3D > FINAL PREPARE > Validate Project`.
4. Run `Shotgun 3D > FINAL PREPARE > Prepare Android Build`.
5. If the scene does not exist, run `Shotgun 3D > Rebuild Main Scene`.
6. Open `Assets/Scenes/Main.unity`.
7. Test Play mode on desktop first.
8. Run `Shotgun 3D > FINAL BUILD > Build Android APK`.

## Important
The repository contains authored mesh assets rather than runtime-generated city geometry. Unity imports the OBJ/MTL files and the editor scene builder adds collision, player controller, camera, weapon, health, lighting and mobile HUD.

## Build output
The editor build command writes:
`Builds/Android/Shotgun3D.apk`

## Current scope
Playable foundation includes a real 3D city, 3D player, third-person movement/camera, mobile movement/look/fire controls, shotgun raycast damage, health, traffic cars and HUD. Advanced production systems such as character rig/animation, missions, police AI, audio, save system, advanced vehicle physics and final AAA-quality art still require additional authored assets and implementation.

## No false build claim
An APK is only considered verified after Unity actually completes the BuildPipeline operation on a machine with the required Android SDK/NDK/JDK and Unity license installed. The repository scripts prepare and automate the build; they do not bypass Unity's local build requirements.
