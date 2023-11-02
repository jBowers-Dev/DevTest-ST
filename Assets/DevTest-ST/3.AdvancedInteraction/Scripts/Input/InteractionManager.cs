using UnityEngine;
using UnityEngine.InputSystem;

namespace JoshBowersDev.DevTestST.AdvancedInteraction
{
    public class InteractionManager : MonoBehaviour
    {
        private PlayerControls _playerControls;

        private IInteractable _previousControl;

        private void Awake()
        {
            _playerControls = new PlayerControls();

            _playerControls.Controls.Interacting.started += OnBeginInteraction;
            _playerControls.Controls.Interacting.canceled += OnEndInteraction;
            _playerControls.Controls.WorldPosition.performed += UpdateControl;
            _playerControls.Controls.Enable();
        }

        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }

        private void OnBeginInteraction(InputAction.CallbackContext context)
        {
            Vector2 pointerPosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // Make sure our hit object has the ControlBase, then alert it
                IInteractable control = hitInfo.collider.GetComponent<IInteractable>();
                if (control != null)
                {
                    control.OnBeginInteraction(context);
                    _previousControl = control;
                }
            }
        }

        private void OnEndInteraction(InputAction.CallbackContext context)
        {
            if (_previousControl != null)
                _previousControl.OnEndInteraction(context);
        }

        private void UpdateControl(InputAction.CallbackContext context)
        {
            Vector2 pointerPosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // Make sure our hit object has the ControlBase, then alert it
                IInteractable control = hitInfo.collider.GetComponent<IInteractable>();
                if (control != null)
                {
                    if (_previousControl != control)
                        _previousControl = control;

                    control.UpdateControl(context);
                }
            }
        }
    }
}