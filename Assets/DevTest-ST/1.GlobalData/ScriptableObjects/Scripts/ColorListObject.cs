using System.Collections.Generic;
using UnityEngine;

namespace JoshBowersDev.DevTestST
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Color List", menuName = "Global Data/Color List")]
    public class ColorListObject : ScriptableObject
    {
        public List<Color> ColorList = new List<Color>();

        /// <summary>
        /// Returns a random color from the preset list.
        /// </summary>
        /// <returns></returns>
        public Color GetColor()
        {
            return ColorList[Mathf.FloorToInt(Random.Range(0, ColorList.Count))];
        }
    }
}