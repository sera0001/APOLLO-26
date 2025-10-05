using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BootstrapUI : MonoBehaviour
{
#if UNITY_EDITOR
    [CustomEditor(typeof(BootstrapUI))]
    public class BootstrapUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var self = (BootstrapUI)target;
            if (GUILayout.Button("Build Sidebar")) self.BuildSidebar();
            if (GUILayout.Button("Add Moon/Mars Toggle")) self.AddMoonMarsToggle();
            if (GUILayout.Button("Add Metrics HUD")) self.AddMetricsHUD();
            if (GUILayout.Button("Add Save/Load Buttons")) self.AddSaveLoad();
            if (GUILayout.Button("Add Mode Toggle (Build/Walk)")) self.AddModeToggle();
        }
    }
#endif

    [SerializeField] private HabitatManager manager;

    // --- Helpers to silence deprecation warnings in 2023+ ---
    private static T FindOne<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return Object.FindFirstObjectByType<T>();
#else
        return Object.FindObjectOfType<T>();
#endif
    }

    private Canvas EnsureCanvas()
    {
        if (FindOne<EventSystem>() == null)
        {
            _ = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        var c = FindOne<Canvas>();
        if (!c)
        {
            var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            c = go.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            var sc = go.GetComponent<CanvasScaler>();
            sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            sc.referenceResolution = new Vector2(1600, 900);
        }
        return c;
    }

    public void BuildSidebar()
    {
        var canvas = EnsureCanvas();
        var panel = new GameObject("Sidebar", typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        var img = panel.GetComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.35f);

        var rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.sizeDelta = new Vector2(240f, 0f);

        AddHeader(panel.transform, "Modules");
        string[] names = System.Enum.GetNames(typeof(HabitatModule));
        for (int i = 0; i < names.Length; i++)
        {
            var b = AddUIButton(panel.transform, names[i]);
            var brt = b.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0f, 1f);
            brt.anchorMax = new Vector2(0f, 1f);
            brt.pivot = new Vector2(0f, 1f);
            brt.anchoredPosition = new Vector2(16f, -60f - i * 56f);
            brt.sizeDelta = new Vector2(208f, 48f);

            var pb = b.gameObject.AddComponent<PaletteButton>();
            pb.manager = manager != null ? manager : FindOne<HabitatManager>();
            pb.moduleIndex = i;
        }
    }

    public void AddMoonMarsToggle()
    {
        var c = EnsureCanvas();
        var go = new GameObject("MoonMars", typeof(Toggle));
        go.transform.SetParent(c.transform, false);
        var t = go.GetComponent<Toggle>();

        var bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(go.transform, false);
        var bgImg = bg.GetComponent<Image>();
        bgImg.color = new Color(1f, 1f, 1f, 0.15f);
        t.targetGraphic = bgImg;
        t.isOn = true; // up = Moon

        var label = new GameObject("Label", typeof(Text)).GetComponent<Text>();
        label.transform.SetParent(go.transform, false);
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.alignment = TextAnchor.MiddleCenter;
        var lrt = label.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;
        label.text = "Moon";

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-20f, -20f);
        rt.sizeDelta = new Vector2(200f, 44f);

        var sw = FindOne<EnvironmentSwitcher>();
        t.onValueChanged.AddListener(on =>
        {
            label.text = on ? "Moon" : "Mars";
            if (sw) sw.SetMoon(on);
        });
    }

    public void AddMetricsHUD()
    {
        var c = EnsureCanvas();
        var panel = new GameObject("MetricsHUD", typeof(Image));
        panel.transform.SetParent(c.transform, false);
        var img = panel.GetComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.35f);

        var rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(1f, 0f);
        rt.anchoredPosition = new Vector2(-20f, 20f);
        rt.sizeDelta = new Vector2(260f, 120f);

        var area = AddUIText(panel.transform, "Area: 0 m²");
        var vol  = AddUIText(panel.transform, "Volume: 0 m³");
        var stat = AddUIText(panel.transform, "OK: 0 Issues: 0");

        var mcalc = FindOne<MetricsCalculator>();
        var chk   = FindOne<ConstraintChecker>();

        var comp = panel.AddComponent<MetricsUI>();
        comp.metrics = mcalc;
        comp.checker = chk;
        comp.areaText = area;
        comp.volumeText = vol;
        comp.statusText = stat;
    }

    public void AddSaveLoad()
    {
        var c = EnsureCanvas();
        var bar = new GameObject("SaveLoadBar", typeof(Image));
        bar.transform.SetParent(c.transform, false);
        var img = bar.GetComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.35f);

        var rt = bar.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -20f);
        rt.sizeDelta = new Vector2(320f, 48f);

        var saveBtn = AddUIButton(bar.transform, "Save");
        var loadBtn = AddUIButton(bar.transform, "Load");

        var srt = saveBtn.GetComponent<RectTransform>();
        srt.anchorMin = new Vector2(0f, 0f);
        srt.anchorMax = new Vector2(0.5f, 1f);
        srt.offsetMin = Vector2.zero; srt.offsetMax = Vector2.zero;

        var lrt = loadBtn.GetComponent<RectTransform>();
        lrt.anchorMin = new Vector2(0.5f, 0f);
        lrt.anchorMax = new Vector2(1f, 1f);
        lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;

        var sl = FindOne<SaveLoadManager>();
        if (sl != null)
        {
            saveBtn.onClick.AddListener(() => sl.Save());
            loadBtn.onClick.AddListener(() => sl.Load());
        }
    }

    public void AddModeToggle()
    {
        var c = EnsureCanvas();
        var go = new GameObject("ModeToggle", typeof(Toggle));
        go.transform.SetParent(c.transform, false);
        var t = go.GetComponent<Toggle>();

        var bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(go.transform, false);
        var bgImg = bg.GetComponent<Image>();
        bgImg.color = new Color(1f, 1f, 1f, 0.15f);
        t.targetGraphic = bgImg;
        t.isOn = true; // Build Mode by default

        var label = new GameObject("Label", typeof(Text)).GetComponent<Text>();
        label.transform.SetParent(go.transform, false);
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.alignment = TextAnchor.MiddleCenter;
        var lrt = label.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;
        label.text = "Build Mode";

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, 20f);
        rt.sizeDelta = new Vector2(200f, 44f);

        var hm  = manager != null ? manager : FindOne<HabitatManager>();
        var fps = FindOne<FPSController>();

        t.onValueChanged.AddListener(on =>
        {
            label.text = on ? "Build Mode" : "Walk Mode";
            if (hm)  hm.SetBuildMode(on);
            if (fps) fps.enabled = !on; // disable FPS in Build, enable in Walk
        });
    }

    // -------- UI helpers --------
    private Button AddUIButton(Transform parent, string label)
    {
        var go = new GameObject(label + "_Btn", typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        var img = go.GetComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.15f);

        var btn = go.GetComponent<Button>();

        var txt = new GameObject("Text", typeof(Text)).GetComponent<Text>();
        txt.transform.SetParent(go.transform, false);
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.resizeTextForBestFit = true; txt.resizeTextMinSize = 10; txt.resizeTextMaxSize = 28;
        txt.text = label;

        var tRT = txt.GetComponent<RectTransform>();
        tRT.anchorMin = Vector2.zero; tRT.anchorMax = Vector2.one;
        tRT.offsetMin = Vector2.zero; tRT.offsetMax = Vector2.zero;

        return btn;
    }

    private Text AddUIText(Transform parent, string text)
    {
        var t = new GameObject("Text", typeof(Text)).GetComponent<Text>();
        t.transform.SetParent(parent, false);
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t.alignment = TextAnchor.MiddleLeft;
        t.text = text;

        var rt = t.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.sizeDelta = new Vector2(240f, 28f);
        rt.anchoredPosition = new Vector2(10f, -10f - parent.childCount * 28f);
        return t;
    }

    private void AddHeader(Transform parent, string title)
    {
        var t = AddUIText(parent, title);
        t.fontSize = 22;
    }
}
