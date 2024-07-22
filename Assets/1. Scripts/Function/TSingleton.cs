using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSingleton<T> : MonoBehaviour where T : TSingleton<T>
{
    static volatile T uniqueInstance;
    static volatile GameObject uniqueObject = null;

    protected TSingleton()
    {
        uniqueInstance = null;
        uniqueObject = null;
    }

    public static T instance
    {
        get
        {
            if (uniqueInstance == null)
            {
                lock(typeof(T))
                {
                    if (uniqueInstance == null && uniqueObject == null)
                    {
                        uniqueObject = new GameObject(typeof(T).Name, typeof(T));
                        uniqueInstance = uniqueObject.GetComponent<T>();

                        uniqueInstance.Init();
                    }
                }
            }

            return uniqueInstance;
        }
    }

    // 상속 전용 클래스로 만듬
    protected virtual void Init()
    {
        DontDestroyOnLoad(gameObject);
    }
}
