﻿using UnityEngine;
using System.Collections;

public static class GameObjectExtensionsTactics
{
	public static T AddChildComponent<T> (this GameObject obj) where T : MonoBehaviour
	{
		GameObject child = new GameObject( typeof(T).Name );
		child.transform.SetParent(obj.transform);
		return child.AddComponent<T>();
	}
}
