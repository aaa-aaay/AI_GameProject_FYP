using UnityEngine;

public class Punch : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float range;

    public void punch()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, range, Vector3.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            EventHandler.InvokeGotHit(gameObject, hits[i].transform.gameObject, damage);
        }
    }
}
