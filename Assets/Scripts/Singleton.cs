using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> where T:class,new()
{
    private static T _minastance=null;
    public static T Instance
    {
        get
        {
            if(_minastance == null)
            {
                _minastance= Activator.CreateInstance<T>();
            }
            return _minastance;
        }
    }

}
