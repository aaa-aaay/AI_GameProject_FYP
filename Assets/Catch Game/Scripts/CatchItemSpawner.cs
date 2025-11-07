using UnityEngine;

public class CatchItemSpawner : MonoBehaviour
{
    [SerializeField] private CatchInstance instance;
    [SerializeField] private float min_spawn_time;
    [SerializeField] private float max_spawn_time;

    private float time_passed;
    private float spawn_time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Restart();

        EventHolder.OnRestart += Restart;
    }

    private void OnDestroy()
    {
        EventHolder.OnRestart -= Restart;
    }

    // Update is called once per frame
    void Update()
    {
        time_passed += Time.deltaTime;
        if (time_passed >spawn_time)
        {
            CatchItem item = instance.GetUnused();

            if (item != null)
            {
                item.transform.position = transform.position;
                item.gameObject.SetActive(true);
            }

            Restart();
        }
    }

    public void Restart()
    {
        time_passed = 0;
        spawn_time = Random.Range(min_spawn_time, max_spawn_time);
    }

    public void Restart(GameObject player)
    {
        if (player == gameObject)
        {
            Restart(gameObject);
        }
    }
}
