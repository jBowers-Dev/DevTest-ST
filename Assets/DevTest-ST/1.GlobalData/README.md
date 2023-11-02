# Global Data & Editor Tools

This feature allows for the creation and management of global list ScriptableObjects, accessible by any runtime script and editable within the editor, and the ability to manipulate and filter GameObjects based on the data provided in the selected lists.

## Features

### 1. Global Lists

- I chose to utilize ScriptableObjects for each of these lists for the following reasons:
    - **Accessibility:** Gives any script easy access to data, makes data easy to manipulate in-editor.
    - **Scalability**: Allows for scalability with multiple versions of each list
    - **Single Responsibility**: No extra functionality, simply contains the required lists.
- One exception is for the ColorList SO's, where they needed an Editor script to verify color requirements
	- **ColorListObjectEditor**:
		- Displays the serialized Color List
		- Checks for new color updates and ensures that they're valid according to requirements.
			- If a color is invalid, it will display a warning label with a list of each color that is invalid. This helps with visibility for the user, but further measures can easily be added if needed.

### 2. MonoBehaviour - `RandomizedDescriptor`

In keeping with the _**Single Responsibility Principle**_, this mono simply holds the required data and gives the required Generate() function. This also aligns with the _**Open-Close Principle**_, as we can derive from this class in the case of further functionality that would be needed.

- Since our data is read-only outside of the mono, mutators were needed and the Description needed some extra functionality.
	- _**SetDescription(string,string)**_
		- Sets the Description with a space in between the adjective and item name
		- Splits the Description based on 'space', then uses the Select LINQ query to make sure the first word is lower case, then makes sure the subsequent elements are title-case. By splitting them using 'space', this also removes them so that it is true lower camel case. 

### 3. GameObjects

- 20 gameobjects are created in the scene.

### 4. Editor Tool - `DescriptorInspectorWindow`

- Displays a list of all GameObjects in the scene that have the `RandomizedDescriptor` MonoBehaviour. 
- **Functionality**:
    - **Display**: Lists the name of each GameObject using the color specified in its `RandomizedDescriptor`.
	    - Simply loops through each currently selected Descriptors, display's their name in the color they're set to, and adds a button.
    - **Search Filter**: Allows filtering of the list based on partial name matching.
	    - This checks against the partial name match **and** the current **Color Filter**, if applicable.
    - **Color Filter**: Provides the ability to filter GameObjects based on their associated color.
	    - Visualized by creating a toggle group where each toggle uses a basic color texture to represent the filter choice. If the toggle group is set to false or no toggles are selected, then it will always return true.
    - **Selection**: Clicking on an item in the list will select the corresponding GameObject in the Unity Editor.

## How To Use

1. Open the Unity Editor and navigate to `SpecularTheory -> Global Data -> Descriptor Inspector`.
2. In the opened window, you can view all the GameObjects with the `RandomizedDescriptor` MonoBehaviour.
3. Use the search bar to filter GameObjects based on partial name matching.
4. Use the color grid to filter GameObjects based on their associated color.
5. Click on a GameObject's name in the list to select it in the Unity Editor.