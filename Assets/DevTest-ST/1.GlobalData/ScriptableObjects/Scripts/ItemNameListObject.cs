using System.Collections.Generic;
using UnityEngine;

namespace JoshBowersDev.DevTestST
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Item Name List", menuName = "Global Data/Item Name List")]
    public class ItemNameListObject : ScriptableObject
    {
        public List<string> ItemNameList = new List<string>();

        /// <summary>
        /// Returns a random ItemName form the preset list.
        /// </summary>
        /// <returns></returns>
        public string GetItemName()
        {
            return ItemNameList[Mathf.FloorToInt(Random.Range(0, ItemNameList.Count))];
        }
    }
}