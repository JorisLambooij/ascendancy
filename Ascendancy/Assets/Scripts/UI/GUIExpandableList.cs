using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR) 
public class GUIExpandableList<T> : GUIContent where T : ScriptableObject
{
    private bool expanded;
    private string label;

    public List<T> elements;

    public GUIExpandableList(string label, bool expanded = false, int count = 4)
    {
        this.label = label;
        elements = new List<T>(count);
        this.expanded = expanded;
    }

    public bool Expanded
    {
        get => expanded;
        set
        {
            expanded = value;
        }
    }

    public void OnGUI()
    {
        expanded = EditorGUILayout.Foldout(expanded, label, true);

        if (expanded)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            
            int newCap = EditorGUILayout.IntField("Count", elements.Capacity);
            
            if (newCap > elements.Capacity)
                elements.Capacity = newCap;
            else if (newCap < elements.Capacity)
            {
                List<T> newList = new List<T>(newCap);
                for (int i = 0; i < newCap; i++)
                {
                    if (i < elements.Count)
                        newList.Add(elements[i]);
                    else
                        newList.Add(null);
                }
                elements = newList;
            }
            

            for (int i = 0; i < elements.Capacity; i++)
            {
                if (i < elements.Count)
                {
                    elements[i] = EditorGUILayout.ObjectField("Element " + i, elements[i], typeof(T), false) as T;
                }
                else
                {
                    elements.Add(EditorGUILayout.ObjectField("Element " + i, null, typeof(T), false) as T);
                }
            }
            
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }

    public T[] ElementsArray
    {
        get
        {
            T[] array = new T[elements.Count];
            elements.CopyTo(array);
            return array;
        }
    }
}
#endif

