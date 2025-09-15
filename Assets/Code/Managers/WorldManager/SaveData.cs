using System.Collections.Generic;

[System.Serializable]
public class LevelProgress
{
    public int levelIndex;
    public int stars; // 0 to 3 stars
}

[System.Serializable]
public class GameProgress
{
    public List<LevelProgress> levels = new List<LevelProgress>();
}
