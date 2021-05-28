using UnityEngine;
public interface IGrabbable
{
    public void Connect();
    public void Disconnect();
    public void Move(Vector3 moveDir);

}