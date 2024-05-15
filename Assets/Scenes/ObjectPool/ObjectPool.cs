using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class ObjectPool
{
	public GameObject ObjectToPool;
	public int PrewarmAmount;
	public bool CanExpanded;
	public string Name;
	public Type type;

	[System.NonSerialized]
	public List<GameObject> Items = new List<GameObject>();
}