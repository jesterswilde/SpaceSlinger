using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class RunningMotion : PlayerMotion
{
    [SerializeField]
    float acceleration;
    [SerializeField]
    float drag;
    [SerializeField]
    float maxSpeed;
    [SerializeField]
    float maxJumpForce;
    [SerializeField]
    float jumpForcePerSec;
    [ShowInInspector]
    float curJumpForce;
    [SerializeField]
    float stoppedDrag = 1f;
    [SerializeField]
    float forwardInput;
    [SerializeField]
    float rightInput;
    bool isMoving => forwardInput != 0 || rightInput != 0;
    Delta<bool> isReadyToJump = new Delta<bool>();
    Vector3 velocity;
    internal override void Run(float deltaTime)
    {
        base.Run(deltaTime);
        HandleMotion(deltaTime);
    }
    internal override void GetInputs()
    {
        base.GetInputs();
        rightInput = Input.GetAxisRaw("Horizontal");
        forwardInput = Input.GetAxisRaw("Vertical");
        HandleJumping();
    }
    void HandleMotion(float deltaTime)
    {
        FaceForward();
        if (isMoving)
            velocity = (rightInput * Vector3.right + forwardInput * Vector3.forward).normalized * acceleration;
        else
            velocity = Vector3.zero;
        if (isReadyToJump.Value)
            curJumpForce = Mathf.Min(curJumpForce + deltaTime * jumpForcePerSec, maxJumpForce);
        player.transform.position += (velocity.x * Orientation.Right + velocity.z * Orientation.Forward) * deltaTime;
    }
    void FaceForward()
    {
        player.transform.rotation = Quaternion.LookRotation(Orientation.Forward, Orientation.Up);
        velocity = player.transform.forward * velocity.magnitude;
    }
    void HandleJumping()
    {
        isReadyToJump.Update(Input.GetKey(KeyCode.Space));
        if (isReadyToJump.Changed) {
            if (isReadyToJump.Value)
                curJumpForce = 0;
            else
                player.Rigid.AddForce(curJumpForce * Orientation.Up, ForceMode.Impulse);
        }
    }
    internal override void Begin(Player _player)
    {
        base.Begin(_player);
    }
}
