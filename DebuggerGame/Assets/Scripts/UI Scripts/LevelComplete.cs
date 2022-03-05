using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    [SerializeField]
    private string nextScenePath;

    void Start()
    {
        Board.instance.WinEvent.AddListener(OnWin);
    }

    void OnWin()
    {
        GetComponent<Animator>().SetTrigger("GameWon");
    }

    void OnLevelCompleteAnimationFinish()
    {
        if(nextScenePath != "")
        {
            SceneManager.LoadScene(nextScenePath);
        }
    }
}
