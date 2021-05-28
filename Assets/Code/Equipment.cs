using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : SerializedMonoBehaviour
{
    [SerializeField, Range(1, 2)]
    int slots;
    public int Slots => slots;
    [SerializeField]
    protected float hoverRange = 50f;
    public float HoverRange => hoverRange;
    [SerializeField]
    public PlayerMode AssociatedMode { get; private set; }

    public virtual void Activate() { }
    public virtual void ActivateSecondary() { }
    public virtual void Equip() { }
    public virtual void Unequip() { }
    protected virtual void EventHappened(PlayerEvents e) { }
    protected virtual void Start()
    {
        Player.PEvent += EventHappened;
    }
}
