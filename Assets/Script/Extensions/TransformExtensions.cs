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

    public static IEnumerator Reduce(this Transform actual, Vector3 target, float duration)
    {
        Vector3 difference = (target - actual.localScale);

        for (float counter = 0; counter < duration; counter += Time.deltaTime)
        {
            float sizeDifference = (Time.deltaTime * difference.magnitude) / duration;
            actual.localScale += difference.normalized * sizeDifference;
            yield return null;
        }
        actual.localScale = target;
    }
}
