using UnityEngine;

public class DisableOnKill : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventHolder.OnKill += CheckDead;
    }

    public void CheckDead(GameObject killer, GameObject target)
    {
        if (target == gameObject)
        {
            gameObject.SetActive(false);
        }
    }
}
