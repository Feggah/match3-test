using System.Collections;
using UnityEngine;

public static class TransformExtensions
{
    public static IEnumerator Move(this Transform actual, Vector3 target, float duration)
    {
        Vector3 distance = (target - actual.position);

        for (float counter = 0; counter < duration; counter += Time.deltaTime)
        {
            float distanceOfEachStep = (Time.deltaTime * distance.magnitude) / duration;
            actual.position += distance.normalized * distanceOfEachStep;
            yield return null;
        }
        actual.position = target;
    }
}
