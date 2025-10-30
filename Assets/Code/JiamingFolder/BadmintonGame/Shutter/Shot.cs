
using System.Collections.Generic;
using UnityEngine;

public abstract class Shot : MonoBehaviour
{


    [SerializeField] private GameObject shotLocationMaker;
    [SerializeField] protected float shotSpreadRange;
    [SerializeField] protected float travelTime = 10f;


    protected Vector3 targetPos;
    protected Vector3 startPos;
    private Vector3 prevPos;


    protected bool isFlying = false;
    protected float elapsedTime;



    protected Vector3 CalculateWhichTarget(List<Transform> listOfTargets, int direction)
    {

        if (listOfTargets == null || listOfTargets.Count == 0)
            return Vector3.zero;

        Vector3 initialTarget = Vector3.zero;

        //set intial Target
        if (direction == 1 )
        {
            initialTarget = listOfTargets[0].position;
        }
        else if(direction == 2)
        {
            initialTarget =  listOfTargets[1].position;
        }
        else
        {
            int randomIndex = Random.Range(0, listOfTargets.Count);
            initialTarget =  listOfTargets[randomIndex].position;
        }


        //calculate final targetPos 
        // a random spread drawn ard the inital position (left and right spread only)
        Vector2 randomOffset = Random.insideUnitCircle * shotSpreadRange;
        Vector3 finalTarget = new Vector3(
            initialTarget.x + randomOffset.x,
            initialTarget.y,
            initialTarget.z + randomOffset.y
        );

        return finalTarget;

    }

    public virtual void Cancel()
    {
        isFlying = false;
    }

    public void ExecuteShot(List<Transform> listOfTargets, int shotDirection)
    {
        startPos = transform.position;
        targetPos = CalculateWhichTarget(listOfTargets, shotDirection);
        SetLocationMarker(targetPos);
        elapsedTime = 0f;
        isFlying = true;



        // Stop any other shot updates on this object
        foreach (var s in GetComponents<Shot>())
            if (s != this) s.Cancel();
    }


    public void SetLocationMarker(Vector3 targetPosition, bool reset = false)
    {

        shotLocationMaker.gameObject.SetActive(!reset);
        shotLocationMaker.transform.position = targetPosition;
    }



    protected void UpdateRotation(Vector3 currentPos)
    {
        if (!isFlying) return;

        if (prevPos == Vector3.zero)
        {
            prevPos = currentPos;
            return;
        }

        Vector3 velocity = (currentPos - prevPos).normalized;
        prevPos = currentPos;

        if (velocity.sqrMagnitude > 0.0001f)
        {
            // Align -Y axis of shuttle with velocity
            Quaternion rotation = Quaternion.LookRotation(velocity, Vector3.forward);
            transform.rotation = rotation * Quaternion.Euler(-90f, 0f, 0f);
        }
    }
}
