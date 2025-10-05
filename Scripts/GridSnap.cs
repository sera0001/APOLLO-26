
using UnityEngine;
public static class GridSnap
{
    public static Vector3 Snap(Vector3 p, float grid)
    {
        return new Vector3(
            Mathf.Round(p.x / grid) * grid,
            Mathf.Round(p.y / grid) * grid,
            Mathf.Round(p.z / grid) * grid
        );
    }
}
