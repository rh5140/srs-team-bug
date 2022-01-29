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
    public BugDatabaseObject database;
    public List<InventorySlot> Container = new List<InventorySlot>();

    private void OnEnable()
    {
#if UNITY_EDITOR
        database = (BugDatabaseObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Bug Database.asset", typeof(BugDatabaseObject));
#else
        database = Resources.Load<BugDatabaseObject>("Bug Database");
#endif
    }
    public void AddBug(BugData _bugdata, int _amount)
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].bugdata == _bugdata)
            {
                Container[i].addAmount(_amount);
                return;
            }
        }
        Container.Add(new InventorySlot(database.GetId[_bugdata], _bugdata, _amount));
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
            Container[i].bugdata = database.GetBug[Container[i].ID];
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public int ID;
    public BugData bugdata;
    public int amount;
    public InventorySlot(int _id, BugData _bugdata, int _amount)
    {
        ID = _id;
        bugdata = _bugdata;
        amount = _amount;
    }

    public void addAmount(int val)
    {
        amount += val;
    }
}