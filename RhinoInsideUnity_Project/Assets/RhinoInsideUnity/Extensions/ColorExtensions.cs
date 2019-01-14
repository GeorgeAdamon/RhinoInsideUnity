using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhinoInsideUnity
{
    public static class ColorExtensions
    {

        #region TO UnityEngine.Color

        /// <summary>
        /// Converts a  System.Drawing.Color struct to a UnityEngine.Color struct.
        /// </summary>
        public static Color ToUnityColor(this System.Drawing.Color systemColor)
        {
            return new Color(systemColor.R, systemColor.G, systemColor.B, systemColor.A);
        }

        #endregion

    }
}
