using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartProgress : MonoBehaviour
{
    Image image;
    void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (Board.instance != null)
        {
            if (Board.instance.restarting)
            {
                image.fillAmount = Board.instance.restartingProgressLeft.Value;
            }
            else
            {
                image.fillAmount = 0;
            }
        }
    }
}
