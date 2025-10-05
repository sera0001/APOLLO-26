
using UnityEngine;
using UnityEngine.UI;

public class MetricsUI : MonoBehaviour
{
    public MetricsCalculator metrics;
    public ConstraintChecker checker;
    public Text areaText;
    public Text volumeText;
    public Text statusText;

    void Update()
    {
        if (!metrics || !checker) return;
        areaText.text = $"Area: {metrics.totalArea:0.0} m²";
        volumeText.text = $"Volume: {metrics.totalVolume:0.0} m³";
        var res = checker.ValidateAll();
        statusText.text = $"OK: {res.ok}   Issues: {res.bad}";
    }
}
