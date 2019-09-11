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
    void Callback(T updatedValue);
}