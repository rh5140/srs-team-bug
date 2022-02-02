using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInventory : MonoBehaviour
{
    public InventorySystem inventory;
    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_SLOTS;
    public int Y_SPACE_BETWEEN_SLOTS;
    public int COLUMNS;
    Dictionary<InventorySlot, GameObject> arthropodsDisplayed = new Dictionary<InventorySlot, GameObject>();
    void Start()
    {
        CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplay();
    }

    public void CreateDisplay()
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            var obj = Instantiate(inventory.Container[i].arthropodData.arthropodIcon, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            arthropodsDisplayed.Add(inventory.Container[i], obj);
        }
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            if (!arthropodsDisplayed.ContainsKey(inventory.Container[i]))
            {
                var obj = Instantiate(inventory.Container[i].arthropodData.arthropodIcon, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                arthropodsDisplayed.Add(inventory.Container[i], obj);
            }
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_SLOTS * (i % COLUMNS)), Y_START + (Y_SPACE_BETWEEN_SLOTS * (i / COLUMNS)), 0f);
    }
}
