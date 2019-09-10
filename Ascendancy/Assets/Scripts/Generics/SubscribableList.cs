using System;
using System.Collections.Generic;
using UnityEngine;

public class SubscribableList<T>
{
    private List<T> list;
    private List<ListSubscriber<T>> subscribers;

    /// <summary>
    /// A List that other Objects can subscribe to, receiving notifications when values change.
    /// </summary>
    public SubscribableList()
    {
        this.list = new List<T>();
        subscribers = new List<ListSubscriber<T>>();
    }

    /// <summary>
    /// Subscribe an Object to this List. It will receive notifications when values change.
    /// </summary>
    /// <param name="subscriber">The subscribing Object must be of type ListSubscriber</param>
    public void Subscribe(ListSubscriber<T> subscriber)
    {
        subscribers.Add(subscriber);
    }

    /// <summary>
    /// Remove a ListSubscriber from the subscriber list.
    /// </summary>
    /// <param name="subscriber">The Subscriber to be removed.</param>
    public void Unsubscribe(ListSubscriber<T> subscriber)
    {
        if (subscribers.Contains(subscriber))
            subscribers.Remove(subscriber);
    }

    /// <summary>
    /// Adds a new Value to the List.
    /// </summary>
    /// <param name="value">Value.</param>
    public void Add(T value)
    {
        list.Add(value);

        foreach (ListSubscriber<T> subscriber in subscribers)
            subscriber.Callback(value);
    }

    /// <summary>
    /// Does this List contain the specified value?
    /// </summary>
    /// <param name="value">The Value.</param>
    /// <returns>True or false.</returns>
    public bool Contains(T value)
    {
        return list.Contains(value);
    }
    
    /// <summary>
    /// Returns the List of all elements, so other scripts can iterate over all elements if needed.
    /// </summary>
    public List<T> AsList
    {
        get => list;
    }
}

