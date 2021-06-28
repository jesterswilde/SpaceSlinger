using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    bool startActive = false; 
    [SerializeField]
    List<KeyPress> keyPresses = new List<KeyPress>();
    [SerializeField]
    List<ButtonPress> mousePresses = new List<ButtonPress>();
    [SerializeField]
    UnityBoolEvent onChangeActive;
    [SerializeField]
    bool _isActive = false;
    public bool IsActive {
        get => _isActive;
        set {
            _isActive = value;
            onChangeActive?.Invoke(value);
        }
    }

    void Update()
    {
        if (!IsActive)
            return;
        keyPresses.ForEach(press => {
            Func<KeyCode, bool> condition = press.PressType switch {
                PressType.UP => Input.GetKeyUp,
                PressType.DOWN => Input.GetKeyDown,
                PressType.HELD => Input.GetKey,
                _=> throw new Exception("Not valid press type")
            };
            if (condition(press.KeyCode))
                press.ThingToDo?.Invoke();
        });
        mousePresses.ForEach(press => {
            Func<int, bool> condition = press.PressType switch {
                PressType.UP => Input.GetMouseButtonUp,
                PressType.DOWN => Input.GetMouseButtonDown,
                PressType.HELD => Input.GetMouseButton,
                _=> throw new Exception("Not valid press type")
            };
            if (condition(press.Index))
                press.ThingToDo?.Invoke();
        });
    }
    private void Awake()
    {
        IsActive = startActive;
    }
    enum PressType {
        UP,
        DOWN,
        HELD,
    }

    [Serializable]
    class KeyPress
    {
        [HorizontalGroup("Keys"), LabelWidth(75)]
        public PressType PressType;
        [HorizontalGroup("Keys"), LabelWidth(75)]
        public KeyCode KeyCode;
        public UnityEvent ThingToDo;
    }
    [Serializable]
    class ButtonPress
    {
        [HorizontalGroup("Keys"), LabelWidth(75)]
        public PressType PressType;
        [HorizontalGroup("Keys"), LabelWidth(75)]
        public int Index;
        public UnityEvent ThingToDo;
    }
}
[Serializable]
public class UnityBoolEvent : UnityEngine.Events.UnityEvent<bool> { }
