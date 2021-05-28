using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Planet : SerializedMonoBehaviour
{
    [SerializeField]
    Detector outerRing;
    float outerRadius;
    [SerializeField, HorizontalGroup("gravStr", LabelWidth = 70)]
    float minGrav = 0.5f;
    [SerializeField, HorizontalGroup("gravStr", LabelWidth = 70)]
    float maxGrav = 1f;
    [SerializeField]
    float innerRaidus = 5f;
    Delta<bool> isAffectingPlayer = new Delta<bool>();
    Vector3 gravityDir => (transform.position - Player.Transform.position);
    public Vector3 GravityForce => gravityDir * Vector3.Distance(transform.position, Player.Transform.position)
                                                       .Map(innerRaidus, outerRadius, maxGrav, minGrav);
    [SerializeField]
    float influence;
    public float Influence => Vector3.Distance(transform.position, Player.Transform.position)
                                        .Map(innerRaidus, outerRadius, 1f, 0f, true);


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, innerRaidus);
    }
    private void Update()
    {
        influence = Influence;
        isAffectingPlayer.Update(outerRing.IsBlocked);
        if (isAffectingPlayer.Changed)
        {
            if (isAffectingPlayer.Value)
                Gravity.AddPlanet(this);
            else
                Gravity.RemovePlanet(this);
        }
        if(isAffectingPlayer.Value && Player.T.Position.Changed)
            Gravity.SetDirty(); 
    }
    private void Awake()
    {
        var scale = outerRing.transform.lossyScale.magnitude;
        outerRadius = outerRing.GetComponent<SphereCollider>().radius * scale;
        innerRaidus = GetComponent<SphereCollider>().radius * transform.lossyScale.magnitude;
    }
}
