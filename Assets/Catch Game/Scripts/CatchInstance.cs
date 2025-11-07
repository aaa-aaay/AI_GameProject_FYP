using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class CatchData
{
    [SerializeField] private GameObject player;
    [SerializeField] private int points;

    public GameObject GetPlayer()
    {
        return player;
    }

    public int GetPoints()
    {
        return points;
    }

    public void SetPoints(int new_points)
    {
        points = new_points;
    }

    public void IncreasePoints()
    {
        points++;
    }
}

public class CatchInstance : MonoBehaviour
{
    [SerializeField] private int max_items;
    [SerializeField] private float game_time;

    [SerializeField] private CatchItem item_prefab;

    [SerializeField] private List<CatchData> players;
    [SerializeField] private List<GameObject> spawners;

    private List<CatchItem> item_pool;

    private float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        item_pool = new List<CatchItem>();
        time_passed = 0;
        for (int i = 0; i < max_items; i++)
        {
            item_pool.Add(Instantiate(item_prefab.gameObject, transform).GetComponent<CatchItem>());
        }
    }

    private void Update()
    {
        time_passed += Time.deltaTime;

        if (time_passed > game_time)
        {
            Restart();
        }
    }

    private void Restart()
    {
        for (int i = 0; i < max_items;i++)
        {
            EventHolder.InvokeOnRestart(item_pool[i].gameObject);
        }
        for (int i = 0; i < players.Count; i++)
        {
            EventHolder.InvokeOnRestart(players[i].GetPlayer());
            players[i].SetPoints(0);
        }
        for(int i = 0;i < spawners.Count; i++)
        {
            EventHolder.InvokeOnRestart(spawners[i].gameObject);
        }
        time_passed = 0;
    }

    public List<CatchItem> GetItems()
    {
        return item_pool;   
    }

    public List<CatchData> GetData()
    {
        return players;
    }

    public CatchItem GetUnused()
    {
        for(int i = 0;i < max_items;i++)
        {
            if (!item_pool[i].gameObject.activeSelf)
            {
                return item_pool[i];
            }
        }
        return null;
    }

    public void OnScore(GameObject scorer, GameObject scored, int points)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetPlayer() == scorer)
            {
                players[i].IncreasePoints();
            }
        }
    }
}
