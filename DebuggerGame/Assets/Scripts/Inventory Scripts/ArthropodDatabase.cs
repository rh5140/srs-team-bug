using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Arthropod Database", menuName = "Inventory System/Arthropod Database")]
public class ArthropodDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public ArthropodData[] Arthropods;
    public Dictionary<ArthropodData, int> GetId = new Dictionary<ArthropodData, int>();
    public Dictionary<int, ArthropodData> GetArthropod = new Dictionary<int, ArthropodData>();

    public void OnAfterDeserialize()
    {
        GetId = new Dictionary<ArthropodData, int>();
        GetArthropod = new Dictionary<int, ArthropodData>();
        for (int i = 0; i < Arthropods.Length; i++)
        {
            GetId.Add(Arthropods[i], i);
            GetArthropod.Add(i, Arthropods[i]);
        }
    }

    public void OnBeforeSerialize()
    {
    }
}
