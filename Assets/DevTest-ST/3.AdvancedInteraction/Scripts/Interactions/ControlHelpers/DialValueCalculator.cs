using UnityEngine;

namespace JoshBowersDev.DevTestST.AdvancedInteraction
{
    /// <summary>
    /// Simple utility class responsible for translating the rotational value of a <see cref="DialControl"/>
    /// into a mapped Value, within specified bounds.
    /// </summary>
    public static class DialValueCalculator
    {
        /// <summary>
        /// Translates rotational values and updates the <see cref="DialControl"/>'s value.
        /// </summary>
        /// <param name="maxAngle"></param>
        /// <param name="minAngle"></param>
        public static int OnValueUpdated(float eulerZ, float minVal, float maxVal, float maxAngle, float minAngle = 360)
        {
            var z = eulerZ;
            var absMax = Mathf.Abs(maxAngle);

            // Normalize the z angle to 0,360
            z = (z + 360) % 360;

            // Since we're working back from 360 instead of 0, automatically set to minimum value if our dial is at the
            // 0 mark.
            if (z == 0)
                return (int)minVal;

            // Clamp z between min and max to be safe
            z = Mathf.Clamp(z, absMax, minAngle);

            // Find the percentage of z's position between min and max, also make sure we don't create a black hole
            // dividing by 0.
            float t = (absMax != minAngle) ? (z - minAngle) / (absMax - minAngle) : 0;

            // Lerp between our set minimum value and maximum value based on where our dial was pointing toward.
            return Mathf.FloorToInt(Mathf.Lerp(minVal, maxVal, t));
        }
    }
}