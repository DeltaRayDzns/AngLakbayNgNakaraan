using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelProgress
{
    private const string Key = "unlocked_level"; 

    public static int GetUnlocked() => PlayerPrefs.GetInt(Key, 1); 
    public static bool IsLevelUnlocked(int levelId) => levelId <= GetUnlocked();

    public static void UnlockUpTo(int levelId)
    {
        int cur = GetUnlocked();
        if (levelId > cur)
        {
            PlayerPrefs.SetInt(Key, levelId);
            PlayerPrefs.Save();
        }
    }

    public static void UnlockNextFromCurrentScene()
    {
        int current = GetCurrentLevelId();
        if (current > 0) UnlockUpTo(current + 1);
    }
    public static void UnlockNextFromCurrent() => UnlockNextFromCurrentScene();

    public static void ResetProgress() => PlayerPrefs.DeleteKey(Key);

    private static int GetCurrentLevelId()
    {
        string name = SceneManager.GetActiveScene().name;

        if (name.StartsWith("Level"))
        {
            if (int.TryParse(name.Substring("Level".Length), out int n))
                return n;
        }

        if (name == "Intro" || name == "Tutorial") return 0;

        return -1; 
    }
}