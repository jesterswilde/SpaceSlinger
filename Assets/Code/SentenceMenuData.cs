using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Words
{
    public class SentenceMenuData : SerializedMonoBehaviour
    {
        ChoiceTri baseTri = ChoiceTri.CreateBase();
        ChoiceTri currentTri;
        [SerializeField]
        SentenceMenuDisplay display;
        public void StartNewSentence()
        {
            currentTri = baseTri;
            display.AppendWords("I want to");
            ShowCurrentTri();
        }
        void ShowCurrentTri()
        {
            bool canClick = true;
            display.ShowTri(currentTri, (word) =>
            {
                if (!canClick)
                    return;
                canClick = false;
                Callback.Create(() => PickedItem(word), SMSettings.EffectDuration);
            });
        }
        public void PickedItem(string word)
        {
            display.AppendWords(word);
            display.ClearTri();
            if (currentTri.IsLeaf)
                CompletedSentence();
            else
            {
                currentTri = currentTri.GetTri(word);
                display.ShowTri(currentTri, PickedItem);
            }
        }
        public void CompletedSentence()
        {
            Debug.Log("Completed Sentence");
            display.Hide();
        }
        public void RegisterWordData(WordData item)
        {
            var pathArray = item.WordPath.Split('/').Select((val, i) => (val, i));
            var curTri = baseTri;
            foreach (var (word, i) in pathArray)
            {
                var nextTri = curTri.HasWord(word) switch {
                    true => curTri.GetTri(word),
                    false => (i == pathArray.Count() - 1) switch
                    {
                        true => ChoiceTri.CreateLeaf(word, item.Equipment),
                        false => ChoiceTri.CreateParent(word)
                    }
                };
                curTri.TryMakeChild(nextTri);
                curTri = nextTri;
            }
        }
        private void Awake()
        {
            GetComponentsInChildren<WordData>().ForEach(RegisterWordData);
        }
        [Serializable]
        public class ChoiceTri
        {
            [SerializeField]
            public Dictionary<string, ChoiceTri> choices;
            public string Word;
            public Equipment Equipment;
            public bool IsLeaf => choices == null;
            public List<ChoiceTri> GetChildren() => choices.Values.Select(val => val).ToList(); 
            public void TryMakeChild(ChoiceTri tri)
            {
                if(!choices.ContainsKey(tri.Word))
                    choices[tri.Word] = tri;
            }
            public ChoiceTri GetTri(string word) => choices[word];
            public static ChoiceTri CreateLeaf(string word, Equipment equip) =>
                new ChoiceTri() { Equipment = equip, Word = word };
            public static ChoiceTri CreateParent(string word) =>
                new ChoiceTri() { Word = word, choices = new Dictionary<string, ChoiceTri>() };
            public static ChoiceTri CreateBase() =>
                new ChoiceTri() { choices = new Dictionary<string, ChoiceTri>() };

            internal bool HasWord(string word) => choices.ContainsKey(word);
        }
    }
}