using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shot : MonoBehaviour
{
    private List<Transform> targetPoints;


    private bool isFlying = false;

    public abstract void ExecuteShot(Transform racketTransform, Rigidbody shuttlecockRigidbody);
}
