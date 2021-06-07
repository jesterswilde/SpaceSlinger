using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SerializeField]
class Basis
{
    [SerializeField, HorizontalGroup("a", LabelWidth = 50)]
    Transform transform;
    public Vector3 Position => transform.position;
    [HorizontalGroup("a", LabelWidth = 10)]
    public Cog Cog;
    public float TurnValue => Cog.TurnAmount.Value;
    [HideInInspector]
    public Vector3 Offset;
}
public class BasisVectorContraption : SerializedMonoBehaviour
{
    [SerializeField]
    int associatedVector = 0;

    [SerializeField]
    Dictionary<Cog.Axis, Basis> basis = new Dictionary<Cog.Axis, Basis>();
    [SerializeField]
    Transform VecObj;
    [SerializeField]
    Vector3 vecDir;
    [SerializeField]
    float magnitude = 5f;
    Vector3 basePos;
    CameraSystem camSystem;

    public void UpdateMagnitude(float mag)
    {
        magnitude = mag;
    }
    public void NormalizeCogs(Cog.Axis axis)
    {
        float remainder = 1 - basis[axis].TurnValue;
        float sum = basis.Where(kvp => kvp.Key != axis).Sum(kvp => kvp.Value.TurnValue);
        basis.Where(kvp => kvp.Key != axis).Select(kvp => kvp.Value.Cog)
            .ForEach(cog => {
                float oldAmount = cog.TurnAmount.Value;
                if (sum == 0)
                    cog.TurnAmount.Update(remainder / 2f);
                else
                    cog.TurnAmount.Update((oldAmount / sum) * remainder);
            });
        UpdateVecPos();
    }
    void SetupCogs()
    {
        var vec = LatticeMaker.GetBasisVector(associatedVector).ToVector();
        var normal = vec.normalized;
        magnitude = vec.magnitude;
        basis[Cog.Axis.X].Cog.TurnAmount.Update(normal.x * 2 - 1);
        basis[Cog.Axis.Y].Cog.TurnAmount.Update(normal.y * 2 - 1);
        basis[Cog.Axis.Z].Cog.TurnAmount.Update(normal.z * 2 - 1);
        UpdateVecPos();
    }
    void UpdateVecPos(){
        vecDir = basis.Values.Aggregate(Vector3.zero, (vec, val) => val.Offset * val.TurnValue + vec);
        VecObj.position = vecDir + basePos;
        LatticeMaker.UpdateBasis(associatedVector, vecDir.normalized, magnitude);
    }
    public void Connect()
    {
        CameraController.LoadSystem(camSystem);
    }
    public void Disconnect()
    {
        CameraController.LoadSystem(null);
    }

    private void Start()
    {
        SetupCogs();
    }
    private void Awake()
    {
        basePos = VecObj.position;
        camSystem = GetComponentInChildren<CameraSystem>();
    }
}