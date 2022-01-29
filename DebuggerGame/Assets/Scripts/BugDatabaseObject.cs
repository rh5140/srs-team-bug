using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bug Database", menuName = "Inventory System/Bug Database")]
public class BugDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public BugData[] Bugs;
    public Dictionary<BugData, int> GetId = new Dictionary<BugData, int>();
    public Dictionary<int, BugData> GetBug = new Dictionary<int, BugData>();

    public void OnAfterDeserialize()
    {
        GetId = new Dictionary<BugData, int>();
        GetBug = new Dictionary<int, BugData>();
        for (int i = 0; i < Bugs.Length; i++)
        {
            GetId.Add(Bugs[i], i);
            GetBug.Add(i, Bugs[i]);
        }
    }

    public void OnBeforeSerialize()
    {
    }
}
