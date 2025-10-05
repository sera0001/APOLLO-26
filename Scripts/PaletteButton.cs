
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PaletteButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public HabitatManager manager;
    public int moduleIndex = 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (manager == null) manager = FindObjectOfType<HabitatManager>();
        manager.SetModule(moduleIndex);
    }
    public void OnPointerUp(PointerEventData eventData) {}
}
