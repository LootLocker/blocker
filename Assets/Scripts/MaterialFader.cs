using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFader : MonoBehaviour
{
    public MeshRenderer ren;

    public float appearDuration;
    public float disappearDuration;

    public enum FadeType { Appear, Disappear};

    private Coroutine appearDisappearRoutine;

    public void Appear()
    {
        if(appearDisappearRoutine != null)
        {
            StopCoroutine(appearDisappearRoutine);
        }
        appearDisappearRoutine = StartCoroutine(AlphaFade(FadeType.Appear));
    }

    public void Disappear()
    {
        if (appearDisappearRoutine != null)
        {
            StopCoroutine(appearDisappearRoutine);
        }
        appearDisappearRoutine = StartCoroutine(AlphaFade(FadeType.Disappear));
    }

    void Start()
    {
        appearDisappearRoutine = StartCoroutine(AlphaFade(FadeType.Appear));
    }

    IEnumerator AlphaFade(FadeType fadeType)
    {
        Color startColor, endColor;
        startColor = endColor = ren.material.color;
        startColor.a = (fadeType == FadeType.Appear) ? 0 : 1f;
        endColor.a = (fadeType == FadeType.Appear) ? 1f : 0f;

        float duration = (fadeType == FadeType.Appear) ? appearDuration : disappearDuration;
        float timer = 0;

        ren.material.SetColor("_Color", startColor);
        while (timer <= duration)
        {
            ren.material.SetColor("_Color", Color.Lerp(startColor, endColor, timer / duration));
            timer += Time.deltaTime;
            yield return null;
        }
        ren.material.SetColor("_Color", endColor);
        if(fadeType == FadeType.Disappear)
        {
            Destroy(gameObject);
        }
    }
}
