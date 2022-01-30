using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : BoardObject
{
    public InventorySystem collection;
    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("save");
            collection.Save();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("load");
            collection.Load();
        }

        //Temporary to add bugs to collection
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("ant added");
            collection.AddBug(collection.database.GetBug[0], 1);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("spider added");
            collection.AddBug(collection.database.GetBug[1], 1);
        }
    }

    private void OnApplicationQuit()
    {
        collection.Container.Clear();
    }
}
