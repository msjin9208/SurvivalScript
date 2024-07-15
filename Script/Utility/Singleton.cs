using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Singleton<T> where T : class
{
    private static T        _instance = null;
    private static object   _syncObj = new object();

    public static T Inst
    {
        get
        {
            lock (_syncObj)
            {
                if (_instance == null)
                {
                    Type t = typeof(T);

                    ConstructorInfo[] ctors = t.GetConstructors();
                    if (ctors.Length > 0)
                    {
                        throw new InvalidOperationException(string.Format("{0} constructor exception.", t.Name));
                    }

                    _instance = (T)Activator.CreateInstance(t, true);
                }
            }

            return _instance;
        }
    }
}
