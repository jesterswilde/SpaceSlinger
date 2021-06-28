using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrbWrapper : MonoBehaviour
{
    Oscillator childOsc;
    Orb child;
    [SerializeField]
    Oscillator.MotionInfo anticMotion; 
    [SerializeField]
    Oscillator.MotionInfo arcMotion;
    public void AnimateToPos(Vector3 destination)=> StartCoroutine(BeginTransition(destination));
    //Meshing coroutines with callbacks feels bad!
    IEnumerator BeginTransition(Vector3 destination)
    {
        childOsc = child.gameObject.AddComponent<Oscillator>();
        AnticipationVibration();
        yield return new WaitForSeconds(Random.Range(1f, 4f));
        ArcToDestination(destination);
    }
    void ArcToDestination(Vector3 destination)
    {
        childOsc.UpdateMotion(arcMotion);
        Lerper.MoveToAbsolute(gameObject, destination, Cleanup, arcMotion.Frequency);
    }
    void AnticipationVibration()=> childOsc.UpdateMotion(anticMotion);
    void Cleanup()
    {
        Destroy(childOsc);
        child.transform.SetParent(null);
        child.GetComponent<Collider>().enabled = true;
        Lerper.MoveToAbsolute(child.gameObject, transform.position);
    }
    private void Awake()
    {
        child = GetComponentInChildren<Orb>(); 
    }
}
