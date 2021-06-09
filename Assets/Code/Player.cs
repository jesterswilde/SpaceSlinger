using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : SerializedMonoBehaviour
{
    Inventory inventory;
    public static Inventory Inventory => T.inventory;
    public static Player T { get; private set; }
    List<Renderer> rends;
    public static Transform Transform => T.transform;
    public Rigidbody Rigid { get; private set; }
    public static event Action<PlayerEvents> PEvent; 
    [SerializeField]
    PlayerMode currentMode = PlayerMode.InAir;
    [ShowInInspector]
    PlayerMotion motion;
    public Delta<Vector3> Position;
    public Vector3 Velocity => Position.Diff / Time.fixedDeltaTime;
    [SerializeField]
    Detector groundDetector;
    [SerializeField]
    Dictionary<PlayerMode, PlayerMotion> motionDict = new Dictionary<PlayerMode, PlayerMotion>();
    [SerializeField]
    float hideDist;
    Delta<bool> isGrounded = new Delta<bool>() { Previous = false, Value = false };
    public bool IsOnGround => groundDetector.IsBlocked;

    public PVisuals Visuals { get; private set; }
    public Equipment ConnectedEquipment { get; private set; }
    [SerializeField]
    public Equipment PrimaryEquipment { get; private set; }
    [SerializeField]
    public Equipment SecondaryEquipment { get; private set; }
    public static List<Equipment> GetEquipment()
    {
        var equip = new List<Equipment>();
        if(T.PrimaryEquipment != null)
            equip.Add(T.PrimaryEquipment);
        if(T.SecondaryEquipment != null)
            equip.Add(T.SecondaryEquipment);
        return equip;
    }
    public static float HoverDist => T.PrimaryEquipment?.HoverRange ?? 0;
    public static Vector3 ProjectilePosition { get
        {
            return Vector3.Project(T.transform.position -  CameraController.Cam.transform.position, CameraController.Cam.transform.forward) + Transform.position;
        } }
    public static Vector3 ProjectileDirection { get {
            if (HoverManager.Target != null)
                return (HoverManager.Target.position - ProjectilePosition).normalized;
            var endTarget = CameraController.Cam.transform.position + CameraController.Cam.transform.forward * HoverDist;
            return (endTarget - ProjectilePosition).normalized;
        } }


    public void ChangeMode(PlayerMode mode)
    {
        if (mode == PlayerMode.Unchanged || motion?.Mode == mode)
            return;
        Debug.Log($"Chaning Mode To {mode} from {motion?.Mode.ToString() ?? "None"}");
        motion?.End();
        motion = motionDict[mode];
        motion?.Begin(this);
        currentMode = motion?.Mode ?? PlayerMode.Unchanged;
    }


    public static void EquipmentConnected(Equipment equip, PlayerMotion motion)
    {
        T.ConnectedEquipment = equip;
        EventHappened(PlayerEvents.Connected);
        T.motion?.End();
        T.motion = motion;
        motion.Begin(T);
        T.currentMode = motion.Mode;
    }
    public static void EventHappened(PlayerEvents e) =>  PEvent?.Invoke(e);

    void CheckForEvents()
    {
        if (isGrounded.Changed)
            EventHappened(isGrounded.Value ? PlayerEvents.Landed : PlayerEvents.Liftoff);
    }
    void HideSnork(float newDist, float oldDist)
    {
        rends.ForEach(rend => rend.enabled = newDist > hideDist);
    }
    void UseEquipment()
    {
        if (!Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetMouseButtonDown(0))
                PrimaryEquipment?.Activate();
            if (Input.GetMouseButtonDown(1))
                SecondaryEquipment?.Activate();
        }
        else { 
            if (Input.GetMouseButtonDown(0))
                PrimaryEquipment?.ActivateSecondary();
            if (Input.GetMouseButtonDown(1))
                SecondaryEquipment?.ActivateSecondary();
        }
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReset()
    {
        PEvent = null;
    }
    private void FixedUpdate()
    {
        motion?.Run(Time.fixedDeltaTime);
        CheckForEvents();
        isGrounded.Update(IsOnGround);
        Position.Update(transform.position);
    }

    private void Update()
    {
        UseEquipment();
        motion?.GetInputs();
    }
    private void Start()
    {
        CameraController.CamDist.OnChange += HideSnork;
    }
    private void Awake()
    {
        T = this;
        Rigid = GetComponent<Rigidbody>();
        inventory = GetComponentInChildren<Inventory>();
        Position = new Delta<Vector3>() { Value = transform.position, Previous = transform.position };
        rends = GetComponentsInChildren<Renderer>().ToList();
        ChangeMode(currentMode);
        PrimaryEquipment?.Equip();
        Visuals = GetComponent<PVisuals>();
    }
}

public enum PlayerMode
{
    Run,
    InAir,
    Swing,
    Unchanged,
    NoControl,
    HoldingOnto,
}
public enum PlayerEvents
{
    Landed,
    Liftoff,
    Connected,
    Disconnected,
    LoseControl
}
