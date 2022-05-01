using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Arthropod Object", menuName = "Inventory System/Arthropod")]
public class ArthropodData : ScriptableObject
{
    public Arthropod arthropod; 
    public GameObject arthropodIcon; 
    public string arthropodName;
    [TextArea(15,20)]
    public string description;
}
