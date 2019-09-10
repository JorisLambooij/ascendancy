using System;
using System.Collections.Generic;
using UnityEngine;

public class SubscribableDictionary<T, U>
{
    private Dictionary<T, U> dict;

    private List<DictionarySubscriber<T, U>> subscribers;

    public SubscribableDictionary(int capacity = 4)
    {
        this.dict = new Dictionary<T, U>();
        subscribers = new List<DictionarySubscriber <T, U>>();
    }
    
    public void Subscribe(DictionarySubscriber<T, U> subscriber)
    {
        subscribers.Add(subscriber);
    }

    public void Unsubscribe(DictionarySubscriber<T, U> subscriber)
    {
        if (subscribers.Contains(subscriber))
            subscribers.Remove(subscriber);
    }

    public void Add(T key, U value)
    {
        if (dict == null)
            Debug.LogError("WTF");
        dict.Add(key, value);

        foreach (DictionarySubscriber<T, U> subscriber in subscribers)
            subscriber.Callback(key, value);
    }

    public void SetValue(T key, U newValue)
    {
        dict[key] = newValue;

        foreach (DictionarySubscriber<T, U> subscriber in subscribers)
            subscriber.Callback(key, newValue);
    }

    public U Value(T key)
    {
        return dict[key];
    }

    public Dictionary<T, U> AsDictionary
    {
        get => dict;
    }
}

