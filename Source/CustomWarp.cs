using UnityEngine;
using System.IO;
using System.Globalization;
using System.Reflection;

[assembly: AssemblyTitle("CustomWarp")]
[assembly: AssemblyDescription("Edit KSP's eight on-rails time-warp multipliers in flight.")]
[assembly: AssemblyVersion("1.1.0")]
[assembly: AssemblyFileVersion("1.1.0")]
[assembly: KSPAssembly("CustomWarp", 1, 1)]

namespace CustomWarp
{
    // Edit all 8 on-rails warp rates from an in-game window.
    // Toggle the editor with  Alt + F8.
    // Settings are saved to  GameData/CustomWarp/PluginData/warp.cfg  and
    // re-applied every time the Flight scene loads.
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class CustomWarp : MonoBehaviour
    {
        public KeyCode toggleKey = KeyCode.F8;

        // KSP stock defaults, used by the "Reset" button.
        private static readonly float[] StockRates =
            new float[] { 1f, 5f, 10f, 50f, 100f, 1000f, 10000f, 100000f };

        private const int SLOT_COUNT = 8;

        private bool showUI;
        private Rect windowRect = new Rect(140, 140, 360, 0);
        private string[] inputs = new string[SLOT_COUNT];
        private string configPath;
        private string status = "";
        private bool appliedFromConfig;     // retry until TimeWarp.fetch is ready

        // Always parse/format with '.' as the decimal point, regardless of the
        // user's Windows locale (this machine is a comma-decimal locale).
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        void Start()
        {
            configPath = Path.Combine(
                KSPUtil.ApplicationRootPath,
                "GameData/CustomWarp/PluginData/warp.cfg".Replace('/', Path.DirectorySeparatorChar));

            TryApplyConfig();
            RefreshInputsFromRates();

            // Re-assert our label text every time the warp rate changes, in case
            // KSP's UI rebuilds its cached strings from the original values.
            if (GameEvents.onTimeWarpRateChanged != null)
                GameEvents.onTimeWarpRateChanged.Add(OnRateChanged);
        }

        void OnDestroy()
        {
            if (GameEvents.onTimeWarpRateChanged != null)
                GameEvents.onTimeWarpRateChanged.Remove(OnRateChanged);
        }

        // NOTE: must never fire onTimeWarpRateChanged itself (infinite loop).
        void OnRateChanged()
        {
            UpdateWarpUiLabels(false);
        }

        void Update()
        {
            // If TimeWarp wasn't ready during Start, keep trying until it is.
            if (!appliedFromConfig) TryApplyConfig();

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(toggleKey))
            {
                showUI = !showUI;
                if (showUI) RefreshInputsFromRates();
            }
        }

        // --- top-of-screen "Edit Warp Rates" button (shows on hover) -----------
        private float showButtonUntil;
        private const float BtnW = 122f;
        private const float BtnH = 22f;

        void OnGUI()
        {
            DrawHoverButton();
            if (showUI)
                windowRect = GUILayout.Window(0x4357F1A, windowRect, DrawWindow, "CustomWarp - Edit Warp Rates");
        }

        // The stock "WARP" indicator + "MET" time readout sit in the TOP-LEFT
        // corner. We watch that corner; while the mouse is over it (or over our
        // button), we show an "Edit Warp Rates" button right below the readout.
        void DrawHoverButton()
        {
            if (Event.current == null) return;

            Rect hoverStrip = new Rect(0f, 0f, 430f, 48f);          // covers WARP row + MET row
            Rect btnRect = new Rect(8f, 52f, BtnW, BtnH);           // just beneath the readout

            Vector2 m = Event.current.mousePosition;
            if (hoverStrip.Contains(m) || btnRect.Contains(m))
                showButtonUntil = Time.unscaledTime + 0.8f;         // grace so it doesn't flicker

            if (Time.unscaledTime >= showButtonUntil) return;

            if (GUI.Button(btnRect, "Edit Warp Rates"))
            {
                showUI = !showUI;
                if (showUI) RefreshInputsFromRates();
            }
        }

