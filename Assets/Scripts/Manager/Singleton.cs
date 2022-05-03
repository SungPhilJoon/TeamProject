using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : class
{
	private static T instance;

	public static T Instance
	{
		get
		{
            if (instance == null)
            {
                instance = System.Activator.CreateInstance(typeof(T)) as T;
            }

            return instance;
		}
	}

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
