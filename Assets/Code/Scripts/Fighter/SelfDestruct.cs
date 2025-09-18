using Unity.VisualScripting;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float lifespan = 5f;
    private float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time_passed = 0;
        EventHandler.GotKill += ResetTimer;
    }

    // Update is called once per frame
    void Update()
    {
        time_passed += Time.deltaTime;
        if (time_passed > lifespan)
        {
            EventHandler.InvokeGotKill(Camera.main.gameObject, gameObject);
        }
    }

    public void ResetTimer(GameObject killer, GameObject target)
    {
        if (target == gameObject)
        {
            time_passed = 0;
        }
    }
}