        void DrawWindow(int id)
        {
            GUILayout.Label("Set the x-multiplier for each of the 8 on-rails warp slots:");
            GUILayout.Space(4);

            for (int i = 0; i < SLOT_COUNT; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Slot " + (i + 1) + ":", GUILayout.Width(60));
                inputs[i] = GUILayout.TextField(inputs[i] ?? "1", GUILayout.Width(180));
                GUILayout.Label(" x", GUILayout.Width(20));
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply")) ApplyFromInputs(true);
            if (GUILayout.Button("Reload from file"))
            {
                appliedFromConfig = false;
                TryApplyConfig();
                RefreshInputsFromRates();
                status = "Reloaded.";
            }
            if (GUILayout.Button("Reset to stock"))
            {
                for (int i = 0; i < SLOT_COUNT; i++) inputs[i] = StockRates[i].ToString(Inv);
                ApplyFromInputs(true);
                status = "Reset to stock defaults.";
            }
            GUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(status))
            {
                GUILayout.Space(4);
                GUILayout.Label(status);
            }

            GUILayout.Space(4);
            if (GUILayout.Button("Close")) showUI = false;

            GUI.DragWindow();
        }

        bool RatesReady()
        {
            return TimeWarp.fetch != null && TimeWarp.fetch.warpRates != null;
        }

        void RefreshInputsFromRates()
        {
            if (!RatesReady()) return;
            for (int i = 0; i < SLOT_COUNT && i < TimeWarp.fetch.warpRates.Length; i++)
                inputs[i] = TimeWarp.fetch.warpRates[i].ToString(Inv);
        }

        void ApplyFromInputs(bool saveAfter)
        {
            if (!RatesReady()) { status = "Warp system not ready yet."; return; }
            int applied = 0;
            for (int i = 0; i < SLOT_COUNT && i < TimeWarp.fetch.warpRates.Length; i++)
            {
                float v;
                if (float.TryParse(inputs[i], NumberStyles.Float, Inv, out v) && v >= 1f)
                {
                    TimeWarp.fetch.warpRates[i] = v;
                    applied++;
                }
            }
            status = "Applied " + applied + " rate(s).";
            if (saveAfter) SaveRatesToConfig();
            UpdateWarpUiLabels(true);
        }

        // The on-screen warp multiplier ("x5", "x1,000", ...) is NOT read live from
        // warpRates -- KSP.UI.UITimeWarpController caches it in a string[] built once
        // at Start(). So editing the rates changes the warp speed but the label keeps
        // showing the old number. Here we rewrite that cached array to match the new
        // rates (preserving KSP's prefix/suffix formatting) and fire the rate-changed
        // event so the controller repaints.
        void UpdateWarpUiLabels(bool fireEvent)
        {
            try
            {
                if (!RatesReady()) return;
                const System.Reflection.BindingFlags F =
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
                    | System.Reflection.BindingFlags.NonPublic;

                // Patch EVERY live controller instance (flight HUD, map view, ...);
                // FindObjectOfType (singular) can return the wrong one.
                UnityEngine.Object[] ctrls =
                    UnityEngine.Object.FindObjectsOfType(typeof(KSP.UI.UITimeWarpController));
                if (ctrls == null || ctrls.Length == 0)
                {
                    Debug.Log("[CustomWarp] no UITimeWarpController instances found");
                    return;
                }

                System.Type ct = typeof(KSP.UI.UITimeWarpController);
                System.Reflection.FieldInfo labelsF = ct.GetField("stateTextHigh", F);
                System.Reflection.FieldInfo tipsF = ct.GetField("tooltips", F);
                float[] rates = TimeWarp.fetch.warpRates;
                int patched = 0;

                foreach (UnityEngine.Object o in ctrls)
                {
                    string[] labels = (labelsF != null) ? labelsF.GetValue(o) as string[] : null;
                    System.Array tips = (tipsF != null) ? tipsF.GetValue(o) as System.Array : null;

                    for (int i = 0; i < rates.Length; i++)
                    {
                        // Resolve the localization TAG to display text ("x50")
                        // first, THEN swap the number in.
                        string disp = (labels != null && i < labels.Length) ? labels[i] : null;
                        try { if (disp != null) disp = KSP.Localization.Localizer.Format(disp); }
                        catch (System.Exception) { }
                        string newLabel = FormatLike(disp, rates[i]);

                        if (labels != null && i < labels.Length) labels[i] = newLabel;

                        if (tips != null && i < tips.Length)
                        {
                            object tc = tips.GetValue(i);
                            if (tc != null)
                            {
                                System.Reflection.FieldInfo tsf = tc.GetType().GetField("textString", F);
                                if (tsf != null) tsf.SetValue(tc, newLabel);
                            }
                        }
                    }
                    patched++;
                }

                if (fireEvent && GameEvents.onTimeWarpRateChanged != null)
                    GameEvents.onTimeWarpRateChanged.Fire();

                Debug.Log("[CustomWarp] patched " + patched + " warp UI controller(s)");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[CustomWarp] UpdateWarpUiLabels: " + e.Message);
            }
        }

