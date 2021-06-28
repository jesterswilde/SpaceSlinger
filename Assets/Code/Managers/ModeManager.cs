using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    static ModeManager t;
    [SerializeField]
    PlayerInput normalMode;
    [SerializeField]
    PlayerInput drawMode;
    [SerializeField]
    GameMode mode;
    [SerializeField]
    float drawModeSpeed = 0.1f;
    public static GameMode Mode => t?.mode ?? GameMode.Normal;


    public void EnterDrawMode()
    {
        normalMode.IsActive = false;
        drawMode.IsActive = true;
        mode = GameMode.Drawing;
        Time.timeScale = t.drawModeSpeed;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void EnterNormalMode()
    {
        normalMode.IsActive = true;
        drawMode.IsActive = false;
        mode = GameMode.Normal;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        EnterNormalMode();
    }
    private void Awake()
    {
        t = this;
    }
}
public enum GameMode
{
    Normal,
    Drawing
}
