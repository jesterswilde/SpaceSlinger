using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ColorChanger))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    Color activeColor;
    ColorChanger colorChanger;

    //This is usually called externally by a detector with a Unity Event.
    public void SetAsActive()
    {
        ResetManager.SetActiveCheckpoint(this);
        colorChanger.ChangeColor(activeColor);
    }
    //This is called by the ResetManger. 
    public void SetAsInactive()
    {
        colorChanger.ChangeToBaseColor();
    }
    private void Awake()
    {
        colorChanger = GetComponent<ColorChanger>();
    }
}
