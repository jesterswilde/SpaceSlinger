using UnityEngine;

[RequireComponent(typeof(ColorChanger))]
public class PVisuals : MonoBehaviour
{
    ColorChanger changer;
    public void CooldownFrom(Color color, float duration)
    {
        changer.ChangeColor(color, false);
        Callback.Create(() => changer.ChangeToBaseColor(), duration);
    }
    private void Awake()
    {
        changer = GetComponent<ColorChanger>();
    }
}
