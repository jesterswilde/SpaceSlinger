using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.Utilities;
using Sirenix.OdinInspector;

public class ColorChanger : MonoBehaviour
{
    [SerializeField]
    float baseLerpSpeed;
    float? externalLerpSpeed;
    float lerpSpeed => externalLerpSpeed.HasValue ? externalLerpSpeed.Value : baseLerpSpeed;
    [ShowInInspector]
    float lerpPos = 0;
    [SerializeField]
    string colorProp = "_Color";
    bool shouldLerp => lerpPos > 0;
    List<Renderer> rends;
    List<Color> baseColors;
    List<Color> targetColors;
    List<Color> startingColors;
    internal void ChangeColor(Color color, bool shouldLerp = true, float? duration = null)
    {
        ChangeColor(rends.Select(_ => color).ToList(), shouldLerp, duration); 
    }
    internal void ChangeColor(List<Color> colors, bool shouldLerp = true, float? duration = null)
    {
        externalLerpSpeed = duration;
        if (shouldLerp)
        {
            startingColors = rends.Select(rend => rend.material.GetColor(colorProp)).ToList();
            targetColors = colors;
            lerpPos = 1f;
        }
        else
            rends.ForEach((rend, i) => rend.material.SetColor(colorProp, colors[i]));
    }
    internal void ChangeToBaseColor(bool shouldLerp = true, float? duration = null)
    {
        ChangeColor(baseColors.ToList(), shouldLerp, duration);
    }
    void LerpColor()
    {
        lerpPos = Mathf.Max(0, lerpPos - lerpSpeed * Time.deltaTime);
        rends.ForEach((rend, i) => rend.material.SetColor(colorProp, Color.Lerp(targetColors[i], startingColors[i], lerpPos)));
    }

    private void Update()
    {
        if (shouldLerp)
            LerpColor();
    }
    private void Awake()
    {
        rends = GetComponentsInChildren<Renderer>().Where(rend => rend.enabled).ToList();
        baseColors = rends.Select(rend => rend.material.GetColor(colorProp)).ToList();
    }
}
