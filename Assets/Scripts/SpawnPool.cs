using System;
using System.Collections.Generic;
using UnityEngine;

namespace SSV_LudumDare40
{
	public class SpawnPool
	{
		private const int INCREMENT_SIZE = 2;

		private Func<int, IPooledObject> createNewObjectAction;
		private Queue<IPooledObject> pooledObjects;
		private Transform objectPoolContainer;

		private int totalSpawned = 0;

		public SpawnPool(int initialCount, Func<int, IPooledObject> createAction, Transform poolContainer = null)
		{
			createNewObjectAction = createAction;
			pooledObjects = new Queue<IPooledObject>(initialCount);
			objectPoolContainer = poolContainer;

			AddObjectsToPool(initialCount);
		}

		public void AddObjectsToPool(int count)
		{
			for (int i = 0; i < count; i++)
			{
				ReturnObject(createNewObjectAction.Invoke(++totalSpawned));
			}
		}

		public GameObject GetObject()
		{
			// Add more if needed
			if (pooledObjects.Count <= 0)
			{
				AddObjectsToPool(INCREMENT_SIZE);
			}

			var obj = pooledObjects.Dequeue();
			obj.ActivateFromPool();
			return obj.GetGameObject();
		}

		public void ReturnObject(GameObject obj)
		{
			var pooledObj = obj.GetComponent<IPooledObject>();
			if (pooledObj != null)
				ReturnObject(pooledObj);
		}

		private void ReturnObject(IPooledObject pooledObj)
		{
			pooledObj.DeactivateToPool();
			if (objectPoolContainer != null)
				pooledObj.GetGameObject().transform.parent = objectPoolContainer;
			pooledObjects.Enqueue(pooledObj);
		}
	}
}