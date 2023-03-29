using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    [Space]
    [Header("Bindings")]
    public KeyCode pauseKey = KeyCode.Escape;

    //pause toggle
    private bool paused = false;


    #region Update Loops
    private void Update()
    {
        PauseSystem();
    }
    #endregion


    #region Pause System
    private void PauseSystem()
    {
        if (Input.GetKeyDown(pauseKey) && paused)
        {
            Time.timeScale = 0;
            paused = true;
        }
        else
        {
            Time.timeScale = 1;
            paused = false;
        }
    }
    #endregion
}
