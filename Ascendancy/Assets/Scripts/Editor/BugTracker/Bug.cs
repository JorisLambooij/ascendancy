using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bug : ScriptableObject
{
    /// <summary>
    /// The name of this bug.
    /// </summary>
    new public string name;

    /// <summary>
    /// Short description.
    /// </summary>
    [TextArea]
    public string description;

    /// <summary>
    /// Priority from 1 to 10.
    /// </summary>
    [Range(1, 10)]
    public int priority;

    /// <summary>
    /// Short description.
    /// </summary>
    public Object[] linkedFiles;

    /// <summary>
    /// Some notes.
    /// </summary>
    public string notes;
}
