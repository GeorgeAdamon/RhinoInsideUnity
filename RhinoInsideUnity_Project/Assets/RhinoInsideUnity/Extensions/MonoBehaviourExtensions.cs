using UnityEngine;

namespace RhinoInsideUnity.Extensions
{
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// The following code was obtained from: http://wiki.unity3d.com/index.php/GetOrAddComponent
        /// </summary>
        public static T GetOrAddComponent<T>(this Component child) where T : Component
        {
            var result = child.GetComponent<T>();
            if (result == null)
            {
                result = child.gameObject.AddComponent<T>();
            }
            return result;
        }
    }
}