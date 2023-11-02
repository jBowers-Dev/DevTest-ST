using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoshBowersDev.DevTestST.AdvancedInteraction
{
    public class DialControl : ControlBase<int>
    {
        public Transform Knob;

        [Tooltip("The actual euler angles, if restriction is needed.")]
        public float LeftLimit, RightLimit;

        [Tooltip("The actual min/max values for the dial.")]
        public int MinVal, MaxVal;

        [Tooltip("Determines which axis to rotate.")]
        public Vector3 AxisRotation;

        private Quaternion _startRot;
        private float _rotAngle = 0;

        #region Unity Callbacks

        private void Start()
        {
            if (Knob == null)
            {
                Debug.LogWarning("Knob has not been assigned, using script's transform.", this);
                Knob = GetComponent<Transform>();
                if (Knob == null)
                {
                    Debug.LogError("Knob assignment failed. Disabling script.", this);
                    enabled = false;
                }
            }

            _startRot = Knob.rotation;

            SetInteractable(true);
        }

        #endregion Unity Callbacks

        #region Override Methods

        public override void OnBeginInteraction(InputAction.CallbackContext context)
        {
            base.OnBeginInteraction(context);
            if (!IsInteracting)
            {
                _startRot = Knob.localRotation;
                Vector3 vector = GetDistanceFromInteractable(Mouse.current.position.ReadValue()); // Can be expanded to include VR controllers or any other InputControl's.
                // Special thanks to https://discussions.unity.com/t/rotate-a-dial-with-circular-motions/80190/3
                // Once we have our world positions and vector difference, calculate the angle in degrees from our starting point.
                _rotAngle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
                _rotAngle -= Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;
                SetInteracting(true);
            }
        }

        public override void OnEndInteraction(InputAction.CallbackContext context)
        {
            SetInteracting(false);
            base.OnEndInteraction(context);
        }

        public override void UpdateControl(InputAction.CallbackContext context)
        {
            base.UpdateControl(context);

            if (!IsInteractable || !IsInteracting)
            {
                return;
            }
            else
            {
                Vector3 vector;

                if (context.valueType == typeof(Vector3))
                    vector = context.ReadValue<Vector3>();
                else
                    vector = context.ReadValue<Vector2>();

                vector = GetDistanceFromInteractable(vector);
                float angle = GetCorrectAngle(vector);
                Quaternion deltaRotation = Quaternion.Euler(0, 0, angle);

                // Multiply our start rotation by the new delta rotation, and get the euler angles for clamping.
                Vector3 eulerRotation = (_startRot * deltaRotation).eulerAngles;

                // Alert anyone listening that the control has been updated
                UpdateInteraction?.Invoke(eulerRotation);
                SetCurrentLocalRotation(eulerRotation);
            }
        }

        #endregion Override Methods

        #region Public Methods

        /// <summary>
        /// Gives manual control over turning the dial
        /// </summary>
        /// <param name="val"></param>
        public override void ManualControl(object val)
        {
            Vector3 euler;

            try
            {
                euler = (Vector3)val;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to cast input value to Vector3 in ManualControl: {e.Message}", this);
                return;
            }

            SetCurrentLocalRotation(euler);
        }

        #endregion Public Methods

        #region Private Methods

        private void SetCurrentLocalRotation(Vector3 val)
        {
            // Ensure the angle is in the correct range
            float clampedZ = ClampAngle(val.z, LeftLimit, RightLimit);
            Knob.localRotation = Quaternion.Euler(val.x, val.y, clampedZ);
        }

        private Vector3 GetDistanceFromInteractable(Vector3 val)
        {
            Vector3 screenPos = Camera.main?.WorldToScreenPoint(Knob.position) ?? Vector3.zero;
            return (Vector3)val - screenPos;
        }

        private float GetCorrectAngle(Vector3 val)
        {
            float angle = Mathf.Atan2(val.y, val.x) * Mathf.Rad2Deg;
            angle -= _rotAngle;

            // Normalize the angle to the range 0,360
            // https://www.reddit.com/r/Unity3D/comments/buvpd4/clamp_a_negative_angle/
            angle = (angle + 360) % 360;

            if (angle > 180) // If angle is greater than 180, map it between [-180, 180)
                angle -= 360;

            return (angle * -1); // Invert the angle for correct rotation
        }

        // Utility method to clamp angles considering negative values
        private float ClampAngle(float angle, float min, float max)
        {
            // Normalize angle to 0,360 range
            angle = (angle + 360) % 360;
            // Convert min and max to 0,360 range for comparison
            float normalizedMin = (min + 360) % 360;
            float normalizedMax = (max + 360) % 360;

            // If min is greater than max, we assume the clamp is wrapping around 360
            if (normalizedMin > normalizedMax)
            {
                // Figure out if we're closest to the min or max
                if (angle > normalizedMax && angle < normalizedMin)
                    return angle < (normalizedMin + normalizedMax) / 2f ? normalizedMax : normalizedMin;
            }
            // Otherwise, simply return the clamped value
            else
            {
                return Mathf.Clamp(angle, normalizedMin, normalizedMax);
            }

            // All good, return initial value.
            return angle;
        }

        #endregion Private Methods
    }
}