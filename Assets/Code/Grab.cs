using System.Linq;
using UnityEngine;

public class Grab : Equipment
{
    [SerializeField]
    LayerMask mask;
    [SerializeField]
    Interactable.Functionality interactsWith;
    public IGrabbable GrabbedThing { get; private set; }
    GrabMotion motion;
    bool isConnected => GrabbedThing != null;

    void Fire()
    {
        Debug.Log("Firing");
        var ray = new Ray(Player.ProjectilePosition, Player.ProjectileDirection);
        if(Physics.Raycast(ray, out RaycastHit hit, hoverRange, mask)){
            var inter = hit.collider.gameObject.GetComponentInParent<Interactable>();
            var grabbable = hit.collider.gameObject.GetComponentsInParent<Interactable>()
                .Where(inter => interactsWith.Contains(inter.Functionalities) && inter is IGrabbable).FirstOrDefault() as IGrabbable;
            Debug.Log($"Hit {inter?.name ?? "Nothing"}");
            if(grabbable != null)
            {
                Debug.Log($"Hit {grabbable.name}");
                var dist = Vector3.Distance(Player.Transform.position, grabbable.transform.position);
                if(dist < grabbable.UsableDistance)
                {
                    Debug.Log("In Range");
                    GrabbedThing = grabbable;
                    motion.HoldOnto(this);
                    Player.EquipmentConnected(this, motion);
                }
            }
        }
    }
    void Disconnect()
    {
        GrabbedThing.Disconnect();
        GrabbedThing = null;
        Player.EventHappened(PlayerEvents.Disconnected);
    }
    public override void Activate()
    {
        if (!isConnected)
            Fire();
        else
            Disconnect();
    }
    private void Awake()
    {
        motion = GetComponentInChildren<GrabMotion>();
    }
}
