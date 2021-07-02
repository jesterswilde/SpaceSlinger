using Febucci.UI;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Words
{
    public class SentenceMenuDisplay : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Tags")]
        string wrappingTag;
        [SerializeField, FoldoutGroup("Tags")]
        string oldElTags;
        [SerializeField, FoldoutGroup("Tags")]
        string newElTags = "<wiggle>$</wiggle>";
        [SerializeField, FoldoutGroup("Prefabs")]
        TextAnimator sentenceAnimPrefab;
        TextAnimator sentenceAnim;
        [SerializeField, FoldoutGroup("Prefabs")]
        WordButton wordPrefab;
        [SerializeField, FoldoutGroup("Prefabs")]
        Transform sentenceParent;
        [SerializeField, FoldoutGroup("Prefabs")]
        Transform wordParent;
        [SerializeField, FoldoutGroup("PLacement")]
        float distance = 10f;
        [SerializeField, FoldoutGroup("PLacement")]
        float maxScatter = 2f;
        [SerializeField, FoldoutGroup("PLacement")]
        float xPosMult = 2.5f;
        [SerializeField, FoldoutGroup("PLacement")]
        float yPosMult = .8f;
        string prevSentence = "";
        List<GameObject> childrenGos;
        internal void Hide()
        {
            Destroy(sentenceAnim.gameObject);
        }
        public void Show()
        {
            prevSentence = "";
        }

        internal void AppendWords(string words)
        {
            if (sentenceAnim == null)
                SetupAnimator();
            string sentence = prevSentence;
            prevSentence += $"  {words}";
            if (!string.IsNullOrEmpty(oldElTags))
                sentence = oldElTags.Replace("$", sentence);
            if (!string.IsNullOrEmpty(newElTags))
                sentence += newElTags.Replace("$", words);
            else
                sentence += words;
            if (!string.IsNullOrEmpty(wrappingTag))
                sentence = wrappingTag.Replace("$", sentence);
            sentenceAnim.SetText(sentence);
        }
        void SetupAnimator()
        {
            sentenceAnim = Instantiate(sentenceAnimPrefab);
            sentenceAnim.transform.SetParent(UI.Canvas.transform, false);
            sentenceAnim.transform.position = sentenceParent.position;
        }

        public void ClearTri()
        {
            childrenGos.ForEach(child => Destroy(child));
        }
        internal void ShowTri(SentenceMenuData.ChoiceTri tri, Action<string> PickedItem)
        {
            var offset = UnityEngine.Random.Range(0f, 1f);
            var children = tri.GetChildren();
            childrenGos = children.Select((child, i) => {
                var button = Instantiate(wordPrefab);
                button.transform.SetParent(UI.Canvas.transform,false);
                float scatter = distance + UnityEngine.Random.Range(0, maxScatter);
                button.transform.position = wordParent.position + 
                    new Vector3(Mathf.Sin(((float)(i + offset) / (float)children.Count) * Mathf.PI * 2) * xPosMult, Mathf.Cos(((float)(i + offset) / (float)children.Count) * Mathf.PI * 2) * yPosMult, 0) * scatter;
                button.Word = child.Word;
                button.DestroyDelay = SMSettings.EffectDuration;
                button.SetOnClick(PickedItem);
                button.FadeIn();
                return button.gameObject;
            }).ToList();
        }
    }
}