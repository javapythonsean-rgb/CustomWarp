# Changelog

## 1.1.1 (2026-07-17)

- Validate the complete warp table before changing any live KSP rate, so one
  invalid field can no longer leave a partially applied configuration.
- Reject non-finite, non-increasing, below-1x, and above-10,000,000x rates.
- Ignore an invalid saved config as a whole and show the exact validation
  error instead of partially loading it.
- Support both the left and right Alt keys for the Alt+F8 shortcut.
- Add a 24-check offline regression harness for rate-table validation.

## 1.1.0 (2026-07-17)

- Keep the stock warp selector labels synchronized with custom multipliers.
- Re-apply labels when the flight UI is rebuilt or the warp rate changes.
- Add complete SpaceDock, KSP-AVC, CKAN, license, and source packaging metadata.

## 1.0.0

- First public release.
- Fixed the warp-freeze bug.
