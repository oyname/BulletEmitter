using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectPoolManager : MonoBehaviour
{
	public ObjectPool[] Pools;
	public static ObjectPoolManager Instance;
	public float lifeTime;

	// This list contains objects, which are deactivated after a defined time.
	// are deactivated automatically
	[System.Serializable]
	public class Entity
	{
		public GameObject go;
		public float lifeTime;
	}
	public List<Entity> listOfGameObjects = new List<Entity>();

	// This list contains objects that can be destroyed all at once
	// can be destroyed
	[System.Serializable]
	public class DestructibleEntity
	{
		public GameObject go;
	}
	public List<DestructibleEntity> destructibleEntity = new List<DestructibleEntity>();

	private void Awake()
    {
		if (Instance != null)
		{
			Debug.LogError("Object already initialized");
			return;
		}

		DontDestroyOnLoad(gameObject);
		Instance = this;
	}

    private void Start()
	{
		StartCoroutine(PrewarmObject());
	}

	private void Update()
	{
		// Update list with objects that will be automatically 
		// certain time are automatically destroyed
		UpdateDestructibleEntity();
		UpdateGameObjectList();
	}

	private void UpdateGameObjectList()
    {
		for (int i = 0; i < listOfGameObjects.Count; i++)
		{
			// When the time comes the object will be destroyed
			if (listOfGameObjects[i].lifeTime < Time.time)
			{
				listOfGameObjects[i].go.transform.SetPositionAndRotation(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
				listOfGameObjects[i].go.SetActive(false);
				listOfGameObjects.RemoveAt(i);
			}
		}
	}

	private void UpdateDestructibleEntity()
	{
		int maxItem = destructibleEntity.Count-1;
		GameObject go;

		for (int i = maxItem; i >=0; i--)
		{
			go = destructibleEntity[i].go;

			// When the time comes the object will be destroyed
			if (!go.activeSelf)
				destructibleEntity.Remove(destructibleEntity[i]);
		}
	}

	private GameObject CreateInstanceAndAddToPool(ObjectPool pool)
	{
		GameObject instance = Instantiate(pool.ObjectToPool) as GameObject;
		DontDestroyOnLoad(instance);
		instance.SetActive(false);

		pool.Items.Add(instance);

		return instance;
	}

	private IEnumerator PrewarmObject()
	{
		Debug.Log("Prewarming Object...");

		foreach (var pool in Pools)
		{
			for (var i = 0; i < pool.PrewarmAmount; i++)
			{
				CreateInstanceAndAddToPool(pool);

				yield return null;
			}
		}

		Debug.Log("Prewarming done.");

		ResetPool();
	}

	// objects created with the Get(string,float) method are deactivated after n seconds.
	// automatically deactivated. Suitable for particle effect that should turn itself off
	public GameObject Get(string name, float lifetime)
	{
		Entity entity;

		ObjectPool pool = Pools.FirstOrDefault(p => p.Name == name);

		if (pool == null)
		{
			Debug.LogError("Object pool for " + name + " not found!");
			return null;
		}

		GameObject item = pool.Items.FirstOrDefault(i => !i.activeInHierarchy);

		if (item)
		{
			entity = new Entity();
			entity.lifeTime = Time.time + lifetime;
			entity.go = item;
			listOfGameObjects.Add(entity);

			return item;
		}

		if (!pool.CanExpanded)
		{
			// All possible objects are active
			return null;
		}

		return CreateInstanceAndAddToPool(pool);
	}

	// objects created with the Get(string) method are preserved until they
	// are disabled in the code. 
	public GameObject Get(string name)
	{
		DestructibleEntity entity;

		ObjectPool pool = Pools.FirstOrDefault(p => p.Name == name);

		if (pool == null)
		{
			Debug.LogError("Object pool for " + name + " not found!");
			return null;
		}

		GameObject item = pool.Items.FirstOrDefault(i => !i.activeInHierarchy);

		entity = new DestructibleEntity();
		destructibleEntity.Add(entity);

		if (!pool.CanExpanded)
		{
			return null;
		}

		return CreateInstanceAndAddToPool(pool);
	}

    public void ResetPool()
    {
		foreach (var pool in Pools)
		{
			for (var i = 0; i < pool.PrewarmAmount; i++)
			{
				pool.Items[i].transform.SetPositionAndRotation(new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f));
				pool.Items[i].SetActive(false);
			}
		}
	}
}