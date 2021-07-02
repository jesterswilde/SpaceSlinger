using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : SerializedMonoBehaviour
{
    protected Player player;
    [SerializeField, FoldoutGroup("Base")]
    bool isKinematic = false;
    [SerializeField, FoldoutGroup("Base")]
    bool useGravity = true;
    [SerializeField, FoldoutGroup("Base")]
    public PlayerMode Mode {protected set; get;}
    internal virtual void End()
    {
        Player.PEvent -= EventHappened;
    }

    internal virtual void Begin(Player _player, bool isChildMotion = false)
    {
        player = _player;
        if (!isChildMotion)
        {
            player.Rigid.isKinematic = isKinematic;
            player.Rigid.useGravity = useGravity;
            Player.PEvent += EventHappened;
        }
    }

    internal virtual void Run(float fixedDeltaTime)
    {
        //Debug.Log($"Running {Mode}");
    }

    protected virtual void EventHappened(PlayerEvents e)
    {
        player.ChangeMode(Response(e));
    }
    protected virtual PlayerMode Response(PlayerEvents e) => e switch
    {
        PlayerEvents.Liftoff => PlayerMode.InAir,
        PlayerEvents.Landed => PlayerMode.Run,
        PlayerEvents.Disconnected => player.IsOnGround switch
        {
            true => PlayerMode.Run,
            false => PlayerMode.InAir
        },
        PlayerEvents.LoseControl => PlayerMode.NoControl,
        _ => PlayerMode.Unchanged
    };

    internal virtual void GetInputs()
    {
    }
}
