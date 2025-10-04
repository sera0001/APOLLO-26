
using UnityEngine;

public class EnvironmentSwitcher : MonoBehaviour
{
    public Transform ground;
    public Light mainLight;
    public bool moon = true;

    public Color moonGround = new Color(0.55f,0.55f,0.6f,1f);
    public Color marsGround = new Color(0.6f,0.3f,0.2f,1f);
    public Color moonLight = new Color(0.85f,0.9f,1f,1f);
    public Color marsLight = new Color(1f,0.85f,0.7f,1f);

    private Material moonMat, marsMat;

    void Start()
    {
        if (!ground) { var g = GameObject.Find("Ground"); if (g) ground = g.transform; }
        if (!mainLight) { mainLight = FindObjectOfType<Light>(); }

        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        moonMat = new Material(shader); moonMat.color = moonGround;
        marsMat = new Material(shader); marsMat.color = marsGround;
        Apply();
    }

    public void SetMoon(bool m) { moon = m; Apply(); }
    public void Toggle() { moon = !moon; Apply(); }

    void Apply()
    {
        if (ground)
        {
            var rends = ground.GetComponentsInChildren<Renderer>(true);
            foreach (var r in rends) r.sharedMaterial = moon ? moonMat : marsMat;
        }
        if (mainLight) mainLight.color = moon ? moonLight : marsLight;
        RenderSettings.fog = true;
        RenderSettings.fogColor = moon ? new Color(0.7f,0.75f,0.85f) : new Color(0.9f,0.7f,0.6f);
        RenderSettings.fogDensity = moon ? 0.004f : 0.006f;
    }
}
