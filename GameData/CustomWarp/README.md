# Custom Warp

Set your own **on-rails time-warp multipliers** in Kerbal Space Program — edited live, in-game, no config wrangling.

**KSP 1.12.x · No dependencies · ~10 KB**

---

## What it does

- Replaces the eight stock on-rails warp steps (`1× 5× 10× 50× 100× 1000× 10000× 100000×`) with values **you** choose.
- Edit them **live in flight** — no file editing, no restart.
- **Relabels the stock time-warp indicator** so it shows your real multipliers, not the stock ones.
- **Saves** your rates and re-applies them on every scene load.
- Leaves stock **altitude limits** untouched, so on-rails warp stays safe at low altitude.

Physics (sub-light) warp is **not** modified — this mod only touches high (on-rails) warp.

## Install

**Manual:** copy the `GameData` folder from the download into your KSP install root, merging with the existing `GameData`. When done you should have:

```
Kerbal Space Program/GameData/CustomWarp/Plugins/CustomWarp.dll
```

**CKAN:** once the mod is indexed, `ckan install CustomWarp`.

## Usage

1. Enter flight (or any scene with time warp).
2. Click the on-screen button that appears by the time-warp control (or press the toggle hotkey) to open **Custom Warp**.
3. Type a multiplier into any of the eight slots. Slot 0 is normally kept at `1×`.
4. **Apply & Save** — rates take effect immediately and persist across sessions.
5. **Restore stock rates** any time from the same window.

## Config

Your rates are stored as plain text, one multiplier per line:

```
GameData/CustomWarp/PluginData/warp.cfg
```

Delete that file to fall back to the stock rates.

## Compatibility

- KSP **1.12.x** (built and tested on 1.12.5).
- No dependencies.
- Safe to add to, or remove from, an existing save.

## License

Released under the [MIT License](LICENSE).

## Author

javap
