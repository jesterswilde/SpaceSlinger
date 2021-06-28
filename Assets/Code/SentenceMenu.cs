using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SentenceMenu : SerializedMonoBehaviour
{
    ChoiceTri baseTri = ChoiceTri.CreateBase();
    ChoiceTri currentTri;
    string currentSentence;
    [SerializeField]
    MenuDisplay display;
    public void StartNewSentence()
    {
        currentTri = baseTri;
        currentSentence = "I want to ";
        display.Sentence(currentSentence);
        display.Tri(currentTri);
    }
    public void PickedItem(string word)
    {
        currentTri = currentTri.GetTri(word);
        currentSentence += $"{word} "; 
        if (currentTri.IsFinal)
            CompletedSentence();
    }
    public void CompletedSentence()
    {
        display.CompletedSentence();
    }
    public void AddSentenceToMenu(MenuItem item)
    {
        var pathArray = item.WordPath.Split('/').Select((val, i) => (val,i));
        var curTri = baseTri;
        foreach(var (val, i) in pathArray)
        {
            var nextTri = (i == (pathArray.Count() - 1)) switch {
                true => ChoiceTri.CreateLeaf(val, item.Equipment),
                false => ChoiceTri.CreateParent(val)
            };
            curTri.MakeChild(nextTri);
            curTri = nextTri;
        }
    }
    private void Awake()
    {
        GetComponentsInChildren<MenuItem>().ForEach(AddSentenceToMenu);
    }
    [Serializable]
    public class ChoiceTri
    {
        [SerializeField]
        Dictionary<string, ChoiceTri> choices;
        public string Word;
        public bool IsFinal;
        public Equipment Equipment;
        public bool IsLeaf => Equipment == null;
        public void MakeChild(ChoiceTri tri)
        {
            choices[tri.Word] = tri;
        }
        public ChoiceTri GetTri(string word) => choices[word];
        public static ChoiceTri CreateLeaf(string word, Equipment equip) =>
            new ChoiceTri() { Equipment = equip, Word = word };
        public static ChoiceTri CreateParent(string word) =>
            new ChoiceTri() { Word = word, choices = new Dictionary<string, ChoiceTri>() };
        public static ChoiceTri CreateBase() =>
            new ChoiceTri() { choices = new Dictionary<string, ChoiceTri>()};
    }
}
