using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("fk..");

        Color newColor;


        //TMPro.TextMeshProUGUI[] textMeshes = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        foreach (Transform cTransform in transform)
        {
            TMPro.TextMeshProUGUI text = cTransform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            string lName = cTransform.GetComponent<ButtonOnClick>().levelName;
            if (!collection.unlockedLevels.Contains(lName))
            {
                newColor = text.color;
                newColor.a = newColor.a / 3;
                text.color = newColor;
            }
            
        }
    }

    public void ButtonMoveScene(string levelName)
    {
        if (SaveManager.instance.unlockedLevels.Contains(levelName))
        {
            string combineString = "Level " + levelName.Substring(0,2) + "-" + levelName.Substring(2,2);
            Debug.Log("loading level" + combineString);
            SceneManager.LoadScene(combineString);
            SaveManager.instance.currentLevel = levelName;
        }
        else
        {
            Debug.Log("ERROR MESSAGE level not unlocked(we need to add some UI prompt lol) Talk to Felix Peng if you have questions");
        }
        
    }

}
