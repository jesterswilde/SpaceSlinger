using UnityEngine;

public class InAirMotion : PlayerMotion
{
    [SerializeField]
    float fallingControl;
    [SerializeField]
    float lerpVelocity = 1f;
    internal override void Begin(Player _player, bool isChildMotion = false)
    {
        base.Begin(_player);
        if(player.Rigid.velocity.magnitude < player.Velocity.magnitude)
            player.Rigid.velocity = player.Velocity;
    }
    internal override void GetInputs()
    {
        base.GetInputs();
        if (Input.GetKeyDown(KeyCode.Z))
            transform.up = Orientation.Up;
    }
    internal override void Run(float deltaTime)
    {
        base.Run(deltaTime);
        var forward = Input.GetAxisRaw("Vertical") * Orientation.Forward;
        var right = Input.GetAxisRaw("Horizontal") * Orientation.Right;
        player.Rigid.velocity += (forward + right).normalized * fallingControl * deltaTime;
        if(player.Velocity != Vector3.zero)
        {
            var lerpTo = Quaternion.LookRotation(player.Velocity, Orientation.Up);
            transform.rotation = Quaternion.Lerp(transform.rotation, lerpTo, lerpVelocity * deltaTime); 
        }
    }
}
