using Photon.Pun;
using System;
using UnityEngine.InputSystem;

namespace JoshBowersDev.DevTestST.AdvancedInteraction
{
    public interface IInteractable
    {
        Action BeginInteraction { get; set; }
        Action<object> UpdateInteraction { get; set; }
        Action EndInteraction { get; set; }

        bool IsInteracting { get; }

        void SetInteracting(bool val) { }

        bool IsInteractable { get; }

        void SetInteractable(bool val) { }

        void OnBeginInteraction(InputAction.CallbackContext context) { }

        void OnEndInteraction(InputAction.CallbackContext context) { }

        void ResetControl() { }

        void UpdateControl(InputAction.CallbackContext context) { }

        [PunRPC]
        void ManualControl(object val) { }
    }
}