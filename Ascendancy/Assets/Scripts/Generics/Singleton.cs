using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoBehaviour_Singleton : MonoBehaviour
{
    private static MonoBehaviour_Singleton instance;

    public static MonoBehaviour_Singleton Instance
    {
        get { return instance; }
    }

    protected virtual void Start()
    {
        instance = this;
    }
}
