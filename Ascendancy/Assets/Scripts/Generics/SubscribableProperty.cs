using System;
using System.Collections.Generic;
using UnityEngine;

public struct SubscribableProperty<T>
{
    private T value;

    private List<PropertySubscriber<T>> subscribers;

    public SubscribableProperty(T value)
    {
        this.value = value;
        subscribers = new List<PropertySubscriber<T>>();
    }

    public void Subscribe(PropertySubscriber<T> subscriber)
    {
        subscribers.Add(subscriber);
    }

    public void Unsubscribe(PropertySubscriber<T> subscriber)
    {
        if (subscribers.Contains(subscriber))
            subscribers.Remove(subscriber);
    }

    public T Value
    {
        get { return value; }
        set
        {
            this.value = value;

            foreach (PropertySubscriber<T> subscriber in subscribers)
                subscriber.Callback(value);
        }
    }
}
