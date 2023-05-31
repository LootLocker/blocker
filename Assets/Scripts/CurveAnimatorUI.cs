using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveAnimatorUI : MonoBehaviour
{
    // Declare public AnimationCurve variables for "appearCurve" and "disappearCurve"
    public AnimationCurve appearCurve;
    public AnimationCurve disappearCurve;

    // Declare a public float variable for "duration"
    public float duration;

    // Declare an enumeration "FadeType" with options "Appear" and "Disappear"
    public enum FadeType { Appear, Disappear };

    // Declare a private Coroutine variable "appearDisappearRoutine"
    private Coroutine appearDisappearRoutine;

    // Declare a public Transform variable "inputBlocker"
    public Transform inputBlocker;

    // Start is called before the first frame update
    public void Show()
    {
        // Set the localScale of the transform to Vector3.zero
        transform.localScale = Vector3.zero;
        // If inputBlocker is not null, set its gameObject's active state to true
        if (inputBlocker != null)
        {
            inputBlocker.gameObject.SetActive(true);
        }
        // Set the active state of the gameObject to true
        gameObject.SetActive(true);
        // If appearDisappearRoutine is not null, stop the coroutine
        if (appearDisappearRoutine != null)
        {
            StopCoroutine(appearDisappearRoutine);
        }
        // Start the ScaleRoutine coroutine and pass in "FadeType.Appear" as a parameter
        appearDisappearRoutine = StartCoroutine(ScaleRoutine(FadeType.Appear));
    }

    public void Hide()
    {
        // If inputBlocker is not null, set its gameObject's active state to true
        if (inputBlocker != null)
        {
            inputBlocker.gameObject.SetActive(true);
        }
        // Set the active state of the gameObject to true
        gameObject.SetActive(true);
        // If appearDisappearRoutine is not null, stop the coroutine
        if (appearDisappearRoutine != null)
        {
            StopCoroutine(appearDisappearRoutine);
        }
        // Start the ScaleRoutine coroutine and pass in "FadeType.Disappear" as a parameter
        appearDisappearRoutine = StartCoroutine(ScaleRoutine(FadeType.Disappear));
    }

    // Define the ScaleRoutine coroutine, which takes in a "FadeType" parameter
    IEnumerator ScaleRoutine(FadeType fadeType)
    {
        // Declare a float variable "timer" and set it to 0
        float timer = 0;
        // Declare a Vector3 variable "startScale" and set it to Vector3.zero if "fadeType" is "Appear" or Vector3.one if "fadeType" is "Disappear"
        Vector3 startScale = fadeType == FadeType.Appear ? Vector3.zero : Vector3.one;
        // Declare a Vector3 variable "endScale" and set it to Vector3.one if "fadeType" is "Appear" or Vector3.zero if "fadeType" is "Disappear"
        Vector3 endScale = fadeType == FadeType.Appear ? Vector3.one : Vector3.zero;
        // Declare an AnimationCurve variable "scaleCurve" and set it to "appearCurve" if "fadeType" is "Appear" or "disappearCurve" if "fadeType" is "Disappear"
        AnimationCurve scaleCurve = fadeType == FadeType.Appear ? appearCurve : disappearCurve;
        // While "timer" is less than or equal to "duration"
        while (timer <= duration)
        {
            // Set the localScale of the transform to the result of a LerpUnclamped between "startScale" and "endScale" using the evaluated value of "scaleCurve" at "timer" divided by "duration"
            transform.localScale = Vector3.LerpUnclamped(startScale, endScale, scaleCurve.Evaluate(timer / duration));
            // Increment "timer" by Time.deltaTime
            timer += Time.deltaTime;
            // Wait for the next frame
            yield return null;
        }
        // Set the localScale of the transform to "endScale"
        transform.localScale = endScale;
        // If "fadeType" is "Disappear"
        if (fadeType == FadeType.Disappear)
        {
            // Set the active state of the gameObject to false
            gameObject.SetActive(false);
            // Set the localScale of the transform to Vector3.zero
            transform.localScale = Vector3.zero;
        }
        // If "fadeType" is "Appear"
        else
        {
            // If inputBlocker is not null, set its gameObject's active state to false
            if (inputBlocker != null)
            {
                inputBlocker.gameObject.SetActive(false);
            }
        }
    }
}
