using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomLeaderBoard
{
    public class Easing 
    {
        public static float LinearEasing
            ( float elapsedTime , float start , float end , float duration )
        {
            if (duration <= 0) return end; // Avoid divide by zero
            return Mathf.Lerp(start , end , elapsedTime / duration);
        }
    }
}
