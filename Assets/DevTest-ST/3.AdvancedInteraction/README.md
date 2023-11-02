# Dial Control and Interaction System

This system provides a highly customizable and interactive Dial Control for Unity projects. The system allows developers to define dial positions and values, with support for free, snapped, and momentary positions. Currently this systems utilizes the Unity.InputSystem, and can easily be converted to receive interactions from any other input type, not just mouse control.

## Features
- Define a range of values with free rotation.
- Set snapped positions.
- Create momentary snapped positions that automatically revert to the previous position.
- Easily assign/retrieve values or IDs. *(These values are serialized for editor updates as well)*
- Events are triggered when values or positions change, as well as when an interaction begins/updates/ends.
- Supports the creation and interaction with multiple dials, each with unique values, IDs, and positions. 
## Usage

### 1. **ControlBase Class:**
- This is a generic base class designed to represent an abstract interactive control with a given type `T` value.
	- Since our interactables need to give out different types, we pass a generic value for our 'value', allowing for better scalability.
- The value ID (`_valueID`) is a unique identifier.
- Getters and setters for `_valueID` and the `_value_` are provided, .
- Events for starting, updating, and ending interaction, along with value changes, are exposed.
	- This gives access for any 'helper' classes that may need this generic information from a derived class.
  
### 2. **DialControl Class:**
- Inherits from `ControlBase<int>`.
- Allows you to assign a `Transform` to the `Knob`.
	- This gives more flexibility, instead of constraining it to the object being turned
- Define the rotation limits, value range, and axis of rotation for the dial.
- Tracks the starting rotation and the current rotation angle.
- Handles interaction events to compute and set the dial's rotation and value during interaction.

### 3. **DialSnapper Class:**
- Works with `DialControl` to snap the dial to defined positions.
- Define a starting position and snap speed, along with a list of dial positions.
- Dial positions include a name, angle, flags for 'free' and 'momentary', and a momentary delay.
- The class computes the closest dial position and handles snapping with optional delay for momentary positions.
	- *Currently, this must be set using Debug editor rotation values.*

### Methods
- `SetValueID(string val)`: Set the Value ID.
- `SetValue(float val)`: Set the Value.
- `string GetValueID()`: Retrieve the Value ID.
- `float GetValue()`: Retrieve the Value.

## Interaction Management System

The `InteractionManager` class and its associated interfaces serve as the cornerstone of the entire interaction system, managing the user’s input and directing it to the appropriate interactive  elements.

### InteractionManager Class

`InteractionManager` is a class responsible for detecting user inputs and forwarding interaction events to the appropriate `IInteractable` objects. It abstractly defines how interaction begins, ends, and updates, without relying on the specifics of the interactive elements themselves.

#### Features:
- Initializes and handles user input controls.
- Detects interactions and forwards them to interactable elements within the scene.
- Maintains reference to the previously interacted control.

### IInteractable Interface

`IInteractable` is an interface that provides a contract for all interactable elements, keeping consistent with our abstraction.

- Defines a set of functionalities that every interactable object must implement, providing easier generic access.
  
- Allows for multiple types of interactable objects to be interacted with.

#### Methods:
- `OnBeginInteraction(InputAction.CallbackContext context)`: Triggered when interaction starts, passing the control type (in this case, the LMB) and the type of interaction (Hold, multi-tap etc).
- `OnEndInteraction(InputAction.CallbackContext context)`: Triggered when interaction ends.
- `UpdateControl(InputAction.CallbackContext context)`: Updates during interaction and passes the world position of the controller.

### IInteractableHelper Interface

`IInteractableHelper` is an additional interface for helper classes that augment the functionality of `IInteractable` objects.

- `IInteractableHelper` offers an extension point for adding more features to interactable objects without altering their basic contract.

#### Methods:
- `OnUpdateInteraction(object val)`: Triggered during interaction updates. 
- `OnInteractableUpdated(bool val)`: Updates interactable status.

## Custom Dial Asset
For visibility's sake, I mocked up a quick prototype dial in Blender and Substance Painter, exporting different detail textures for the varying line placements.

![Dial Substance Image](/_Images/Dial_Substance.png)
