using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JoshBowersDev.DevTestST.EditorTools
{
    public class DescriptorInspectorWindow : EditorWindow
    {
        private Vector2 _scrollPosition = Vector2.zero;
        private string _searchTerm = string.Empty;

        private static List<RandomizedDescriptor> _allDescriptors = new List<RandomizedDescriptor>();
        private List<RandomizedDescriptor> _currentDescriptors = new List<RandomizedDescriptor>();

        private List<Color> _allColorFilters = new List<Color>();
        private List<Color> _currentColorFilters = new List<Color>();
        private List<bool> _toggles = new List<bool>();
        private GUIContent[] _activeColorFiltersContent = new GUIContent[0];
        private bool _isColorFilterToggled = false;

        private GUIStyle _headerStyle = null;
        private Texture2D _headerBackgroundTexture;

        [MenuItem("DevTest-ST/Global Data/Descriptor Inspector")]
        public static void ShowWindow()
        {
            DescriptorInspectorWindow window = GetWindow<DescriptorInspectorWindow>();
            window.titleContent = new GUIContent("Global Data Inspector");
        }

        private void OnEnable()
        {
            InitStyling();
            _isColorFilterToggled = false;
            _searchTerm = string.Empty;
            UpdateWindow();
        }

        private void InitStyling()
        {
            _headerStyle = new GUIStyle(EditorStyles.largeLabel);
            _headerBackgroundTexture = new Texture2D(1, 1);
            _headerBackgroundTexture.SetPixel(0, 0, Color.black);
            _headerBackgroundTexture.Apply();

            _headerStyle.normal.background = _headerBackgroundTexture;
            _headerStyle.normal.textColor = Color.white;
            _headerStyle.alignment = TextAnchor.MiddleCenter;
            _headerStyle.fontSize = 14;
            _headerStyle.fixedHeight = 20f;
        }

        private void OnGUI()
        {
            // Only call once when needed
            if (_headerStyle == null)
                InitStyling();

            EditorGUILayout.LabelField(new GUIContent("Filtering"), _headerStyle);

            // Create search bar
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Search", "Search for any term"), GUILayout.Width(50));
            _searchTerm = EditorGUILayout.TextField(_searchTerm);

            // Create our buttons
            if (GUILayout.Button("Search"))
            {
                Search();
            }
            if (GUILayout.Button("Update"))
            {
                UpdateWindow();
            }
            EditorGUILayout.EndHorizontal();

            DisplayTextureGrid();
            EditorGUILayout.LabelField(new GUIContent("Descriptors"), _headerStyle);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DisplayDescriptors();
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Performs a search based on partial string match based on <see cref="_searchTerm"/> and whether all or a specific color is selected.
        /// </summary>
        /// <remarks><i>Bug: When deselecting a color, you must click off of the window before the search will update.</remarks>
        private void Search()
        {
            // Reset current descriptors list.
            _currentDescriptors.Clear();

            foreach (var descriptor in _allDescriptors)
            {
                if ((descriptor.Description.Contains(_searchTerm) || string.IsNullOrEmpty(_searchTerm)) && IsColorSelected(descriptor.Color))
                {
                    _currentDescriptors.Add(descriptor);
                }
            }
        }

        // Instead of constantly calling this expensive functionality, leave it up to the user when to update.
        private void UpdateWindow()
        {
            _allDescriptors = FindObjectsOfType<RandomizedDescriptor>().ToList();
            Debug.Log($"Total RandomizedDescriptors: {_allDescriptors.Count}");

            if (!_isColorFilterToggled && string.IsNullOrEmpty(_searchTerm))
            {
                _currentDescriptors = new List<RandomizedDescriptor>(_allDescriptors);
                foreach (var desc in _currentDescriptors)
                {
                    if (string.IsNullOrEmpty(desc.Description))
                        desc.Generate();
                }
            }
            else
            {
                Search();
            }

            _searchTerm = string.Empty;
            UpdateColorSelections();
            InitializeToggles();
            Search();
            Repaint();
        }

        // Initialize the current filter toggles based on how many distinct colors are available in the scene
        private void InitializeToggles()
        {
            _isColorFilterToggled = false;
            _toggles.Clear();
            for (int i = 0; i < _activeColorFiltersContent.Length; i++)
            {
                _toggles.Add(false);
            }
        }

        // Get each of the currently selected descriptors, assign their styling based on the color and display.
        private void DisplayDescriptors()
        {
            int count = _currentDescriptors.Count;
            for (int i = 0; i < count; i++)
            {
                GUIContent content = new GUIContent(_currentDescriptors[i].Description);
                GUIStyle style = new GUIStyle() { normal = { textColor = _currentDescriptors[i].Color } };
                if (GUILayout.Button(content, style))
                {
                    Selection.activeObject = _currentDescriptors[i].gameObject;
                }
                EditorGUILayout.Space(10);
            }
        }

        // Display a simple bool grid with the corresponding color next to each for easier visuals.
        private void DisplayTextureGrid()
        {
            int count = _activeColorFiltersContent.Length;
            int columns = 5;

            _isColorFilterToggled = EditorGUILayout.BeginToggleGroup("Filter by Color", _isColorFilterToggled);

            bool isInHorizontal = false;
            for (int i = 0; i < count; i++)
            {
                // If we've reached a new column
                if (i % columns == 0)
                {
                    // End the previous column
                    if (isInHorizontal)
                    {
                        EditorGUILayout.EndHorizontal();
                        isInHorizontal = false;
                    }
                    // Begin the new one
                    EditorGUILayout.BeginHorizontal();
                    isInHorizontal = true;
                }

                // Drawing the texture toggle
                _toggles[i] = GUILayout.Toggle(_toggles[i], _activeColorFiltersContent[i], GUILayout.Width(50), GUILayout.Height(50));

                // If this toggle was selected, update the selected index and reset other toggles
                if (_toggles[i])
                {
                    if (!_currentColorFilters.Contains(_allColorFilters[i]))
                        _currentColorFilters.Add(_allColorFilters[i]);
                }
                else
                {
                    if (_currentColorFilters.Contains(_allColorFilters[i]))
                        _currentColorFilters.Remove(_allColorFilters[i]);
                }
            }

            if (isInHorizontal)
            {
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndToggleGroup();
        }

        // Gets all active colors and loads them
        private void UpdateColorSelections()
        {
            _allColorFilters = _allDescriptors.Select(x => x.Color).Distinct().ToList(); // Use LINQ to get each unique color, no duplicates.
            _activeColorFiltersContent = LoadColorSelections();
        }

        // Loads a GUIContent array based on all of the colors found in the scene. This is used to provide the distinct visuals for color filtering in the window.
        private GUIContent[] LoadColorSelections()
        {
            int count = _allColorFilters.Count;
            GUIContent[] results = new GUIContent[count];
            for (int i = 0; i < count; i++)
            {
                int texSize = 50;
                Texture2D tex = new Texture2D(texSize, texSize);
                Color fillColor = _allColorFilters[i];
                for (int j = 0; j < texSize * texSize; j++)
                {
                    tex.SetPixel(j % texSize, j / texSize, fillColor);
                }
                tex.Apply();
                results[i] = new GUIContent(tex);
            }
            return results;
        }

        // Returns whether color filtering is turned on with more than one selected, and if the input color matches anything in the list, otherwise return true if one or the other parameter is not valid.
        private bool IsColorSelected(Color color)
        {
            return (_currentColorFilters.Count > 0 && _isColorFilterToggled) ? _currentColorFilters.Contains(color) : true;
        }
    }
}