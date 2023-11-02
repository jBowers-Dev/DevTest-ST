using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JoshBowersDev.DevTestST.EditorTools
{
    [CustomEditor(typeof(ColorListObject))]
    public class ColorListObjectEditor : Editor
    {
        /// <summary>
        /// Reference to <see cref="ColorListObject"/> being inspected, which allows for undo operations and automatic UI updates.
        /// </summary>
        private SerializedProperty colorListProperty;

        private void OnEnable()
        {
            colorListProperty = serializedObject.FindProperty("ColorList");
        }

        public override void OnInspectorGUI()
        {
            ColorListObject colorList = (ColorListObject)target;

            EditorGUILayout.PropertyField(colorListProperty, new GUIContent("Color List"));

            // Make a list of all known invalid colors.
            List<int> invalidList = new List<int>();
            for (int i = 0; i < colorList.ColorList.Count; i++)
            {
                if (!IsValidColor(colorList.ColorList[i]))
                {
                    invalidList.Add(i);
                }
            }

            // If we have one or more colors, display a warning box about each one that needs to be fixed.
            if (invalidList.Count > 0)
            {
                string invalids = "";
                for (int i = 0; i < invalidList.Count; i++)
                {
                    invalids += string.Concat(invalidList[i], "\n");
                }
                EditorGUILayout.HelpBox($"Incorrect color on the following, remember that alpha must always be 100%, no “grays” or “reds”\n {invalids}", MessageType.Warning);
            }
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        /// <summary>
        /// Ensure that alpha must always be 100%, no 'grays' or 'reds'
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Whether the color is valid based on constraints.</returns>
        /// <remarks>What constitutes 'red' is subjective, simply alter the values in the third 'if' statement if needed.</remarks>
        private bool IsValidColor(Color color)
        {
            if (color.a != 1) return false;

            if (color.r == color.g && color.g == color.b) return false;

            if (color.r > 0.8f && color.g < 0.5f && color.b < 0.5f) return false;

            return true;
        }
    }
}