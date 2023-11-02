using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoshBowersDev.DevTestST.AdvancedInteraction
{
    public class ControlBase<T> : MonoBehaviour, IInteractable
    {
        #region Events

        public Action BeginInteraction { get; set; }
        public Action<object> UpdateInteraction { get; set; }
        public Action EndInteraction { get; set; }

        public Action<string> OnValueIDChanged;
        public Action<T> ValueChanged;

        #endregion Events

        #region Variables

        public PlayerControls PlayerControls { get; private set; }
        public bool IsInteractable { get; private set; }
        public bool IsInteracting { get; private set; }

        [SerializeField]
        private string _valueID;

        [SerializeField]
        private T _value;

        #endregion Variables

        #region Base Methods

        public string GetValueID()
        { return _valueID; }

        public void SetValueID(string val)
        {
            if (!string.IsNullOrEmpty(val))
                _valueID = val;
            else
                Debug.LogError("ValueID cannot be empty.");
            OnValueIDChanged?.Invoke(val);
        }

        public T GetValue()
        { return (T)_value; }

        public void SetValue(T val)
        {
            _value = val;
            ValueChanged?.Invoke(_value);
        }

        public virtual bool GetInteractable()
        { return IsInteractable; }

        public virtual void SetInteractable(bool val)
        { IsInteractable = val; }

        public virtual bool Interacting => IsInteracting;

        public virtual void SetInteracting(bool val)
        { IsInteracting = val; }

        #endregion Base Methods

        #region Virtual Methods

        public virtual void OnBeginInteraction(InputAction.CallbackContext context)
        {
            BeginInteraction?.Invoke();
        }

        public virtual void OnEndInteraction(InputAction.CallbackContext context)
        {
            EndInteraction?.Invoke();
        }

        public virtual void ResetControl()
        { }

        public virtual void UpdateControl(InputAction.CallbackContext context)
        { }

        public virtual void ManualControl(object val)
        { }

        #endregion Virtual Methods
    }
}