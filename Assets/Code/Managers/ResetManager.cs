using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetManager : MonoBehaviour
{
    static ResetManager t;
    [SerializeField]
    int maxRemoteness;
    [SerializeField]
    int dropRemoteness;
    [SerializeField]
    float dropSpeed = 5;
    Checkpoint activeCheckpoing;
    Vector3 playerStartingPos;
    Vector3 checkPointPos => activeCheckpoing?.transform.position ?? playerStartingPos;

    void CheckForDrop(Cell curCell, Cell prevCell)
    {
        if (curCell == null)
            return;
        if(curCell.Remoteness >= maxRemoteness){
            var cell = GridManager.Raycast(checkPointPos, Gravity.AmbientGravity.normalized * -1, cell => cell.Remoteness >= dropRemoteness);
            var cellPos = GridManager.GetPositionInCell(cell);
            float dist = Vector3.Distance(checkPointPos, cellPos);
            Player.Transform.position = checkPointPos + Gravity.AmbientGravity.normalized * -1 * dist;
            Player.T.Rigid.velocity = Gravity.AmbientGravity * dropSpeed;
        }
    }

    internal static void SetActiveCheckpoint(Checkpoint checkpoint)
    {
        t.activeCheckpoing?.SetAsInactive();
        t.activeCheckpoing = checkpoint;
    }

    private void Start()
    {
        if (!Player.DoesExist)
            return;
        GridManager.PlayerCell.OnChange += CheckForDrop;
        playerStartingPos = Player.T.transform.position;
    }
    private void Awake()
    {
        t = this;
    }
}
