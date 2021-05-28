using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetManager : MonoBehaviour
{
    [SerializeField]
    int maxRemoteness;
    [SerializeField]
    int dropRemoteness;
    Vector3 checkPointPos;

    void CheckForDrop(Cell curCell, Cell prevCell)
    {
        if (curCell == null)
            return;
        if(curCell.Remoteness >= maxRemoteness){
            var cell = GridManager.Raycast(checkPointPos, Gravity.AmbientGravity.normalized * -1, cell => cell.Remoteness >= dropRemoteness);
            var cellPos = GridManager.GetPositionInCell(cell);
            float dist = Vector3.Distance(checkPointPos, cellPos);
            Player.Transform.position = checkPointPos + Gravity.AmbientGravity.normalized * -1 * dist;
            Player.T.Rigid.velocity = Vector3.zero;
        }
    }
    private void Start()
    {
        GridManager.PlayerCell.OnChange += CheckForDrop;
    }
}
