using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverManager : MonoBehaviour
{
    static HoverManager t;
    float rayDist => Player.HoverDist;
    Delta<Hoverable> hovering;
    public static Transform Target => t.hovering.Value?.transform;
    [SerializeField]
    LayerMask hoverMask;
    void CheckHover()
    {
        var ray = CameraController.Cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.SphereCast(ray, 1f, out hit, rayDist, hoverMask))
            hovering.Update(hit.collider.gameObject.GetComponentInParent<Hoverable>());
        else
            hovering.Update(null);
        if (hovering.Changed)
        {
            hovering.Previous?.HoverExit();
            hovering.Value?.HoverEnter();
        }
        hovering.Value?.HoverOver();
    }
    private void Update()
    {
        CheckHover();
    }
    private void Awake()
    {
        t = this;
        hovering = new Delta<Hoverable>() { Previous = null, Value = null };
    }
}
