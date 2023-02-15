using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
	public static T Instance { get; private set; }

	protected virtual void Awake()
	{
		if (Instance == null)
		{
			Instance = (T)this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
}

