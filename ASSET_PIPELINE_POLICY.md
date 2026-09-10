# Asset pipeline policy

- Bulk asset downloading through GitHub Actions is disabled.
- Do not download or commit large external packs on every push.
- Unity runtime code must reference validated prefabs/assets, not guessed paths to raw files outside `Assets/Resources`.
- Large binary assets are configured for Git LFS through `.gitattributes`; existing history is not rewritten automatically by CI.
- Prefer a small, curated set of production assets and validate the Unity import/build before adding more.
