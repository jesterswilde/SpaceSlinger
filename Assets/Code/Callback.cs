using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Have functions invoked at a later time.  
/// Must have one in scene to work.
/// Is singleton
/// </summary>
public class Callback : MonoBehaviour
{
    static int curID = 0;
    static List<CB> updateCallbacks = new List<CB>();
    static List<CB> fixedUpdateCallbacks = new List<CB>();

    void Update()
    {
        updateCallbacks = Process(Time.deltaTime, updateCallbacks);
    }
    void FixedUpdate()
    {
        fixedUpdateCallbacks = Process(Time.fixedDeltaTime, fixedUpdateCallbacks);
    }
    /// <summary>
    /// Inovke passed action after n seconds.
    /// Called during update loop and uses deltaTime
    /// </summary>
    /// <param name="action">Thing to be invoked</param>
    /// <param name="timer">in seconds</param>
    /// <returns>ID, used to cancel callback early</returns>
    public static int Create(Action action, float timer) => AddCB(action, timer, updateCallbacks);
    /// <summary>
    /// Removes a callback that hasn't yet been invoked.
    /// </summary>
    /// <param name="id"></param>
    public static void Remove(int id) => updateCallbacks = updateCallbacks.Where(cb => cb.ID != id).ToList(); 
    /// <summary>
    /// Invoke passed action after n seconds
    /// Called during fixed update loop and uses fixedDeltaTime
    /// </summary>
    /// <param name="action">Thing to be invoked</param>
    /// <param name="timer">in seconds</param>
    /// <returns>ID, used to cancel callback early</returns>
    public static int CreateFixed(Action action, float timer) => AddCB(action, timer, fixedUpdateCallbacks);
    /// <summary>
    /// Removes a callback that hasn't yet been invoked.
    /// </summary>
    /// <param name="id"></param>
    public static void RemoveFixed(int id) => fixedUpdateCallbacks = fixedUpdateCallbacks.Where(cb => cb.ID != id).ToList(); 

    static int AddCB(Action action, float timer,List<CB> callbacks)
    {
        var cb = new CB() { Action = action, Timer = timer, ID = curID++ };
        callbacks.Add(cb);
        return cb.ID;
    }
    static List<CB> Process(float delta, List<CB> callbacks)=>
        callbacks?.Where(cb =>
        {
            cb.Timer -= delta;
            if (cb.Timer <= 0)
                cb.Action();
            return cb.Timer > 0;
        }).ToList();

    //This bit is so that it works without domain reloading. Increase that start time!
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetStatic()
    {
        updateCallbacks = new List<CB>();
        fixedUpdateCallbacks = new List<CB>();
        curID = 0;
    }
}
class CB
{
    public Action Action;
    public float Timer;
    public int ID;
}
