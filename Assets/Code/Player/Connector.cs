using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Connector : Equipment
{
    [SerializeField]
    LineRenderer linePrefab;
    [SerializeField]
    Hook hookPrefab;
    [SerializeField]
    float RateOfFire;
    [SerializeField]
    float maxLength;
    [SerializeField]
    Interactable.Functionality canAttachTo = Interactable.Functionality.Swingable;
    public float MaxLength => maxLength;
    bool canFire = true;
    Hook activeHook;
    LineRenderer line;
    public Transform From { get; private set; }
    [SerializeField]
    Transform to;
    public Transform To {
        get => to;
        set => to = value;
    }
    Interactable target;
    public override Interactable ConnectedTo => target;
    public override bool IsConnected => target != null;
    [SerializeField]
    Material hookedLineMat;
    [SerializeField]
    Material travelingLineMat;
    [SerializeField]
    int numMiddlePoints;
    ConnectorMotion motion;
    Interactable connectedInter;

    void Fire()
    {
        //GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = Player.ProjectilePosition;
        Player.EventHappened(PlayerEvents.Disconnected);
        activeHook = Instantiate(hookPrefab);
        activeHook.transform.position = Player.ProjectilePosition;
        activeHook.transform.forward = Player.ProjectileDirection;
        activeHook.Setup(Player.ProjectileDirection, CanAttachTo, AttachedTo);
        canFire = false;
        Callback.Create(() => canFire = true, RateOfFire);
        From = Player.Transform;
        To = activeHook.transform;
        line = Instantiate(linePrefab);
        line.material = travelingLineMat;
    }
    bool CanAttachTo(GameObject go)
    {
        var comp = go.GetComponent<Interactable>();
        return comp != null && (comp.Functionalities & canAttachTo) > 0; 
    }
    void AttachedTo(GameObject go)
    {
        target = go.GetComponent<Interactable>();
        From = Player.Transform;
        To = activeHook.transform;
        line.material = hookedLineMat;
        motion.Connected(this);
        Player.EquipmentConnected(motion);

        connectedInter = go.GetComponentInChildren<Interactable>();
        connectedInter?.Connected();
    }
    public void Disconnect()
    {
        if(line != null)
            Destroy(line.gameObject);
        if (activeHook != null)
        {
            activeHook.IsActive = false;
            activeHook = null;
        }
        target = null;
        From = null;
        To = null;
        connectedInter?.Disconnected();
        connectedInter = null;
    }
    protected override void EventHappened(PlayerEvents e)
    {
        if (e == PlayerEvents.Disconnected)
            Disconnect();
    }
    public override void Activate()
    {
        if (canFire)
            Fire();
    }
    public override void ActivateSecondary() => Player.EventHappened(PlayerEvents.Disconnected);
    void UpdateLineAndHook()
    {
        if (!IsConnected && activeHook != null && (Vector3.Distance(From.position, To.position) > MaxLength))
            Disconnect();
        if(To !=  null && From != null)
            line?.SetPositions(new Vector3[2] { From.position, To.position });
    }
    private void Update()
    {
        UpdateLineAndHook();
    }
    private void Awake()
    {
        motion = GetComponentInChildren<ConnectorMotion>();
    }
}
