using System.Collections.Generic;
using UnityEngine;

namespace JoshBowersDev.DevTestST
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Adjective List", menuName = "Global Data/Adjective List")]
    public class AdjectiveListObject : ScriptableObject
    {
        public List<string> AdjectiveList = new List<string>();

        /// <summary>
        /// Returns a random adjective from the preset list.
        /// </summary>
        /// <returns></returns>
        public string GetAdjective()
        {
            return AdjectiveList[Mathf.FloorToInt(Random.Range(0, AdjectiveList.Count))];
        }
    }
}