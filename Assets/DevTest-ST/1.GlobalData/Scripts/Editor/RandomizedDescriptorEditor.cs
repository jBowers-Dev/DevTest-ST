using UnityEditor;
using UnityEngine;

namespace JoshBowersDev.DevTestST.EditorTools
{
    [CustomEditor(typeof(RandomizedDescriptor))]
    [CanEditMultipleObjects]
    public class RandomizedDescriptorEditor : Editor
    {
        // Boolean flags to check if the properties in RandomizedDescriptor objects are null.
        private bool _isAdjectiveObjectNull = true;

        private bool _isItemNameObjectNull = true;
        private bool _isColorListObjectNull = true;

        // References to the SO's on the RandomizedDescriptor being inspected, which allows for undo operations and automatic UI updates.
        private SerializedProperty _adjectiveListProperty;

        private SerializedProperty _itemNameProperty;
        private SerializedProperty _colorListProperty;

        private void OnEnable()
        {
            // Retrieve references to the properties of the RandomizedDescriptor being inspected.
            _adjectiveListProperty = serializedObject.FindProperty("AdjectiveListObject");
            _itemNameProperty = serializedObject.FindProperty("ItemNameListObject");
            _colorListProperty = serializedObject.FindProperty("ColorListObject");
        }

        public override void OnInspectorGUI()
        {
            RandomizedDescriptor descriptor = (RandomizedDescriptor)target;

            // Check to make sure each needed asset is present, otherwise warn the user with a Warning Box.
            _isAdjectiveObjectNull = descriptor.AdjectiveListObject == null;
            if (_isAdjectiveObjectNull)
            {
                EditorGUILayout.HelpBox("You must assign an Adjective List!", MessageType.Warning);
            }
            EditorGUILayout.PropertyField(_adjectiveListProperty, new GUIContent("Adjective List"));

            EditorGUILayout.Space(5);

            _isItemNameObjectNull = descriptor.ItemNameListObject == null;
            if (_isItemNameObjectNull)
            {
                EditorGUILayout.HelpBox("You must assign an Item Name List!", MessageType.Warning);
            }
            EditorGUILayout.PropertyField(_itemNameProperty, new GUIContent("Item Name List"));

            EditorGUILayout.Space(5);

            _isColorListObjectNull = descriptor.ColorListObject == null;
            if (_isColorListObjectNull)
            {
                EditorGUILayout.HelpBox("You must assign a Color List!", MessageType.Warning);
            }
            EditorGUILayout.PropertyField(_colorListProperty, new GUIContent("Color List"));

            EditorGUILayout.Space(5);

            if (!_isAdjectiveObjectNull && !_isItemNameObjectNull && !_isColorListObjectNull)
            {
                // Make sure when we hit generate, that each asset has at least one element in their list.
                if (GUILayout.Button("Generate"))
                {
                    // When the Generate button is pressed, check if all properties have at least one element, then call the Generate method of the descriptor.
                    // Otherwise, display an error dialog.
                    if (descriptor.AdjectiveListObject.AdjectiveList.Count > 0
                    && descriptor.ItemNameListObject.ItemNameList.Count > 0
                    && descriptor.ColorListObject.ColorList.Count > 0
                    )
                    {
                        descriptor.Generate();
                        EditorUtility.SetDirty(descriptor);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error while generating", "One or more lists are empty, please check data assets.", "Ok");
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}