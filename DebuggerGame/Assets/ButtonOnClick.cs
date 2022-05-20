using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOnClick : MonoBehaviour
{
    public string levelName;
    void Start()
    {
        Button targetButton = this.gameObject.GetComponent<Button>();
        targetButton.onClick.AddListener(delegate {this.gameObject.GetComponentInParent<ButtonManager>().ButtonMoveScene(levelName);});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
