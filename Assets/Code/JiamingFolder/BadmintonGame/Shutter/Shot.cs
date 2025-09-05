using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shot : MonoBehaviour
{


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

    public virtual void Cancel()
    {
        isFlying = false;
    }

    public void ExecuteShot(List<Transform> listOfTargets)
    {
        startPos = transform.position;
        targetPos = CalculateWhichTarget(listOfTargets);

        elapsedTime = 0f;
        isFlying = true;


        // Stop any other shot updates on this object
        foreach (var s in GetComponents<Shot>())
            if (s != this) s.Cancel();
    }
}
