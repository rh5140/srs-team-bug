using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "New Save State", menuName = "Save System/Save State")]
public class SaveState : ScriptableObject, ISerializationCallbackReceiver
{
    public string savePath;

    [SerializeField]
    //Level info
    public string currentLevel;
    public HashSet<string> unlockedLevels = new HashSet<string>();
    public List<string> unlockedLevelsSerializable = new List<string>();

    public HashSet<string> unlockedCharacters = new HashSet<string>();
    public List<string> unlockedCharactersSerializable = new List<string>();

    public HashSet<string> newestCharacterUnlocks = new HashSet<string>();
    public List<string> newestCharacterUnlocksSerializable = new List<string>();

    //public levelDatabase levelDatabase;
    public bool saveCreated;

    //Map info
    public Vector2Int mapPosition;

    //Collection info
    public ArthropodDatabase arthropodDatabase;
    public List<InventorySlot> Collection = new List<InventorySlot>();
    private void OnEnable()
    {
        //unlock first level
        unlockedLevels.Add("0000");
        hideFlags = HideFlags.DontUnloadUnusedAsset;
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
        unlockedLevelsSerializable = unlockedLevels.ToList();
        unlockedCharactersSerializable = unlockedCharacters.ToList();
        newestCharacterUnlocksSerializable = newestCharacterUnlocks.ToList();
        string saveData = JsonUtility.ToJson(this, true);
        Debug.Log(saveData);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        bf.Serialize(file, saveData);
        file.Close();
        unlockedCharactersSerializable.Clear();
        unlockedLevelsSerializable.Clear();
        newestCharacterUnlocksSerializable.Clear();
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

    public void ClearNewestCharacterUnlocks()
    {
        newestCharacterUnlocks = new HashSet<string>();
        Save();
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

        foreach (string level in unlockedLevelsSerializable)
        {
            unlockedLevels.Add(level);
        }

        foreach (string character in unlockedCharactersSerializable)
        {
            unlockedCharacters.Add(character);
        }

        foreach (string newCharacter in newestCharacterUnlocksSerializable)
        {
            newestCharacterUnlocks.Add(newCharacter);
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