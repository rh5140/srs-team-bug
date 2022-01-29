using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bug Object", menuName = "Inventory System/Bug")]
public class BugData : ScriptableObject //BugData is ScriptableObject associated with bug BoardObject
{
    public BoardObject bug; 
    public GameObject bugIcon; 
    public string bugName;
    [TextArea(15,20)]
    public string description;
}
