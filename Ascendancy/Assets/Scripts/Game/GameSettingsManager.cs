﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager instance;

    #region Tweakables
    [Header("Start Resources")]
    /// <summary>
    /// Resource amount at game start.
    /// </summary>
    public List<ResourceAmount> startResources = new List<ResourceAmount>();
    #endregion

    private void Start()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

}
