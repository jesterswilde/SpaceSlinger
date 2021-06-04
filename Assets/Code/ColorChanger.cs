using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.Utilities;

public class ColorChanger : MonoBehaviour
{
    [SerializeField]
    float lerpSpeed;
    float lerpPos = 0;
    bool shouldLerp => lerpPos > 0;
    List<Renderer> rends;
    List<Color> baseColors;
    List<Color> targetColors;
    List<Color> startingColors;
    internal void ChangeColor(Color color)
    {
        ChangeColor(rends.Select(_ => color).ToList()); 
    }
    internal void ChangeColor(List<Color> colors)
    {
        startingColors = rends.Select(rend => rend.material.color).ToList();
        targetColors = colors;
        lerpPos = 1f;
    }
    internal void ChangeToBaseColor()
    {
        ChangeColor(baseColors.ToList());
    }
    void LerpColor()
    {
        lerpPos = Mathf.Max(0, lerpPos - lerpSpeed * Time.deltaTime);
        rends.ForEach((rend, i) => rend.material.color = Color.Lerp(targetColors[i], startingColors[i], lerpPos));
    }

    private void Update()
    {
        if (shouldLerp)
            LerpColor();
    }
    private void Awake()
    {
        rends = GetComponentsInChildren<Renderer>().Where(rend => rend.enabled).ToList();
        baseColors = rends.Select(rend => rend.material.color).ToList();
    }
}
