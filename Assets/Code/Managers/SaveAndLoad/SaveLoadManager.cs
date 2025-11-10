using UnityEngine;

public class SaveLoadManager : MonoBehaviour,IGameService
{
    private GameProgress progress;

    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
        progress = new GameProgress();
        //CreateNewSaveData();
        LoadData();
    }

    private void OnDisable()
    {
        //ServiceLocator.Instance.RemoveService<SaveLoadManager>(false);
    }

    public void SaveData(int worldNo, int starCount)
    {

        foreach(var levelDetail in progress.levels)
        {
            if(levelDetail.levelIndex == worldNo)
            {
                Debug.Log("found the world");
                Debug.Log(levelDetail.stars);
                Debug.Log(levelDetail.levelIndex);
                if (starCount > levelDetail.stars)
                {
                    levelDetail.stars = starCount;
                    levelDetail.unlocked = true;
                    Debug.Log("unlocked");
                }
                else return;


                GameProgress progressToSave = new GameProgress();
                progressToSave = progress;
                string json = JsonUtility.ToJson(progressToSave);
                PlayerPrefs.SetString("GameProgress", json);
                PlayerPrefs.Save();


            }
        }
    }

    public bool LoadData()
    {

        string keyToGet = "GameProgress";


        if (PlayerPrefs.HasKey(keyToGet))
        {
            string json = PlayerPrefs.GetString(keyToGet);
            progress = JsonUtility.FromJson<GameProgress>(json);
            return true;
        }
        else
        {
            CreateNewSaveData();
            //save a new file


        }

        return false;
    }

    public GameProgress GetGameSave()
    {
        return progress;
    }

    public void CreateNewSaveData()
    {
        GameProgress newSave = new GameProgress();
        newSave.levels.Add(new LevelProgress { levelIndex = 1, stars = 0, unlocked = true });
        newSave.levels.Add(new LevelProgress { levelIndex = 2, stars = 0, unlocked = false });
        newSave.levels.Add(new LevelProgress { levelIndex = 3, stars = 0, unlocked = false });
        newSave.levels.Add(new LevelProgress { levelIndex = 4, stars = 0, unlocked = false });
        newSave.levels.Add(new LevelProgress { levelIndex = 5, stars = 0, unlocked = false });

        string json = JsonUtility.ToJson(newSave);
        PlayerPrefs.SetString("GameProgress", json);
        PlayerPrefs.Save();

        progress = newSave; 

        
    }


    //things to do here: load pref file, save pref file
    // tell the lobbyworlds their stats everytime the world is loaded;
}
