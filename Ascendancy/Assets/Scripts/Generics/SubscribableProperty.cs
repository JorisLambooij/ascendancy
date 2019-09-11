using System;
using System.Collections.Generic;
using UnityEngine;

public struct SubscribableProperty<T>
{
    private T value;
    private List<PropertySubscriber<T>> subscribers;

    /// <summary>
    /// A Property that other objects can subscribe to.
    /// </summary>
    /// <param name="value">The initial value of the property.</param>
    public SubscribableProperty(T value)
    {
        this.value = value;
        subscribers = new List<PropertySubscriber<T>>();
    }

    /// <summary>
    /// Subscribe a PropertySubscriber to this Property.
    /// </summary>
    /// <param name="subscriber">The subscribing Object must be of type PropertySubscriber</param>
    public void Subscribe(PropertySubscriber<T> subscriber)
    {
        subscribers.Add(subscriber);
    }
    /// <summary>
    /// Unscubscribe from this Property.
    /// </summary>
    /// <param name="subscriber">The PropertySubscriber we want to have removed.</param>
    public void Unsubscribe(PropertySubscriber<T> subscriber)
    {
        if (subscribers.Contains(subscriber))
            subscribers.Remove(subscriber);
    }

    /// <summary>
    /// Gets or sets the Value of this Property.
    /// </summary>
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
