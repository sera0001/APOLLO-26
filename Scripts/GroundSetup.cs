
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GroundSetup : MonoBehaviour
{
    public void SetAsGround()
    {
        var col = GetComponent<Collider>();
        if (!col) col = gameObject.AddComponent<MeshCollider>();
        int idx = LayerMask.NameToLayer("Ground");
#if UNITY_EDITOR
        if (idx == -1)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            for (int i = 8; i < layersProp.arraySize; i++)
            {
                SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
                if (sp.stringValue == "")
                {
                    sp.stringValue = "Ground";
                    tagManager.ApplyModifiedProperties();
                    break;
                }
            }
            idx = LayerMask.NameToLayer("Ground");
        }
#endif
        if (idx < 0) idx = 8;
        gameObject.layer = idx;
    }
}
