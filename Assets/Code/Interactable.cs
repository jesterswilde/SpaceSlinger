using UnityEngine;

public class Interactable : MonoBehaviour 
{

    [SerializeField]
    Functionality functionalities;
    public Functionality Functionalities => functionalities;
    public void Collected()
    {

    }
    [System.Flags]
    public enum Functionality
    {
        Swingable = 1 << 0,
        Grabbable = 1 << 1
    }
}
 public static class InteractableExtensions
{
    public static bool Contains(this Interactable.Functionality thisFunc, Interactable.Functionality other) => (thisFunc & other) > 0; 
}
