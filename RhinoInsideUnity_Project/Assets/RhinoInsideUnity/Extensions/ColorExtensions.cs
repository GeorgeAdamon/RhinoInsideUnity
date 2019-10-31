using UnityEngine;

namespace RhinoInsideUnity.Extensions
{
    public static class ColorExtensions
    {

        #region TO UnityEngine.Color

        /// <summary>
        /// Converts a System.Drawing.Color struct to a UnityEngine.Color struct.
        /// </summary>
        public static Color ToUnityColor(this System.Drawing.Color systemColor)
        {
            return new Color(systemColor.R, systemColor.G, systemColor.B, systemColor.A);
        }

        /// <summary>
        /// Converts a UnityEngine.Color32 struct to a System.Drawing.Color struct.
        /// </summary>
        public static System.Drawing.Color ToSystemColor(this Color32 unityColor)
        {
            return System.Drawing.Color.FromArgb(unityColor.r,unityColor.g, unityColor.b, unityColor.a);
        }
        
        /// <summary>
        /// Converts a UnityEngine.Color struct to a System.Drawing.Color struct.
        /// </summary>
        public static System.Drawing.Color ToSystemColor(this Color unityColor)
        {
            return ((Color32) unityColor).ToSystemColor();
        }
        #endregion
    }
}
