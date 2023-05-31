using System.Collections;
using UnityEngine;

public class CurveAnimator : MonoBehaviour
{
    // Declare a public AnimationCurve variable "scaleCurve"
    public AnimationCurve scaleCurve;

    // Declare a private Vector3 variable "startScale"
    private Vector3 startScale;

    // Declare a public float variable "duration"
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        // Set "startScale" to the localScale of the transform
        startScale = transform.localScale;
        // Set the localScale of the transform to Vector3.zero
        transform.localScale = Vector3.zero;
        // Start the ScaleRoutine coroutine
        StartCoroutine(ScaleRoutine());
    }

    // Define the ScaleRoutine coroutine
    IEnumerator ScaleRoutine()
    {
        // Declare a float variable "timer" and set it to 0
        float timer = 0;
        // While "timer" is less than or equal to "duration"
        while (timer <= duration)
        {
            // Set the localScale of the transform to the result of a LerpUnclamped between Vector3.zero and "startScale" using the evaluated value of "scaleCurve" at "timer" divided by "duration"
            transform.localScale = Vector3.LerpUnclamped(Vector3.zero, startScale, scaleCurve.Evaluate(timer / duration));
            // Increment "timer" by Time.deltaTime
            timer += Time.deltaTime;
            // Wait for the next frame
            yield return null;
        }
        // Set the localScale of the transform to "startScale"
        transform.localScale = startScale;
    }
}
