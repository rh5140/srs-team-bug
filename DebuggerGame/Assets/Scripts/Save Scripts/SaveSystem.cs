using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Save State", menuName = "Save System/Save State")]
public class SaveSystem : ScriptableObject, ISerializationCallbackReceiver
{
    public string savePath;

    //Level info
    public int currentLevel;
    //public levelDatabase levelDatabase;
    //public List<Level> completedLevels = new List<Level>();

    //Map info
    public Vector2Int mapPosition;

    //Collection info
    public ArthropodDatabase arthropodDatabase;
    public List<InventorySlot> Collection = new List<InventorySlot>();

    private void OnEnable()
    {
#if UNITY_EDITOR
        arthropodDatabase = (ArthropodDatabase)AssetDatabase.LoadAssetAtPath("Assets/Resources/Arthropod Database.asset", typeof(ArthropodDatabase));
#else
        arthropodDatabase = Resources.Load<ArthropodDatabase>("Arthropod Database");
#endif
    }
    public void AddArthropod(ArthropodData _arthropodData, int _amount)
    {
        for (int i = 0; i < Collection.Count; i++)
        {
            if (Collection[i].arthropodData == _arthropodData)
            {
                Collection[i].addAmount(_amount);
                return;
            }
        }
        Collection.Add(new InventorySlot(arthropodDatabase.GetId[_arthropodData], _arthropodData, _amount));
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
        for (int i = 0; i < Collection.Count; i++)
        {
            Collection[i].arthropodData = arthropodDatabase.GetArthropod[Collection[i].ID];
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