﻿using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
	#region fields
	private static object   m_Lock = new object();
	private static T		instance = null;
	#endregion

	#region methods
	public static T Instance
	{
		get
		{
			lock ( m_Lock )
			{
				if ( instance == null )
				{
					instance = FindObjectOfType<T>();
					if ( instance == null )
					{
						GameObject obj = new GameObject
						{
							name = typeof(T).Name
						};
						instance = obj.AddComponent<T>();
					}
				}
			}			

			return instance;
		}
	}

	public static void Create()
	{
		//var manager = Instance;
	}

	//protected virtual void Initialize() { }


	protected virtual void Awake()
	{
		if (instance == null)
		{
			instance = this as T;
			DontDestroyOnLoad( gameObject );
			//Initialize();
		}
		else
		{
			// If there is a same component in the next scene and the component class tries to call singleton, 
			// then the instance here won't be null anymore, therefore the game object the instance has,
			// is gonna be destroyed here by the following line. Because Singleton can't be multiple.
			Destroy(gameObject);
		}
	}
	/*
	public static bool IsActivated()
	{
		if (instance != null)
			return true;
		else
			return false;
	}
	*/
	#endregion
}
