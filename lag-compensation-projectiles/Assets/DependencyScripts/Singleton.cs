using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace LagCompensationProjectiles
{
	public class Singleton<T> : MonoBehaviour where T : Component
	{
		public static T Instance { get; private set; }

		public virtual void Awake()
		{
			if (Instance == null)
			{
				Instance = this as T;
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	public class NetworkedSingleton<T> : NetworkBehaviour where T : Component
	{
		public static T Instance { get; private set; }

		public virtual void Awake()
		{
			if (Instance == null)
			{
				Instance = this as T;
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	public class SingletonPersistent<T> : MonoBehaviour where T : Component
	{
		public static T Instance { get; private set; }

		public virtual void Awake()
		{
			if (Instance == null)
			{
				Instance = this as T;
				DontDestroyOnLoad(this);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
	{
		static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}

				var results = Resources.FindObjectsOfTypeAll<T>();
				if (results.Length == 0)
				{
					Debug.LogError("SingletonScriptableObject: Results length is 0 of " + typeof(T));
					return null;
				}
				if (results.Length > 1)
				{
					Debug.LogError("SingletonScriptableObject: Results length is greater than 1 of " + typeof(T));

					return null;
				}

				_instance = results[0];
				_instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
				return _instance;
			}
		}
	}
}
