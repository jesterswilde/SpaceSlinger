using Febucci.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



namespace Words
{
    public class WordButton : MonoBehaviour
    {
        TextAnimatorPlayer player;
        TextAnimator animator;
        [SerializeField]
        public string Word;
        [SerializeField]
        string enterTag = "<wiggle>$</wiggle>";
        [SerializeField]
        string exitTag = "<fade>$</fade>";
        [SerializeField]
        string hoverTag = "";
        [SerializeField]
        public float DestroyDelay = 2f;
        [SerializeField]
        Image frame;
        [SerializeField]
        bool runOnAwake = false;
        Action<string> onClick;
        Coroutine fadeCoroutine;
        Color baseColor;
        public void SetOnClick(Action<string> action)=> onClick = action;
        public void GotClickedOn()
        {
            FadeOut();
        }

        public void FadeIn()
        {
            player.ShowText(enterTag.Replace("$", Word));
            FadeButton(0.5f, baseColor);
        }
        public void FadeOut()
        {
            animator.EnableAppearancesLocally(true);
            animator.SetText(exitTag.Replace("$", Word), false);
            onClick?.Invoke(Word);
            FadeButton(DestroyDelay, Color.clear);
        }
        public void OnHover()
        {
            if(hoverTag != "")
            {
                animator.EnableAppearancesLocally(false);
                animator.SetText(hoverTag.Replace("$", Word), false);
            }
            FadeButton(0.2f, SMSettings.GetColor("hover"));
        }
        public void OnBlur()
        {
            if(hoverTag != "")
            {
                animator.EnableAppearancesLocally(false);
                animator.SetText(enterTag.Replace("$", Word), false);
            }
            FadeButton(0.2f, baseColor);
        }
        void FadeButton(float duration, Color targetCol)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(_FadeButton(duration, targetCol));
        }
        IEnumerator _FadeButton(float duration, Color targetCol)
        {
            float cur = 0;
            float wait = .1f;
            Color startColor = frame.color;
            while(cur < duration)
            {
                frame.color = Color.Lerp(startColor, targetCol, cur / duration);
                cur += wait;
                yield return new WaitForSeconds(wait);
            }
        }
        private void Awake()
        {
            player = GetComponentInChildren<TextAnimatorPlayer>();
            animator = GetComponentInChildren<TextAnimator>();
            baseColor = frame.color;
            frame.color = Color.clear;
            if (runOnAwake)
                FadeIn();
        }
    }
}
