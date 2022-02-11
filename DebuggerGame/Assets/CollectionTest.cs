using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionTest : MonoBehaviour
{
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            player.collection.Save();
            Debug.Log("save");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            player.collection.Load();
            Debug.Log("load");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            player.collection.AddArthropod(player.collection.database.GetArthropod[0], 1);
            Debug.Log("ant");
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            player.collection.AddArthropod(player.collection.database.GetArthropod[1], 1);
            Debug.Log("spider");
        }
    }
}