        // Replace the numeric part of an existing label with the new rate, keeping
        // whatever prefix/suffix KSP used (e.g. "x" or "×").
        static string FormatLike(string sample, float rate)
        {
            string num = FormatRate(rate);
            // Unresolved tag or empty -> emit a clean literal instead of corrupting it.
            if (string.IsNullOrEmpty(sample) || sample[0] == '#') return "x" + num;

            int a = -1, b = -1;
            for (int i = 0; i < sample.Length; i++)
                if (char.IsDigit(sample[i])) { if (a < 0) a = i; b = i; }
            if (a < 0) return "x" + num;          // no digits in sample -> clean fallback

            string prefix = sample.Substring(0, a);
            string suffix = sample.Substring(b + 1);
            return prefix + num + suffix;
        }

        static string FormatRate(float r)
        {
            if (Mathf.Abs(r - Mathf.Round(r)) < 0.001f)
                return ((long)Mathf.Round(r)).ToString("#,##0", Inv);
            return r.ToString("#,##0.###", Inv);
        }

        // Reads warp.cfg into the live rates. Sets appliedFromConfig once it runs
        // against a ready TimeWarp (even if the file is absent -> nothing to do).
        void TryApplyConfig()
        {
            if (!RatesReady()) return;
            appliedFromConfig = true;

            if (!File.Exists(configPath)) return;
            try
            {
                string[] lines = File.ReadAllLines(configPath);
                for (int i = 0; i < lines.Length && i < SLOT_COUNT && i < TimeWarp.fetch.warpRates.Length; i++)
                {
                    float v;
                    if (float.TryParse(lines[i].Trim(), NumberStyles.Float, Inv, out v) && v >= 1f)
                        TimeWarp.fetch.warpRates[i] = v;
                }
                UpdateWarpUiLabels(false);   // keep the on-screen labels in sync
                                             // (the rate-changed hook also re-asserts
                                             // them once the UI exists)
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[CustomWarp] LoadConfig failed: " + e.Message);
            }
        }

        void SaveRatesToConfig()
        {
            if (!RatesReady()) return;
            try
            {
                string dir = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                using (StreamWriter w = new StreamWriter(configPath))
                {
                    for (int i = 0; i < SLOT_COUNT && i < TimeWarp.fetch.warpRates.Length; i++)
                        w.WriteLine(TimeWarp.fetch.warpRates[i].ToString(Inv));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[CustomWarp] SaveConfig failed: " + e.Message);
            }
        }
    }
}
