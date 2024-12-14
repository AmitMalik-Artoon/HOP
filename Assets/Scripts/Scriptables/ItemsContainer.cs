using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "ItemsContainer", menuName = "Scriptable/ItemsContainer")]
internal class ItemsContainer : ScriptableObject
{
    public List<ItemInfo> commonItemsList = new List<ItemInfo>();
    public List<ItemInfo> rareItemsList = new List<ItemInfo>();
    public List<ItemInfo> epicItemsList = new List<ItemInfo>();
}

[System.Serializable]
internal class ItemInfo
{
    public Item prefab;
    public int score;
}
