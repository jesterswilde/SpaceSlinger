using Febucci.UI.Core;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace Words
{
    [UnityEngine.Scripting.Preserve]
    [EffectInfo(tag: "color")]
    class ColorBehavior : BehaviorBase
    {
        bool useTags = false;
        float r;
        float g;
        float b;
        string colorType;
        float startTime;
        float lerpTime = 0.5f;
        float duration => Time.time - startTime;

        public override void ApplyEffect(ref CharacterData data, int charIndex)
        {
            Color color;
            if (useTags)
                color = new Color(r, g, b);
            else if (!string.IsNullOrEmpty(colorType))
                color = SMSettings.GetColor(colorType);
            else
                color = Color.magenta;
            if (duration < lerpTime)
                color = Color.Lerp(Color.white, color, duration / lerpTime);
            data.colors.SetColor(color);
        }

        public override void SetDefaultValues(BehaviorDefaultValues data)
        {
            startTime = Time.time;
        }

        public override void SetModifier(string modifierName, string modifierValue)
        {
            switch (modifierName)
            {
                case "r":  SetModTo(ref r, modifierValue); useTags = true; break;
                case "g":  SetModTo(ref g, modifierValue); useTags = true; break;
                case "b":  SetModTo(ref b, modifierValue); useTags = true; break;
                case "use": colorType = modifierValue; break; 
            }
        }
    }
}