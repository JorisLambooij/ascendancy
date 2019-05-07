using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ControlMode will handle all received Inputs according to the situation.
/// There should be ControlModes for when the player is in a menu, during the game, when selecting units or buildings etc.
/// </summary>
public abstract class ControlMode
{
    public static GameManager gameManager;

    public abstract void HandleInput();
}
