using UnityEngine;

public class LaunchedObj : MonoBehaviour
{
    Oscillator osc;
    [SerializeField]
    Oscillator.MotionInfo neutralMotion;
    [SerializeField]
    Oscillator.MotionInfo primedMotion;
    [SerializeField]
    float launchVelocity = 10f;
    public Interactable Interactable { get; private set; }
    public float LaunchVelocity {
        get => launchVelocity;
        set => launchVelocity = value;
    }
    Rigidbody rigid; 
    public void Deactivate()
    {
        osc.UpdateMotion(neutralMotion);
    }
    public void PrimeForLaunch()
    {
        osc.UpdateMotion(primedMotion);
    }
    public void Launch()
    {
        rigid.transform.SetParent(null);
        rigid.useGravity = true;
        rigid.isKinematic = false;
        rigid.velocity = transform.up * LaunchVelocity;
        Callback.CreateFixed(() => rigid.isKinematic = false, 0.5f);
        Destroy(osc);
        Destroy(gameObject);
    }
    private void Awake()
    {
        osc = GetComponentInChildren<Oscillator>();
        osc.UpdateMotion(neutralMotion);
        rigid = GetComponentInChildren<Rigidbody>();
        Interactable = GetComponentInChildren<Interactable>();
    }
    private void Start()
    {
        Interactable.OnConnect += () => CameraController.LoadSystem(CameraController.AttachedCam);
        //Interactable.OnDisconnect += () => CameraController.UnloadSystem(CameraController.AttachedCam);
    }
}
