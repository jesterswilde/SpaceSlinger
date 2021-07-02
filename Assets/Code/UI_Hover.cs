using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Action OnEnter;
    public event Action OnExit;
    public event Action OnClick;
    public UnityEvent OnEnterUE;
    public UnityEvent OnExitUE;
    public UnityEvent OnClickUE;
    bool isOver = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        OnEnter?.Invoke();
        OnEnterUE?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
        OnExit?.Invoke();
        OnExitUE?.Invoke();
    }
    void CheckOnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick?.Invoke();
            OnClickUE?.Invoke();
        }
    }
    private void Update()
    {
        if (isOver)
            CheckOnClick();
    }
}
