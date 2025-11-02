using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    [Header("Level tiles (menu objects)")]
    public GameObject level1Item;   
    public GameObject level2Item;   
    public GameObject level3Item;   
    public GameObject lockedToast;  

    void OnEnable() => RefreshTiles();

    public void RefreshTiles()
    {
        int unlocked = LevelProgress.GetUnlocked();   

        if (level1Item) level1Item.SetActive(true);
        if (level2Item) level2Item.SetActive(unlocked >= 2);
        if (level3Item) level3Item.SetActive(unlocked >= 3);
    }

    public void OpenLevel(int levelId)
    {
        if (levelId == 1)
        {
            SceneManager.LoadScene("Intro");
            return;
        }

        if (!LevelProgress.IsLevelUnlocked(levelId))
        {
            if (lockedToast) lockedToast.SetActive(true);
            Debug.Log($"[LevelMenu] Level {levelId} is locked.");
            return;
        }

        string levelName = "Level" + levelId; 
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelName);
    }
}