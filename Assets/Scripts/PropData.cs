using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropData:ScriptableObject
{
    public List<PropItem> propItemList = new List<PropItem>();
}
[System.Serializable]
public class PropItem
{
    public int prop_id;
    public string prop_name;
    public int prop_level;
    public Sprite prop_sprite;
    public string prop_des;
}
