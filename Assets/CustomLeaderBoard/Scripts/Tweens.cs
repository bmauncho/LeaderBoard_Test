using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomLeaderBoard
{
    public static class Tweens
    {
        private static readonly Dictionary<MonoBehaviour , Coroutine> ActiveCoroutines = new();

        public static void Value (
            MonoBehaviour monoBehaviour ,
            float from ,
            float to ,
            Action<float> onUpdate ,
            float duration ,
            float delay = 0f ,
            Action onComplete = null ,
            Func<float , float , float , float , float> easingFunction = null )
        {
            if (monoBehaviour == null || !monoBehaviour.gameObject.activeInHierarchy) return;

            if (ActiveCoroutines.ContainsKey(monoBehaviour))
            {
                monoBehaviour.StopCoroutine(ActiveCoroutines [monoBehaviour]);
                ActiveCoroutines.Remove(monoBehaviour);
            }

            easingFunction ??= Easing.LinearEasing; // Default to linear easing

            ActiveCoroutines [monoBehaviour] = monoBehaviour.StartCoroutine(
                RunAnimation(from , to , onUpdate , duration , delay , easingFunction , () =>
                {
                    onComplete?.Invoke();
                    ActiveCoroutines.Remove(monoBehaviour);
                })
            );
        }

        private static IEnumerator RunAnimation (
            float from ,
            float to ,
            Action<float> onUpdate ,
            float duration ,
            float delay ,
            Func<float , float , float , float , float> easingFunction ,
            Action onComplete )
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            if (duration <= 0f)
            {
                onUpdate?.Invoke(to);
                onComplete?.Invoke();
                yield break;
            }

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float value = easingFunction(elapsedTime , from , to , duration);
                onUpdate?.Invoke(value);
                yield return null;
            }

            onUpdate?.Invoke(to);
            onComplete?.Invoke();
        }
    }
}
