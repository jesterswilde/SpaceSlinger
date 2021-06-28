using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerper : MonoBehaviour
{
    Queue<LerpElement> queue = new Queue<LerpElement>();
    LerpData curLerp = null;
    [SerializeField]
    float lerpSpeed = 0.5f;
    bool destroyWhenComplete = false;

    #region Making Lerp Elements
    /// <summary>
    /// Lerps to an angle based on the up (defaults to 0,1,0) Invokes action when completed
    /// Note: Will lerp the shortest distance to angle, IE won't lerp at all for 360.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="up"></param>
    /// <param name="finished"></param>
    public void LerpRot(float angle, Vector3? up, Action finished = null, LerpStyle style = LerpStyle.Linear)
    {
        queue.Enqueue(new LerpElement() { Type = LerpType.Rotation, Angle = angle, RotNormal = up ?? transform.up, Action = finished, LerpStyle = style});
    }
    /// <summary>
    /// Lerps to (absolute, not relative) position.  Invokes action when completed
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="finished"></param>
    public void LerpRelativePos(Vector3 pos, Action finished = null, LerpStyle style = LerpStyle.Linear)=>
        queue.Enqueue(new LerpElement() {Type = LerpType.Relative_Translation, Pos = pos, Action = finished, LerpStyle = style });
    public void LerpAbsolutePos(Vector3 pos, Action finished = null, LerpStyle style = LerpStyle.Linear)=>
        queue.Enqueue(new LerpElement() { Type = LerpType.Absolute_Translation, Pos = pos, Action = finished, LerpStyle = style });
    #endregion

    #region Element to Data
    LerpData MakeLerpData(LerpElement el)=> el.Type switch
        {
            LerpType.Rotation => MakeRotationData(el),
            LerpType.Absolute_Translation => MakeAbsoluteTranslationData(el),
            LerpType.Relative_Translation => MakeRelativeTranslationData(el),
            _=> throw new Exception("Unhandled lerp type")
        };
    LerpData MakeRotationData(LerpElement el)
    {
        var lerpFrom = transform.rotation;
        transform.Rotate(el.RotNormal, el.Angle);
        var lerpTo = transform.rotation;
        transform.Rotate(transform.up, -el.Angle);
        return new LerpData() { Type = el.Type, ToQuat = lerpTo, FromQuat = lerpFrom, Transition = 0, Finished = el.Action, LerpStyle = el.LerpStyle };
    }
    LerpData MakeRelativeTranslationData(LerpElement el) =>
       new LerpData() { Type = el.Type, FromPos = transform.position, ToPos = transform.position + el.Pos, Finished = el.Action, LerpStyle = el.LerpStyle };
    LerpData MakeAbsoluteTranslationData(LerpElement el) =>
       new LerpData() { Type = el.Type, FromPos = transform.position, ToPos = el.Pos, Finished = el.Action, LerpStyle = el.LerpStyle };
    #endregion

    void Lerp()
    {
        //lerp faster if there are lots of things pending
        float speed = curLerp.Transition + Time.deltaTime * lerpSpeed * (queue.Count + 1);
        curLerp.Transition = Mathf.Min(1, speed);
        float modTrans = curLerp.LerpStyle switch
        {
            LerpStyle.Linear => curLerp.Transition,
            LerpStyle.Hermite => Mathfx.Hermite(0, 1, curLerp.Transition),
            LerpStyle.Sinerp => Mathfx.Sinerp(0, 1, curLerp.Transition),
            _ => throw new Exception("Not a valid input")
        };
        switch (curLerp.Type) {
            case LerpType.Rotation: 
                transform.rotation = Quaternion.Slerp(curLerp.FromQuat, curLerp.ToQuat, modTrans);
                break;
            case LerpType.Absolute_Translation:
            case LerpType.Relative_Translation:
                transform.position = Vector3.Slerp(curLerp.FromPos, curLerp.ToPos, modTrans);
                break;
        }
        if (curLerp.Transition >= 1f)
        {
            curLerp.Finished?.Invoke();
            curLerp = null;
        }
    }
    void Update()
    {
       if(curLerp == null && queue.Count > 0)
            curLerp = MakeLerpData(queue.Dequeue());
        if (curLerp != null)
            Lerp();
        if (destroyWhenComplete && curLerp == null && queue.Count == 0)
           Destroy(this);
    }
    #region Static Making funcs
    /// <summary>
    /// Adds lerper componenet to gameobject then invokes the related method to move gameobject over time.
    /// Returns the lerper component which can have more elements queued up. 
    /// If there is already a lerper on the gameObject, it will just add to the queue. 
    /// </summary>
    /// <param name="go"></param>
    /// <param name="destination"></param>
    /// <param name="finished"></param>
    /// <param name="speed"></param>
    /// <param name="destroyAfter"></param>
    /// <returns></returns>
    public static Lerper MoveToRelative(GameObject go, Vector3 destination, Action finished = null, float speed = 0f, bool destroyAfter = false, LerpStyle style = LerpStyle.Linear)
    {
        var lerper = MakeLerper(go, speed, destroyAfter);
        lerper.LerpRelativePos(destination, finished, style: style);
        return lerper; 
    }
    public static Lerper MoveToAbsolute(GameObject go, Vector3 destination, Action finished = null, float speed = 0f, bool destroyAfter = true, LerpStyle style = LerpStyle.Linear)
    {
        var lerper = MakeLerper(go, speed, destroyAfter);
        lerper.LerpAbsolutePos(destination, finished, style: style);
        return lerper; 
    }
    /// <summary>
    /// Adds lerper component to gameobject then invokes the related method to rotate gameobject over time. 
    /// Returns the lerper component which can have more elements queued up. 
    /// If there is already a lerper on the gameObject, it will just add to the queue. 
    /// </summary>
    /// <param name="go"></param>
    /// <param name="angle"></param>
    /// <param name="up"></param>
    /// <param name="finished"></param>
    /// <param name="speed"></param>
    /// <param name="destroyAfter"></param>
    /// <returns></returns>
    public static Lerper RotateTo(GameObject go, float angle, Vector3? up = null, Action finished = null, float speed = 0f, bool destroyAfter = false, LerpStyle style = LerpStyle.Linear)
    {
        var lerper = MakeLerper(go, speed, destroyAfter);
        lerper.LerpRot(angle, up, finished, style: style);
        return lerper;
    }
    static Lerper MakeLerper(GameObject go, float speed = 0f, bool destroyAfter = false)
    {
        Lerper lerper = go.GetComponent<Lerper>();
        if(lerper == null)
            lerper = go.AddComponent<Lerper>();
        if (speed > 0)
            lerper.lerpSpeed = speed;
        lerper.destroyWhenComplete = destroyAfter;
        return lerper;
    }
    #endregion
    class LerpData
    {
        public LerpType Type;
        public Quaternion FromQuat;
        public Quaternion ToQuat;
        public Vector3 FromPos;
        public Vector3 ToPos;
        public float Transition;
        public Action Finished;
        public LerpStyle LerpStyle;
    }
    class LerpElement
    {
        public LerpType Type;
        public float Angle;
        public Vector3 RotNormal;
        public Vector3 Pos;
        public Action Action;
        public LerpStyle LerpStyle = LerpStyle.Linear;
    }
    enum LerpType
    {
        Rotation,
        Relative_Translation,
        Absolute_Translation,
    }
    public enum LerpStyle
    {
        Linear,
        Hermite,
        Sinerp,
    }
}
