using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayUtil<T>
{
    public static T[] Append(T[] model, T element)
    {
        T[] result = new T[model.Length + 1];
        for(int i=0; i<model.Length; i++)
        {
            result[i] = model[i];
        }
        result[model.Length] = element;
        return result;
    }

    public static T[] Remove(T[] model, int index)
    {
        T[] result = new T[model.Length - 1];
        for(int i=0; i<index; i++)
        {
            result[i] = model[i];
        }
        for(int i=index + 1; i<model.Length; i++)
        {
            result[i-1] = model[i];
        }
        return result;
    }
}
