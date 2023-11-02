using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace JoshBowersDev.DevTestST.AdvancedInteraction
{
    [System.Serializable]
    public struct DialPosition
    {
        public string positionName;

        [Tooltip("As of this writing, you must set the inspector to 'Debug' mode when getting the correct z value for the angle")]
        public float angle;

        [Tooltip("If true, then the dial will not snap to this position if it's closest.")]
        public bool isFree;

        [Tooltip("If true, then the dial will snap back to the previous position after the momentary delay")]
        public bool isMomentary;

        public float momentaryDelay;

        public bool Equals(DialPosition other)
        {
            return positionName == other.positionName && angle == other.angle &&
                    isFree == other.isFree && isMomentary == other.isMomentary &&
                    momentaryDelay == other.momentaryDelay;
        }
    }

    [RequireComponent(typeof(DialControl))]
    public class DialSnapper : MonoBehaviour, IInteractableHelper
    {
        [Tooltip("Sets the starting position on Start, leave empty if not needed.")]
        [SerializeField]
        private string _startingPosition;

        [SerializeField]
        private float _snapSpeed;

        [SerializeField]
        private Ease _snapEase = Ease.Linear;

        [SerializeField]
        private List<DialPosition> _dialPositions;

        private DialPosition _currentDialPosition;
        private DialControl _dialControl;

        [SerializeField]
        private bool _isSnapped = false;

        [SerializeField]
        private bool _isSnapping = false;

        private void OnEnable()
        {
            if (_dialControl == null)
                _dialControl = GetComponent<DialControl>();

            if (_dialControl != null)
            {
                _dialControl.BeginInteraction += OnBeginInteraction;
                _dialControl.UpdateInteraction += OnUpdateInteraction;
                _dialControl.EndInteraction += OnEndInteraction;
            }
            else
                Debug.LogError("Dial Control was not assigned OnEnable.");
        }

        private void OnDisable()
        {
            if (_dialControl != null)
            {
                _dialControl.BeginInteraction -= OnBeginInteraction;
                _dialControl.UpdateInteraction -= OnUpdateInteraction;
                _dialControl.EndInteraction -= OnEndInteraction;
            }
            else
                Debug.LogError("Dial Control was not found on disable.");
        }

        private void Start()
        {
            if (_dialPositions == null || !_dialPositions.Any())
            {
                Debug.LogWarning($"No dial positions were found for {name}, disabling component.");
                enabled = false;
                return;
            }

            if (!string.IsNullOrEmpty(_startingPosition))
            {
                _currentDialPosition = _dialPositions.FirstOrDefault(x => x.positionName == _startingPosition);

                if (_currentDialPosition.Equals(default))
                    Debug.LogWarning($"Starting position {_startingPosition} not found in dial positions for {name}");
            }
        }

        public void OnBeginInteraction()
        {
            _isSnapped = false;
        }

        public void OnUpdateInteraction(object val)
        {
            Vector3 euler;
            try
            {
                euler = (Vector3)val;
            }
            catch (System.Exception e)
            {
                return;
            }

            _currentDialPosition = GetClosestDialPosition(euler.z);
            _dialControl.SetValue(DialValueCalculator.OnValueUpdated(
                    _dialControl.Knob.localEulerAngles.z,
                    _dialControl.MinVal,
                    _dialControl.MaxVal,
                    _currentDialPosition.angle));
            if (!_currentDialPosition.isFree && !_currentDialPosition.isMomentary)
            {
                SnapToPosition(_currentDialPosition, false);
            }
            else if (_isSnapped)
                _isSnapped = false;
        }

        public void OnEndInteraction()
        {
            // Already snapping, don't do anything
            if (_isSnapping)
                return;

            // If we haven't snapped yet, get the current closest position and snap
            if (!_isSnapped)
            {
                DialPosition closestPosition = GetClosestDialPosition();
                SnapToPosition(closestPosition, true);
            }
            else // We've already snapped, set to false so that we can again next time.
            {
                _isSnapped = false;
            }
        }

        public void OnInteractableUpdated(bool val)
        {
            if (_dialControl != null)
                OnEndInteraction();
        }

        // Gets the closest position based on our Knob's local z rotation value, compared to the position angle value.
        private DialPosition GetClosestDialPosition(float optionalZ = -1)
        {
            float currentAngle;
            if (optionalZ < 0)
                currentAngle = _dialControl.Knob.localEulerAngles.z; // Assuming z-axis rotation
            else
                currentAngle = optionalZ;

            DialPosition closestPosition = new DialPosition();
            float minDistance = float.MaxValue;

            foreach (var position in _dialPositions)
            {
                // Use DeltaAngle to handle 0/360-degree wrap-around, this will allow the user to set angles based on editor euler values.
                float distance = Mathf.Abs(Mathf.DeltaAngle(currentAngle, position.angle));
                float otherDistance = Mathf.DeltaAngle(currentAngle, position.angle);

                // If this distance is less than the current minimum distance, update it since it's closer.
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPosition = position;
                }
            }

            return closestPosition;
        }

        // Handles snap functionality depending on the position chosen.
        private void SnapToPosition(DialPosition position, bool invokeOnComplete)
        {
            // If the position is free and not momentary, then we know we're updating our free range value and don't need to snap.
            if (position.isFree && !position.isMomentary)
            {
                _dialControl.SetValue(DialValueCalculator.OnValueUpdated(
                    _dialControl.Knob.localEulerAngles.z,
                    _dialControl.MinVal,
                    _dialControl.MaxVal,
                    position.angle));
                return;
            }
            // Snap to the new angle and assign our current position.
            RotateDialToAngle(position.angle, invokeOnComplete);
            _currentDialPosition = position;

            // If the position is only momentary, get the previous element and snap back to it after the set momentary delay.
            if (position.isMomentary)
            {
                _isSnapping = true;
                int index = _dialPositions.IndexOf(position);
                if (index > 0)
                {
                    RotateDialToAngle(_dialPositions[index - 1].angle, invokeOnComplete, position.momentaryDelay);
                    _dialControl.EndInteraction.Invoke();
                }
                else
                {
                    Debug.LogWarning("Cannot access previous position, index is zero.");
                }
                _isSnapping = false;
            }
        }

        // Simple sequence that allows us to control the rotation and act after it's complete.
        private Sequence RotateDialToAngle(float angle, bool invokeOnComplete, float delay = 0)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_dialControl.Knob.DOLocalRotate(_dialControl.AxisRotation * angle, _snapSpeed)
                .SetDelay(delay)
                .SetEase(_snapEase)
                .OnComplete(() => { if (invokeOnComplete) _dialControl.EndInteraction?.Invoke(); })); // Alert listeners that the snapper has snapped.
            sequence.AppendCallback(() => _isSnapped = true);

            return sequence;
        }
    }
}