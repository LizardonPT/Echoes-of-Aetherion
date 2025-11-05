using System.Collections.Generic;
using UnityEngine;

namespace EchoesOfEtherion.ObjectPooling
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject objectToPool;
        [SerializeField] private uint objectPoolAmount;

        private Queue<GameObject> objectPool;

        [Header("Debug Options")]
        [SerializeField]
        private bool showDebugMessages = false;

        private void Awake()
        {
            objectPool = new Queue<GameObject>();
        }

        private void Start()
        {
            if (objectToPool == null)
            {
                LogError("No object assigned to pool.");
                return;
            }
            else if (!objectToPool.TryGetComponent<IPooledObject>(out _))
            {
                LogError($"Pooled object prefab '{objectToPool.name}' does not have a component implementing IPooledObject.");
                return;
            }

            for (int i = 0; i < objectPoolAmount; i++)
            {
                CreateNewObject();
            }
        }

        private GameObject CreateNewObject()
        {
            GameObject newObject = Instantiate(objectToPool, transform);
            if (!newObject.TryGetComponent<IPooledObject>(out _))
            {
                LogError($"Pooled object prefab '{objectToPool.name}' does not have a component implementing IPooledObject.");
                Destroy(newObject);
                return null;
            }
            objectPool.Enqueue(newObject);
            newObject.SetActive(false);

            Log($"Created new object: {newObject.name}");
            return newObject;
        }

        public GameObject GetObject()
        {
            GameObject poolObject = null;

            while (objectPool.Count > 0 && poolObject == null)
            {
                poolObject = objectPool.Dequeue();
                if (poolObject == null)
                {
                    Log("Found null object in pool, skipping.");
                }
            }

            if (poolObject == null)
            {
                poolObject = CreateNewObject();
                if (poolObject == null)
                {
                    LogError("Failed to create a new pooled object.");
                    return null;
                }
            }
            Log($"Reusing object from pool: {poolObject.name}");

            if (!poolObject.TryGetComponent(out IPooledObject pooledObjComponent))
            {
                LogError($"Object '{poolObject.name}' does not have a component implementing IPooledObject.");
                return null;
            }

            pooledObjComponent.SetPool(this);

            poolObject.transform.SetParent(null);
            poolObject.SetActive(true);

            pooledObjComponent.StartObject();

            Log($"Activated object: {poolObject.name}");

            return poolObject;
        }

        public void ReturnObject(GameObject poolObject)
        {
            if (!poolObject.TryGetComponent(out IPooledObject pooledObjComponent))
            {
                LogError($"Object '{poolObject.name}' does not have a component implementing IPooledObject.");
                return;
            }

            pooledObjComponent.StopObject();

            poolObject.transform.SetParent(transform);
            poolObject.transform.localPosition = Vector3.zero;
            poolObject.SetActive(false);

            objectPool.Enqueue(poolObject);

            Log($"Returned object to pool: {poolObject.name}");
        }

        private void Log(string message)
        {
            if (showDebugMessages)
            {
                Debug.Log($"[{objectToPool.name} Pool] {message}");
            }
        }

        private void LogError(string message)
        {
            Debug.LogError($"[{objectToPool.name} Pool] {message}");
        }

        private void OnValidate()
        {
            if (objectToPool != null && !objectToPool.TryGetComponent<IPooledObject>(out _))
            {
                LogError($"Pooled object prefab '{objectToPool.name}' does not have a component implementing IPooledObject.");
            }
        }
    }
}