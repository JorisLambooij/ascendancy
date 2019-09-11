using System;
using System.Collections.Generic;
using UnityEngine;

public class SubscribableDictionary<T, U>
{
    private Dictionary<T, U> dict;
    private List<DictionarySubscriber<T, U>> subscribers;

    /// <summary>
    /// A Dictionary that other Objects can subscribe to, receiving notifications when values change.
    /// </summary>
    public SubscribableDictionary(int capacity = 4)
    {
        this.dict = new Dictionary<T, U>();
        subscribers = new List<DictionarySubscriber <T, U>>();
    }
    
    /// <summary>
    /// Subscribe an Object to this Dictionary. It will receive notifications when values change.
    /// </summary>
    /// <param name="subscriber">The subscribing Object must be of type DicitonarySubscriber</param>
    public void Subscribe(DictionarySubscriber<T, U> subscriber)
    {
        subscribers.Add(subscriber);
    }

    /// <summary>
    /// Remove a DictionarySubscriber from the subscriber list.
    /// </summary>
    /// <param name="subscriber">The Subscriber to be removed.</param>
    public void Unsubscribe(DictionarySubscriber<T, U> subscriber)
    {
        if (subscribers.Contains(subscriber))
            subscribers.Remove(subscriber);
    }

    /// <summary>
    /// Adds a new Key-Value-Pair to the Dictionary.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    public void Add(T key, U value)
    {
        if (dict == null)
            Debug.LogError("WTF");
        dict.Add(key, value);

        foreach (DictionarySubscriber<T, U> subscriber in subscribers)
            subscriber.Callback(key, value);
    }

    /// <summary>
    /// Set a new Value for a spefified Key.
    /// </summary>
    /// <param name="key">The Key.</param>
    /// <param name="newValue">The new Value.</param>
    public void SetValue(T key, U newValue)
    {
        dict[key] = newValue;

        foreach (DictionarySubscriber<T, U> subscriber in subscribers)
            subscriber.Callback(key, newValue);
    }
    
    /// <summary>
    /// Returns the Value belonging to a specified Key, or null if the Key is not present in the Dictionary.
    /// </summary>
    /// <param name="key">The Key.</param>
    /// <returns>A Value of type U.</returns>
    public U GetValue(T key)
    {
        if (dict.ContainsKey(key))
            return dict[key];
        return default(U);
    }

    public Dictionary<T, U> AsDictionary
    {
        get => dict;
    }
}

