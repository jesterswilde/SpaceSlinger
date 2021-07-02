using UnityEngine;

namespace Febucci.UI.Core
{
    [UnityEngine.Scripting.Preserve]
    [EffectInfo(tag: TAnimTags.ap_Fade)]
    class FadeToColorAppearance : AppearanceBase
    {
        public override void ApplyEffect(ref CharacterData data, int charIndex)
        {
            data.colors.LerpUnclamped(Color.clear, Tween.EaseInOut(1 - (data.passedTime / effectDuration)));
        }

        public override void SetDefaultValues(AppearanceDefaultValues data)
        {
            effectDuration = data.defaults.fadeDuration;
        }
    }
}
