
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private SaveLoadManager _loadManager;
    private GameProgress _progress;



    [SerializeField] private List<LobbyWorld> _worldList;
    [SerializeField] private List<WorldSelect> _worldSelectList;
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
            LobbyWorld world = _worldList[data.levelIndex - 1];
            _worldSelectList[data.levelIndex - 1].SetStarCount(data.stars); //set the star counts
            int tmpint = data.levelIndex - 1;
            if (data.stars >= 3) 
                world.SetState(true);
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
            Debug.Log(_progress.levels[4].stars);
        }
    }
}
