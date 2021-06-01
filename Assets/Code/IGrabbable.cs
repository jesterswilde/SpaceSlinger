using UnityEngine;
public interface IGrabbable
{
    public string name { get; }
    public Transform transform { get; }
    public float UsableDistance { get; }
    public void Connect();
    public void Disconnect();
    public void Move(float deltaTime, Vector3 moveDir);

}