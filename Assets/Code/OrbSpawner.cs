using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

class OrbSpawner : MonoBehaviour
{
    [SerializeField]
    int itemsNeeded;
    Vector3 spawnPos;
    List<Orb> orbs = new List<Orb>();
    [SerializeField]
    Transform baseOrbPrefab;
    [SerializeField]
    OrbWrapper orbPrefab;
    [SerializeField]
    float shootDist = 5f;
    [SerializeField]
    int orbsPerStep = 8;

    private void Start()
    {
        Player.Inventory.OnItemAdded += PlayerGotItem;
    }

    private void PlayerGotItem(Item obj)
    {
        if (obj.Type == ItemType.BasisVector)
        {
            itemsNeeded--;
            if (itemsNeeded == 0)
                SpawnOrbs();
        }
    }

    private void SpawnOrbs()
    {
        spawnPos = Player.Transform.position + Orientation.Right * (shootDist /2) + Orientation.Forward * shootDist + Orientation.Up * shootDist * 2;
        var startOrb = Instantiate(baseOrbPrefab);
        startOrb.transform.position = Player.Transform.position;
        Lerper.MoveToAbsolute(startOrb.gameObject, spawnPos, () =>
        {
            StartCoroutine(MakeAndPositionBaseOrb());
            Callback.Create(() => Destroy(startOrb.gameObject), 10f);
        });
    }

    IEnumerator MakeAndPositionBaseOrb()
    {
        var max = LatticeMaker.Points.Count - 1;
        for(int i = 0; i <= max; i++)
        {
            ShootOrb(LatticeMaker.Points[max-i], max-i);
            if ((i % orbsPerStep == 0))
                yield return new WaitForSeconds(.05f);
        };
    }
    void ShootOrb(PointData point, int i)
    {
        Vector3 target = (Mathf.Sin(i) * transform.right * 10 + Mathf.Cos(i) * transform.up * 10 + (i * transform.forward / 10)) * shootDist;
        var orb = Instantiate(orbPrefab);
        orb.transform.position = spawnPos;
        orb.transform.forward = Orientation.Forward;
        point.Transform = orb.transform;
        Lerper.MoveToRelative(orb.gameObject, target, speed: 0.2f, style: Lerper.LerpStyle.Sinerp,
            finished: () => orb.AnimateToPos(point.Position)
        );
    }
}
