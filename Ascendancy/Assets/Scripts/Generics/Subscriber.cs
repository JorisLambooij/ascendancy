using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PropertySubscriber<T>
{
    void Callback(T updatedValue);
}

public interface DictionarySubscriber<T, U>
{
    void Callback(T key, U newValue);
}

public interface ListSubscriber<T>
{
    /// <summary>
    /// Called when one new element is added.
    /// </summary>
    /// <param name="updatedValue"></param>
    void NewElementCallback(T updatedValue);

    /// <summary>
    /// Called when the entire list is reordered, deleted or otherwise changed.
    /// </summary>
    /// <param name="newList"></param>
    void NewListCallback(List<T> newList);
}