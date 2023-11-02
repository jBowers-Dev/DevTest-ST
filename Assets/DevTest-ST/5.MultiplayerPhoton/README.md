## Network Interaction System Overview

A networked interaction system that enables seamless, synchronized control over interactive elements across the network.

### NetworkManager Class

`NetworkManager` is a singleton class responsible for establishing and maintaining the connection to Photon.

#### Features:
- Initializes the connection to the Photon Network.
- Automatically creates or joins a room for players.
- Will close the application if disconnected.

### NetworkInteractable Class

`NetworkInteractable` is a class ensuring that the interactive object are network-synchronized and that their control is seamless across different connected clients. It delegates the responsibility of handling interactions to the `IInteractable` and `IInteractableHelper` components.

#### Features:
- Maintains network synchronization of interactions.
- Controls ownership of interactive elements.
- Handles networked interaction start, update, and end events.

### INetworkInteractable Interface

`INetworkInteractable` is an interface providing a blueprint for networked interactable objects, allowing different types to be handled in a unified manner.

#### Methods:
- `OnBeginInteraction()`: Called when interaction starts.
- `OnUpdateInteraction(object value)`: Called during interaction updates.
- `OnEndInteraction()`: Called when interaction ends.
- `OnSyncIsInteractable(bool val)`: Synchronizes whether this object is currently interactable or not.
- `OnSyncNetworkUpdate(object value)`: Synchronizes interaction updates across the network.

### Usage Workflow

1. **Initialization**: `NetworkManager` initializes the connection to Photon.
2. **Room Creation/Joining**: Players automatically create or join a room named "DialMania".
3. **Interaction Handling**: `NetworkInteractable` objects handle interaction events and synchronize them over the network.

### Example Usage

Make sure to incorporate the `NetworkManager` into your scene and attach `NetworkInteractable` to `IInteractable` objects that should be interactable across the network. New network interactable classes should also implement the `INetworkInteractable` interface and handle the interaction events appropriately for networked interaction.