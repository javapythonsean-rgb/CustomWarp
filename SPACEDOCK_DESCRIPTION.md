# Custom Warp

**Tired of waiting through 100,000× when you want to skip a decade? Or wishing you had a step between 1,000× and 10,000×? Set your own on-rails time-warp rates — and change them without ever leaving the game.**

Custom Warp replaces Kerbal Space Program's eight fixed on-rails warp multipliers with values *you* pick. Open the editor in flight, type the numbers you want, hit **Apply & Save**, and warp away. Your rates stick between sessions and the stock time-warp indicator updates to show your real multipliers — no guessing which step you're on.

## Features

- 🕹️ **Live in-game editor** — no config files, no restart. Open it, edit, apply.
- ⏩ **Eight fully custom on-rails rates** — fill in whatever multipliers suit your playstyle.
- 🏷️ **Accurate warp readout** — the stock indicator is relabelled to match your rates.
- 💾 **Persistent** — your rates are saved and re-applied on every scene load.
- 🛟 **Safe** — stock altitude limits are left intact, so low-altitude warp still behaves.
- 🪶 **Featherweight** — a single ~10 KB plugin, **zero dependencies**.

## Install

Extract the download's `GameData` folder into your KSP install root (merge with the existing `GameData`). You should end up with:

`Kerbal Space Program/GameData/CustomWarp/Plugins/CustomWarp.dll`

Then launch, enter flight, and open the **Custom Warp** window from the button by the time-warp control.

## Notes

- Affects **on-rails** (high) warp only — physics warp is unchanged.
- Rates are stored in plain text at `GameData/CustomWarp/PluginData/warp.cfg`; delete it to reset to stock.

## Compatibility

- **KSP 1.12.x** (built and tested on 1.12.5)
- No dependencies
- Safe to add to or remove from existing saves

## License

[MIT](https://opensource.org/licenses/MIT) — do what you like, just keep the notice.
