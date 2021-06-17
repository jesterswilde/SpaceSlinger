using System;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField]
    LayerMask mask;
    [SerializeField]
    float speed = 1f;
    public bool IsActive { get; set; } = true;
    public Rigidbody Rigid { get; private set; }
    Func<GameObject, bool> ShouldHook;
    Action<GameObject> DidHookOntoThing;
    bool isHooked = false;

    public void Setup(Vector3 initialForce, Func<GameObject, bool> hookCheck, Action<GameObject> didhook)
    {
        Rigid.AddForce(initialForce * speed, ForceMode.VelocityChange);
        ShouldHook = hookCheck;
        DidHookOntoThing = didhook;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (IsActive && !isHooked && (ShouldHook?.Invoke(collision.gameObject) ?? false))
            HookOnto(collision.gameObject);
    }
    void HookOnto(GameObject hookedOnto)
    {
        Debug.Log($"Hooked onto {hookedOnto.name}");
        isHooked = true; 
        DidHookOntoThing?.Invoke(hookedOnto);
        Destroy(Rigid);
        transform.forward = hookedOnto.transform.position - transform.position;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 20))
            transform.position = hit.point; 
        transform.SetParent(hookedOnto.transform, true);
    }

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody>();
    }
}
