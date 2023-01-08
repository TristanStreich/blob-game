using UnityEditor;
using UnityEngine;

namespace Utils
{
    [InitializeOnLoad]
    public static class ExtensionMethods
    {
        // Get Average Position of a collision
        public static Vector3 AveragePosition(this Collision collision) {
            Vector3 sum = Vector3.zero;
            foreach (ContactPoint contact in collision.contacts) {
                sum += contact.point;
            }
            return sum / collision.contacts.Length;
        }

        public static float HeightAboveGround(this Transform transform) {
            // Raycast down from the transform to find the ground below it
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit)) {
                // Return the distance between the transform and the ground
                return transform.position.y - hit.point.y;
            }
            else {
                // If no ground was found, return 0
                return 0f;
            }
        }
    }
}
