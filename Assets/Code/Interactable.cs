using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Interactable : SerializedMonoBehaviour
{
    public event Action OnConnect;
    public event Action OnDisconnect;
    [SerializeField]
    Functionality functionalities;
    public Functionality Functionalities => functionalities;
    public void Connected(){
        Debug.Log($"Connect to {name}");
        OnConnect?.Invoke(); 
    }

    public void Disconnected() => OnDisconnect?.Invoke();
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
