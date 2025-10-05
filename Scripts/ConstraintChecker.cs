
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RuleSet
{
    public string version;
    public Dictionary<string, float> per_crew_volume_m3;
    public Dictionary<string, float> fixed_min_volume_m3;
    public Dictionary<string, float> percent_of_total;
}

public class ConstraintChecker : MonoBehaviour
{
    public int crewSize = 4;
    public int missionDays = 90;
    public Color okColor = new Color(0.3f,0.9f,0.5f,0.7f);
    public Color badColor = new Color(1f,0.3f,0.3f,0.7f);
    private RuleSet rules;

    void Start()
    {
        var text = Resources.Load<TextAsset>("habitat_rules");
        if (text != null) rules = JsonUtility.FromJson<RuleSet>(text.text);
        else rules = new RuleSet();
    }

    public (int ok, int bad) ValidateAll()
    {
        int ok=0, bad=0;
        float totalVol = 0f;
        var mods = GameObject.FindObjectsOfType<ModuleTag>();
        foreach (var m in mods) totalVol += m.transform.localScale.x * m.transform.localScale.y * m.transform.localScale.z;
        foreach (var m in mods)
        {
            string key = m.module.ToString();
            float vol = m.transform.localScale.x * m.transform.localScale.y * m.transform.localScale.z;
            bool good = IsOkay(key, vol, totalVol);
            var r = m.GetComponent<Renderer>(); if (r) { var mat = r.material; mat.color = good ? okColor : badColor; }
            if (good) ok++; else bad++;
        }
        return (ok,bad);
    }

    bool IsOkay(string key, float vol, float totalVol)
    {
        if (rules == null) return true;
        if (rules.per_crew_volume_m3 != null && rules.per_crew_volume_m3.ContainsKey(key))
            if (vol < rules.per_crew_volume_m3[key] * crewSize) return false;
        if (rules.fixed_min_volume_m3 != null && rules.fixed_min_volume_m3.ContainsKey(key))
            if (vol < rules.fixed_min_volume_m3[key]) return false;
        if (rules.percent_of_total != null && rules.percent_of_total.ContainsKey(key))
            if (vol < rules.percent_of_total[key] * totalVol) return false;
        return true;
    }
}
