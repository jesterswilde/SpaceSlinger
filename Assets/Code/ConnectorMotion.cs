using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class ConnectorMotion : PlayerMotion
{
    [SerializeField]
    float speedIncrease;
    [SerializeField]
    float lengthChangeRate;
    [SerializeField]
    float minLength = 1f;
    [SerializeField]
    float lerpSpeed = 1f;
    [SerializeField]
    Color onCooldownColor;
    [SerializeField]
    float cooldownDuration; 
    float maxLength => connector.MaxLength;
    [SerializeField]
    float connectionLength;
    Connector connector;
    bool startedBoosting = false;
    bool canBoost = true;
    bool isBoosting = false;
    [SerializeField]
    float boostDuration = 0.5f;
    [SerializeField]
    RunningMotion groundedMotion;

    Vector3 orbPos => connector.To.position;
    Vector3 playerPos
    {
        get => player.transform.position;
        set => player.transform.position = value;
    }
    internal void Connected(Connector _connector)
    {
        connector = _connector;
    }
    internal override void Begin(Player _player, bool isChildMotion = false)
    {
        base.Begin(_player);
        connectionLength = Mathf.Max(Vector3.Distance(playerPos, orbPos), minLength);
        groundedMotion.Begin(_player, true); 
    }
    
    internal override void Run(float deltaTime)
    {
        base.Run(deltaTime);
        HandleChangeLength(deltaTime);
        if (player.IsOnGround)
        {
            groundedMotion.Run(deltaTime);
            HandleGroundedRopeLength(deltaTime);
        }
        else
        {
            HandleSpeedChange(deltaTime);
            HandleSwing(deltaTime);
            LerpToOrientation(deltaTime);
        }
    }
    void HandleGroundedRopeLength(float deltaTime)
    {
        var dist = Vector3.Distance(orbPos, player.transform.position);
        Debug.Log($"Dist: {dist} | ConnLength; {connectionLength} | OrbPos: {orbPos}");
        if(dist > connectionLength)
        {
            var dir = (player.transform.position - orbPos).normalized;
            player.transform.position = orbPos + dir * connectionLength;
        }
    }
    protected override void EventHappened(PlayerEvents e)
    {
        if (e == PlayerEvents.Connected)
            connectionLength = Mathf.Max(Vector3.Distance(playerPos, orbPos), minLength);
        base.EventHappened(e);
    }
    protected override PlayerMode Response(PlayerEvents e)=> e switch {
            PlayerEvents.Liftoff => PlayerMode.Unchanged,
            PlayerEvents.Landed => PlayerMode.Unchanged,
            _ => base.Response(e)
        };
    void HandleSwing(float deltaTime)
    {
        var velocity = player.Velocity;
        player.Rigid.velocity = velocity;
        var dist = Vector3.Distance(playerPos, orbPos);
        if(dist > connectionLength)
        {
            var dir = (playerPos - orbPos).normalized;
            playerPos = orbPos + dir * connectionLength;
        }
        player.transform.rotation = Quaternion.LookRotation(player.transform.forward, Orientation.Up);
    }


    void LerpToOrientation(float deltaTime)
    {
        var lerpTo = Quaternion.LookRotation(transform.forward, Orientation.Up);
        transform.rotation = Quaternion.Lerp(transform.rotation, lerpTo, lerpSpeed * deltaTime); 
    }
    void HandleChangeLength(float deltaTime)
    {
        float change = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        change -= Input.GetKey(KeyCode.X) ? 1f : 0f;
        if (change == 0)
            return;
        connectionLength = Mathf.Clamp(connectionLength - (change * deltaTime * lengthChangeRate),minLength, maxLength);
    }
    void HandleSpeedChange(float deltaTime)
    {
        if (canBoost && startedBoosting)
        {
            isBoosting = true;
            canBoost = false;
            startedBoosting = false;
            Callback.Create(() => isBoosting = false, boostDuration);
            Callback.Create(()=> canBoost = true, cooldownDuration);
            player.Visuals.CooldownFrom(onCooldownColor, cooldownDuration);
        }
        if(isBoosting && Input.GetKey(KeyCode.W))
            playerPos += Orientation.Forward * speedIncrease * deltaTime;
    }
    internal override void GetInputs()
    {
        if (canBoost && Input.GetKeyDown(KeyCode.W))
            startedBoosting = true;
        if (player.IsOnGround)
            groundedMotion.GetInputs();
    }
}
