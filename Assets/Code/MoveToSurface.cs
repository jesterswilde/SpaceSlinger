using UnityEngine;

public class MoveToSurface : MonoBehaviour
{
    [SerializeField]
    LayerMask mask;
    [SerializeField]
    float offset;
    void SetPosAndDir()
    {
        Ray ray = new Ray(transform.position, transform.up * -1);
        if(Physics.Raycast(ray, out RaycastHit hit, 50, mask)){
            transform.up = hit.normal;
            transform.position = hit.point + transform.up * offset;
        }
    }

    private void Start()
    {
        SetPosAndDir();
    }
}
