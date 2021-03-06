using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : SerializedMonoBehaviour
{
    [SerializeField]
    protected float hoverRange = 50f;
    public virtual bool IsConnected => false;
    public virtual Interactable ConnectedTo => null;
    public float HoverRange => hoverRange;
    public virtual bool ShouldHighlight(Hoverable hoverable) => Vector3.Distance(hoverable.transform.position, Player.Transform.position) < hoverRange;
    public virtual void Activate() { }
    public virtual void ActivateSecondary() { }
    public virtual void Deactivate() { }
    public virtual void DeactivateSecondary() { }
    public virtual void Equip() { }
    public virtual void Unequip() { }
    protected virtual void EventHappened(PlayerEvents e) { }
    protected virtual void Start()
    {
        Player.PEvent += EventHappened;
    }
}
[Serializable]
class EquipDesc
{
    public string Description;
}
