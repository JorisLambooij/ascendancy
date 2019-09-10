using System;
using System.Collections.Generic;
using UnityEngine;

public class SubscribableList<T>
{
    private List<T> list;
    private List<ListSubscriber<T>> subscribers;

    public SubscribableList()
    {
        this.list = new List<T>();
        subscribers = new List<ListSubscriber<T>>();
    }
    
    public void Subscribe(ListSubscriber<T> subscriber)
    {
        subscribers.Add(subscriber);
    }

    public void Unsubscribe(ListSubscriber<T> subscriber)
    {
        if (subscribers.Contains(subscriber))
            subscribers.Remove(subscriber);
    }

    public void Add(T value)
    {
        list.Add(value);

        foreach (ListSubscriber<T> subscriber in subscribers)
            subscriber.Callback(value);
    }

    public bool Contains(T value)
    {
        return list.Contains(value);
    }
    
    public List<T> AsList
    {
        get => list;
    }
}

