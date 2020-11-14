using System.Collections;
using UnityEngine;

namespace PixalHelper {
    public class Math {
        public static float Approach (float start, float end, float step) {
            if (start < end) return Mathf.Min (start + step, end);
            return Mathf.Max (start - step, end);
        }
    }
}