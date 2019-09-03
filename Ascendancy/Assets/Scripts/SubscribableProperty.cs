using System;
using System.Collections.Generic;
using UnityEngine;

public struct SubscribableProperty<T>
{
    private T value;

    private List<Action<T>> subscribers;

    public SubscribableProperty(T value)
    {
        this.value = value;
        subscribers = new List<Action<T>>();
    }

    public void Subscribe(Action<T> callback)
    {
        subscribers.Add(callback);
    }

    public void Unsubscribe(Action<T> callback)
    {
        if (subscribers.Contains(callback))
            subscribers.Remove(callback);
    }

    public T Value
    {
        get { return value; }
        set
        {
            this.value = value;
            
            foreach (Action<T> callback in subscribers)
                callback(value);
        }
    }
}
