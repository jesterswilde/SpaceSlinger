using System.Linq;
using UnityEngine;

public class Grab : Equipment
{
    [SerializeField]
    LayerMask mask;
    [SerializeField]
    Interactable.Functionality interactsWith;
    IGrabbable grabbedThing;
    public IGrabbable GrabbedThing => grabbedThing;

    void Fire()
    {
        var ray = new Ray(Player.ProjectilePosition, Player.ProjectileDirection);
        if(Physics.Raycast(ray, out RaycastHit hit, hoverRange, mask)){
            var grabbable = hit.collider.gameObject.GetComponentsInChildren<Interactable>()
                .Where(inter => interactsWith.Contains(inter.Functionalities) && inter is IGrabbable).FirstOrDefault() as IGrabbable;
            if(grabbable != null)
            {
                grabbedThing = grabbable;
                Player.EquipmentConnected(this);
            }
        }
    }    
}
