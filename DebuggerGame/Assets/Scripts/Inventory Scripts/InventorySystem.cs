using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventorySystem : ScriptableObject, ISerializationCallbackReceiver
{
    public string savePath;
    public ArthropodDatabase database;
    public List<InventorySlot> Container = new List<InventorySlot>();

    private void OnEnable()
    {
#if UNITY_EDITOR
        database = (ArthropodDatabase)AssetDatabase.LoadAssetAtPath("Assets/Resources/Arthropod Database.asset", typeof(ArthropodDatabase));
#else
        database = Resources.Load<ArthropodDatabase>("Arthropod Database");
#endif
    }
    public void AddArthropod(ArthropodData _arthropodData, int _amount)
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].arthropodData == _arthropodData)
            {
                Container[i].addAmount(_amount);
                return;
            }
        }
        Container.Add(new InventorySlot(database.GetId[_arthropodData], _arthropodData, _amount));
    }

    public void Save()
    {
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        bf.Serialize(file, saveData);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < Container.Count; i++)
        {
            Container[i].arthropodData = database.GetArthropod[Container[i].ID];
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public int ID;
    public ArthropodData arthropodData;
    public int amount;
    public InventorySlot(int _id, ArthropodData _arthropodData, int _amount)
    {
        ID = _id;
        arthropodData = _arthropodData;
        amount = _amount;
    }

    public void addAmount(int val)
    {
        amount += val;
    }
}