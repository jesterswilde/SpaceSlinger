using Sirenix.Serialization;
using Unity.VisualScripting;
using UnityEngine;

namespace Drawing
{
    public class FloatingPlatform : MonoBehaviour
    {
        Rigidbody rigid;
        Vector3 startPos;
        [SerializeField]
        float forceMultiplier = 1f;
        [SerializeField]
        float outMultiplier = 1f;
        [SerializeField]
        float floatForce = 1f;
        [SerializeField]
        public void GotHit(Vector3 force)
        {
            var outForce = (transform.position - Camera.main.transform.position).normalized * force.magnitude;
            rigid.AddForce(force * forceMultiplier, ForceMode.Impulse);
            rigid.AddForce(outForce * outMultiplier, ForceMode.Impulse);
        }
        private void FixedUpdate()
        {
            var diff = startPos - transform.position;
            rigid.AddForce(diff * floatForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
        }
        private void Start()
        {
            startPos = transform.position;
        }
    }
}
