using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInventory : MonoBehaviour
{
    public SaveState save;
    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_SLOTS;
    public int Y_SPACE_BETWEEN_SLOTS;
    public int COLUMNS;
    Dictionary<InventorySlot, GameObject> arthropodsDisplayed = new Dictionary<InventorySlot, GameObject>();
    void Start()
    {
        CreateDisplay();
        //save = SaveManager.instance.save;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplay();
    }

    public void CreateDisplay()
    {
        for (int i = 0; i < save.Collection.Count; i++)
        {
            var obj = Instantiate(save.Collection[i].arthropodData.arthropodIcon, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            arthropodsDisplayed.Add(save.Collection[i], obj);
        }
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < save.Collection.Count; i++)
        {
            if (!arthropodsDisplayed.ContainsKey(save.Collection[i]))
            {
                var obj = Instantiate(save.Collection[i].arthropodData.arthropodIcon, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                arthropodsDisplayed.Add(save.Collection[i], obj);
            }
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_SLOTS * (i % COLUMNS)), Y_START + (Y_SPACE_BETWEEN_SLOTS * (i / COLUMNS)), 0f);
    }
}
