using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
public class Cog : Interactable, IGrabbable
{
    [SerializeField]
    float initialTurned = 0.5f;
    BasisVectorContraption contraption;
    [SerializeField]
    public Axis Controlling { get; private set; }

    [SerializeField]
    Delta<float> turnAmount = new Delta<float>();
    public Delta<float> TurnAmount => turnAmount;
    [SerializeField]
    float turnSpeed = 0.3f;
    [SerializeField]
    float maxRevolutions = 3;
    [SerializeField]
    List<Transform> spots;
    Transform chosenSpot;
    [SerializeField, FoldoutGroup("Debug")]
    Vector3 testDir;
    Vector3 upDir; 
    bool PlayerIsConnected => chosenSpot != null;

    [SerializeField]
    float usableDist = 5f;
    public float UsableDistance => usableDist;

    public void Connect()
    {
        chosenSpot = spots.MinBy(spot => Vector3.Distance(Player.Transform.position, spot.position));
        Player.Transform.position = chosenSpot.transform.position;
        Player.Transform.forward = chosenSpot.transform.forward;
        contraption.Connect();
    }

    public void Disconnect()
    {
        chosenSpot = null;
        contraption.Disconnect();
    }

    void LerpToAmount(float amount, float oldAmount = 0f)
    {
        transform.Rotate(upDir, TurnAmount.Diff * maxRevolutions * 360);
        if (PlayerIsConnected) {
            Player.Transform.position = chosenSpot.transform.position;
            Player.Transform.forward = chosenSpot.transform.forward;
        }
    }
    [Button, FoldoutGroup("Debug")]
    void TestMove()
    {
        Move(1f, testDir);
    }
    public void Move(float deltaTime, Vector3 moveDir)
    {
        if (moveDir == Vector3.zero)
            return;
        var newAmount = Mathf.Clamp(TurnAmount.Value + deltaTime * maxRevolutions * turnSpeed * moveDir.z, 0, 1);
        TurnAmount.Update(newAmount);
        contraption.NormalizeCogs(Controlling);
    }
    private void Awake()
    {
        TurnAmount.Update(initialTurned);
    }
    private void Start()
    {
        TurnAmount.OnChange += LerpToAmount;
        contraption = GetComponentInParent<BasisVectorContraption>();
        upDir = transform.up;
    }
    public enum Axis
    {
        X,
        Y,
        Z
    }
}
