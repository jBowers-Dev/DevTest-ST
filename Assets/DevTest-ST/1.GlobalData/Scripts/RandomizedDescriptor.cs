using System.Linq;
using UnityEngine;

namespace JoshBowersDev.DevTestST
{
    /// <summary>
    /// A gameobject that can randomly choose it's name and color based off of global editor lists.
    /// </summary>
    public class RandomizedDescriptor : MonoBehaviour
    {
        // Any name change to these three objects need to be reflected in the RandomizedDescriptorEditor script.
        public AdjectiveListObject AdjectiveListObject;

        public ItemNameListObject ItemNameListObject;

        public ColorListObject ColorListObject;

        public string Description { get; private set; }
        public Color Color { get; private set; }

#if UNITY_EDITOR

        /// <summary>
        /// Editor-only function that generates a new description and color, renaming the gameobject as well.
        /// </summary>
        /// <remarks>This is currently called from the corresponding Editor script, RandomizedDescriptorEditor</remarks>
        public void Generate()
        {
            SetDescription(AdjectiveListObject.GetAdjective(), ItemNameListObject.GetItemName());
            SetColor(ColorListObject.GetColor());
        }

# endif

        private void SetDescription(string adjective, string itemName)
        {
            Description = string.Concat(adjective, " ", itemName);
            string formatted = Description;

            formatted = string.Concat(formatted.Split(' ') // Split the description based on spaces, if the input is already in camelcase then it will simply pass over it.
                .Select((text, index) => index == 0 ? text.ToLower() : char.ToUpper(text[0]) + text.Substring(1).ToLower())); // Our first part must always be lower case, ensure that the following parts are title-case.

            gameObject.name = formatted;
        }

        private void SetColor(Color color)
        {
            Color = color;
        }
    }
}