using System.Collections;
using UnityEngine;

namespace Drawing
{
    public class PlatformGrid : MonoBehaviour
    {
        [SerializeField]
        Transform planePrefab;
        Transform plane;
        [SerializeField]
        float gridSize = 1f;
        [SerializeField]
        int numLayers = 4;
        [SerializeField]
        float layerHeightDiff = 2f;
        int curLayer = -1;
        bool isActive => plane != null;
        [SerializeField]
        float baseY = 0f;
        [SerializeField]
        float baseScale = 1f;
        [SerializeField]
        float sizePerLayer = 1f;
        [SerializeField]
        bool alwaysShow = false;
        [SerializeField]
        float fadeOutSpeed = 1f;
        [SerializeField]
        float fadeInSpeed = 1f;
        bool shouldFollowPlayer => Player.T != null;
        Transform camTrans => CameraController.IsSetup ? CameraController.Cam.transform : Camera.main.transform;

        int GetLayer() => Mathf.FloorToInt(
            Mathf.Clamp( 1 - (camTrans.forward.y * -1) ,0, 1) * numLayers);
        Vector3 GetBasePos() => shouldFollowPlayer ? new Vector3(Player.Transform.position.x, baseY - curLayer * layerHeightDiff, Player.Transform.position.z) :
            new Vector3(0, baseY - curLayer * layerHeightDiff, 0);
        public void Show()
        {
            plane = Instantiate(planePrefab);
        }
        public void Hide()
        {
            Destroy(plane.gameObject);
            curLayer = -1; 
        }
        void UpdateLayer()
        {
            FadeOutCurPlane();
            FadeInNewPlane();
            plane.transform.position = GetBasePos();
            plane.transform.localScale = Vector3.one * (baseScale + sizePerLayer * curLayer);
        }
        void FadeOutCurPlane()
        {
            if (plane == null)
                return;
            plane.GetComponent<ColorChanger>().ChangeColor(new Color(0, 0, 0, 0), duration: fadeOutSpeed);
            var go = plane.gameObject;
            Callback.Create(() => Destroy(go), fadeOutSpeed);
            plane = null;
        }
        void FadeInNewPlane()
        {
            plane = Instantiate(planePrefab);
            //var changer = plane.GetComponent<ColorChanger>();
            //changer.ChangeColor(new Color(0, 0, 0, 0), shouldLerp: false);
            //changer.ChangeToBaseColor(duration: fadeInSpeed);
        }

        private void Update()
        {
            if (!isActive)
                return;
            var nextLayer = GetLayer();
            if(nextLayer != curLayer)
            {
                curLayer = nextLayer;
                UpdateLayer();
            }else if(shouldFollowPlayer)
                plane.transform.position = new Vector3(Player.Transform.position.x, plane.transform.position.y, Player.Transform.position.z);
        }
        private void Start()
        {
            if (alwaysShow)
                Show();
        }
    }
}
