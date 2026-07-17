# Custom Warp

Custom Warp lets you edit all eight of KSP's on-rails time-warp multipliers
from a small in-game window.

## Features

- Change any of the eight warp slots while in flight.
- Save custom rates to `GameData/CustomWarp/PluginData/warp.cfg`.
- Reapply saved rates whenever the flight scene loads.
- Keep the stock warp-selector labels synchronized with the real rates.
- Reload the config or reset every slot to stock from the same window.

Press **Alt+F8** in flight to open or close the editor. Enter multipliers using
a period as the decimal separator, then click **Apply**.

## Install

Copy `GameData/CustomWarp` from this ZIP into KSP's `GameData` folder. The
installed DLL should be at:

`Kerbal Space Program/GameData/CustomWarp/Plugins/CustomWarp.dll`

No dependencies are required. Custom Warp supports KSP 1.12.x.

## Notes

- Slot 1 must be at least 1x.
- KSP still enforces its normal altitude-based warp limits.
- Very high rates can reduce orbital precision. Rates above roughly
  10,000,000x are not recommended.
- Removing the mod restores stock behavior the next time KSP starts.

## Uninstall

Delete `GameData/CustomWarp`.

## Source and license

Source is included in the ZIP under `Source/` and published at
https://github.com/javapythonsean-rgb/CustomWarp. Custom Warp is released
under the MIT License; see `LICENSE.txt`.
