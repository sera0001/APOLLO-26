
using UnityEngine;

public class MetricsCalculator : MonoBehaviour
{
    public float totalArea;
    public float totalVolume;

    void Update()
    {
        totalArea = 0f; totalVolume = 0f;
        var mods = GameObject.FindObjectsOfType<ModuleTag>();
        foreach (var m in mods)
        {
            var s = m.transform.localScale;
            totalArea += s.x * s.z;
            totalVolume += s.x * s.y * s.z;
        }
    }
}
