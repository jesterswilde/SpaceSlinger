using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Words
{
    public class SMSettings : SerializedMonoBehaviour
    {
        static SMSettings t;
        [SerializeField]
        float effectDuration = 2f;
        [SerializeField]
        Dictionary<string, Color> colorsByName = new Dictionary<string, Color>();

        public static Color GetColor(string colName)
        {
            if (t.colorsByName.ContainsKey(colName))
                return t.colorsByName[colName];
            return Color.magenta;
        }
        public static float EffectDuration => t.effectDuration;
        private void Awake()
        {
            t = this;
        }
    }
}