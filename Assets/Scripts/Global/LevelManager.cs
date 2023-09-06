using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static List<LevelDataContainer.LevelData> levels = new List<LevelDataContainer.LevelData>();

    public static int currentLevel => PlayerPrefs.GetInt("CurrentLevel", 0);

    public static void Init()
    {
        levels = Resources.Load<LevelDataContainer>("Data/LevelDataContainer").levels;
    }

    public static void SaveLevelUp()
    {
        int newLevel = currentLevel != levels.Count ? currentLevel + 1 : currentLevel;
        PlayerPrefs.SetInt("CurrentLevel", newLevel);
    }

    public static LevelDataContainer.LevelData GetLevelData()
    {
        return currentLevel < levels.Count ? levels[currentLevel] : levels.Last();
    }
}
