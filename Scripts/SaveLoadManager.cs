
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class ModuleDTO
{
    public string type;
    public float[] pos;
    public float[] rot;
    public float[] scale;
}

[System.Serializable]
public class LayoutDTO
{
    public List<ModuleDTO> modules = new List<ModuleDTO>();
}

public class SaveLoadManager : MonoBehaviour
{
    public string filename = "layout.json";

    public void Save()
    {
        var dto = new LayoutDTO();
        foreach (var tag in GameObject.FindObjectsOfType<ModuleTag>())
        {
            var t = tag.transform;
            dto.modules.Add(new ModuleDTO{
                type = tag.module.ToString(),
                pos = new float[]{ t.position.x, t.position.y, t.position.z },
                rot = new float[]{ t.eulerAngles.x, t.eulerAngles.y, t.eulerAngles.z },
                scale = new float[]{ t.localScale.x, t.localScale.y, t.localScale.z }
            });
        }
        var json = JsonUtility.ToJson(dto, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, filename), json);
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        if (!File.Exists(path)) return;
        var json = File.ReadAllText(path);
        var dto = JsonUtility.FromJson<LayoutDTO>(json);

        foreach (var tag in GameObject.FindObjectsOfType<ModuleTag>()) Destroy(tag.gameObject);

        foreach (var m in dto.modules)
        {
            HabitatModule mod = (HabitatModule)System.Enum.Parse(typeof(HabitatModule), m.type);
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = new Vector3(m.pos[0], m.pos[1], m.pos[2]);
            go.transform.eulerAngles = new Vector3(m.rot[0], m.rot[1], m.rot[2]);
            go.transform.localScale = new Vector3(m.scale[0], m.scale[1], m.scale[2]);
            go.AddComponent<ModuleTag>().module = mod;
        }
    }
}
