using Febucci.UI.Core;
using MiscUtil.Linq.Extensions;
using System.Linq;
using UnityEngine;

namespace Words
{
    [UnityEngine.Scripting.Preserve]
    [EffectInfo(tag: "fadeout")]
    class FadeOutAppearance : AppearanceBase
    {
        float endTime;
        public override void ApplyEffect(ref CharacterData data, int charIndex)
        {
            data.colors.LerpUnclamped(Color.clear, Tween.EaseInOut(data.passedTime / endTime));
            var bot = data.vertices.Min(a => a.y);
            var top = data.vertices.Max(a => a.y);
            var diff = bot - top;
            data.vertices.LerpUnclamped(
                    data.vertices.GetMiddlePos() + diff * Vector3.up * 3,
                    Tween.EaseInOut(data.passedTime / endTime)
                );
        }

        public override void SetDefaultValues(AppearanceDefaultValues data)
        {
            effectDuration = 100f;
            endTime = SMSettings.EffectDuration;
        }
    }
}