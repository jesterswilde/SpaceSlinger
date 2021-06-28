using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gear : MonoBehaviour
{
    static Gear t;
    public static Gear T => t; 
    [SerializeField]
    List<Equipment> allEquipment;
    [SerializeField]
    int _equipIndex = -1;
    int equipIndex
    {
        get => _equipIndex;
        set
        {
            Equipped?.Unequip();
            while (value < 0)
                value += allEquipment.Count;
            _equipIndex = value % allEquipment.Count;
            Equipped?.Equip();
        }
    }

    public Equipment Equipped => allEquipment[equipIndex];
    public Equipment ConnectedEquipment => (Equipped != null && Equipped.IsConnected) ? Equipped : null;
    public void SelectNext() => equipIndex++;
    public void SelectPrev() => equipIndex--;
    public void SelectIndex(int index) => equipIndex = index;

    public void ActivateEquipped(){
        Debug.Log("Activating");
        Equipped?.Activate(); 
    }
    public void DeactivateEquipped() => Equipped?.Deactivate();
    public void ActivateEquippedSecondary() => Equipped?.ActivateSecondary();
    public void DeactivateEquippedSecondary() => Equipped?.DeactivateSecondary();
    private void Awake()
    {
        allEquipment = GetComponentsInChildren<Equipment>().ToList();
        t = this;
    }
    private void Start()
    {
        equipIndex = 0;
    }
}
