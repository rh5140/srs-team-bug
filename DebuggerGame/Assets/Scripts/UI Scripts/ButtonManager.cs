using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public SaveState collection;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("fk..");

        Color newColor;
        TextMesh[] textMeshes = GetComponentsInChildren<TextMesh>();
        foreach (TextMesh textMesh in textMeshes)
        {
            Debug.Log("Color Setting!");
            newColor = textMesh.color;
            newColor.a = newColor.a / 2;
            textMesh.color = newColor;
        }
    }

    public void ButtonMoveScene(string levelName)
    {
        if (collection.unlockedLevels.Contains(levelName))
        {
            string combineString = "Level " + levelName.Substring(0,2) + "-" + levelName.Substring(2,2);
            Debug.Log("loading level" + combineString);
            SceneManager.LoadScene(combineString);
        }
        else
        {
            Debug.Log("ERROR MESSAGE level not unlocked(we need to add some UI prompt lol) Talk to Felix Peng if you have questions");
        }
        
    }

}
