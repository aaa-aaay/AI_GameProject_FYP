
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private SaveLoadManager _loadManager;
    private GameProgress _progress;


    [SerializeField] private List<LobbyWorld> _worldList;
    private void Start()
    {
        _loadManager = ServiceLocator.Instance.GetService<SaveLoadManager>();

        _loadManager.LoadData();
        _progress = _loadManager.GetGameSave();


        foreach (var world in _worldList)
        {
            world.SetState(false);
        }

        foreach (var data in _progress.levels)
        {
            if (data.stars != 0)
            {
                _worldList[data.levelIndex - 1].SetState(true);

            }
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            Debug.Log(_progress.levels[0].stars);
            Debug.Log(_progress.levels[1].stars);
            Debug.Log(_progress.levels[2].stars);
            Debug.Log(_progress.levels[3].stars);
        }
    }
}
