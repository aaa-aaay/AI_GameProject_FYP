using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shot : MonoBehaviour
{
    [SerializeField]
    protected List<Transform> targetPoints;


    protected Vector3 startPos;
    protected Vector3 targetPos;

    protected float elapsedTime;

    protected bool isFlying = false;

    protected Vector3 CalculateWhichTarget(List<Transform> listOfTargets)
    {
        if (listOfTargets == null || listOfTargets.Count == 0)
            return Vector3.zero; 

        int randomIndex = Random.Range(0, listOfTargets.Count);
        return listOfTargets[randomIndex].position;
    }


    public abstract void ExecuteShot(List<Transform> listOfTargets);
}
