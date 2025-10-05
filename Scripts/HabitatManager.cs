
using UnityEngine;

public class HabitatManager : MonoBehaviour
{
    public Camera playerCamera;
    public float gridSize = 1f;
    public LayerMask groundMask;
    public Material previewMat;
    public Material placedMat;

    public bool buildMode = true;
    public HabitatModule currentModule = HabitatModule.Sleep;

    private GameObject previewObj;
    private Vector3 currentSize = new Vector3(2f, 2.5f, 2f);
    private float rotationY = 0f;

   void Awake()
{
    if (playerCamera == null)
    {
#if UNITY_2023_1_OR_NEWER
        playerCamera = Object.FindFirstObjectByType<Camera>();
#else
        playerCamera = Object.FindObjectOfType<Camera>();
#endif
    }
}

    private GameObject MakePrimitive(HabitatModule module, bool isPreview = false)
{
    // Create a basic cube (you can later swap to prefab per module)
    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
    obj.name = module.ToString();

    // Give it a default size
    obj.transform.localScale = new Vector3(2f, 2f, 2f);

    // Give it a light color when previewing
    var renderer = obj.GetComponent<Renderer>();
    if (renderer)
    {
        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        renderer.material.color = isPreview ? new Color(1f, 1f, 1f, 0.5f) : Color.white;
    }

    // Disable collider while previewing
    if (isPreview)
    {
        var col = obj.GetComponent<Collider>();
        if (col) col.enabled = false;
    }

    return obj;
}

    void Update()
    {
        if (!buildMode) return;

        if (previewObj == null) previewObj = MakePrimitive(currentModule, true);

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 300f, groundMask))
        {
            Vector3 p = GridSnap.Snap(hit.point, gridSize);
            p.y += currentSize.y * 0.5f;
            previewObj.transform.position = p;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            rotationY += 90f;
            previewObj.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        }
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float s = 1f + scroll * 0.1f;
            currentSize = Vector3.Max(new Vector3(0.5f,0.5f,0.5f), currentSize * s);
            previewObj.transform.localScale = currentSize;
        }
        if (Input.GetMouseButtonDown(0)) PlaceAtPreview();
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (Physics.Raycast(ray, out RaycastHit del, 300f))
            {
                if (del.collider != null && del.collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
                    Destroy(del.collider.gameObject);
            }
        }
    }

    void PlaceAtPreview()
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.transform.position = previewObj.transform.position;
        g.transform.rotation = previewObj.transform.rotation;
        g.transform.localScale = currentSize;
        g.AddComponent<ModuleTag>().module = currentModule;
        var mr = g.GetComponent<MeshRenderer>();
        if (mr) mr.sharedMaterial = placedMat;
    }

    public void SetModule(int idx)
    {
        currentModule = (HabitatModule)idx;
        if (previewObj) { GameObject.Destroy(previewObj); previewObj = null; }
    }

    public void SetBuildMode(bool on)
    {
        buildMode = on;
        if (!on && previewObj) { Destroy(previewObj); previewObj = null; }
    }
}
