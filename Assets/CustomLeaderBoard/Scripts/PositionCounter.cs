using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CustomLeaderBoard
{
    public class PositionCounter : MonoBehaviour
    {
        [Header("Counter Settings")]
        [SerializeField] private string format = "{0}"; // Display format for the counter value.
        [SerializeField] private TMP_Text label;       // Reference to the TMP_Text label.

        [Header("Animation Settings")]
        [SerializeField] private float defaultDuration = 1f; // Default animation duration.

        private int _value;           // Actual counter value.
        private int _animatedValue;   // Value during animation.

        public void SetDuration ( float duration ) => defaultDuration = Mathf.Max(0 , duration);

        public int GetCount () => _value;

        public void SetCountQuiet ( int value )
        {
            _value = value;
            _animatedValue = value;
            UpdateLabel();
        }

        public void SetCount ( int value , float? duration = null )
        {
            _value = value;
            float animDuration = duration ?? defaultDuration;

            // Animation logic with customizable duration.
            StartAnimation(_animatedValue , _value , animDuration);
        }

        private void StartAnimation ( int from , int to , float duration )
        {
            // Null check for safety.
            if (label == null)
            {
                Debug.LogError("Label is not assigned. Please assign a TMP_Text object.");
                return;
            }

            Tweens.Value(this , from , to , animatedValue =>
            {
                _animatedValue = Mathf.FloorToInt(animatedValue);
                UpdateLabel();
            } , duration);
        }

        private void UpdateLabel ()
        {
            if (label != null)
            {
                label.text = string.Format(format , _animatedValue);
            }
            else
            {
                Debug.LogWarning("Label is not assigned, skipping label update.");
            }
        }
    }

}
