using System.Linq;
using UnityEngine;

public class ShootBack : MonoBehaviour
{
    [SerializeField]
    private BadmintonCourtTargets targets;

    private bool shotBack;

    private void Start()
    {
        shotBack = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shutter"))
        {

            if (!shotBack) return;
            shotBack = false;


            Shot shot = other.GetComponent<Shot>();
            shot.ExecuteShot(targets.backTargetsBlue, 3);
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Shutter"))
        {

            if (!shotBack) return;
            shotBack = false;

            Shot shot = other.GetComponent<Shot>();
            shot.ExecuteShot(targets.backTargetsBlue,3);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        shotBack = true;
    }
}
