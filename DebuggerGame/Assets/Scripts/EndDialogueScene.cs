using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDialogueScene : MonoBehaviour
{
    void Start()
    {
        Board.instance.EndLevelEvent.AddListener(OnEndLevel);
    }

    void Update()
    {
        //Instawin upon hitting correct key (TEMP)
        float win = Input.GetAxisRaw("Win");
        if (!Mathf.Approximately(win, 0f))
        {
            Board.instance.InstantWin();
        }
    }

    public void OnEndLevel()
    {
        SaveManager.instance.currentLevel = null;
        foreach (string levelName in Board.instance.unlockLevels)
        {
            SaveManager.instance.unlockedLevels.Add(levelName);
            SaveManager.instance.Save();
        }

        foreach (string characterName in Board.instance.unlockCharacters)
        {
            if (!SaveManager.instance.unlockedCharacters.Contains(characterName))
            {
                SaveManager.instance.newestCharacterUnlocks.Add(characterName);
                SaveManager.instance.Save();
            }

            SaveManager.instance.unlockedCharacters.Add(characterName);
            SaveManager.instance.Save();
        }
    }
}

