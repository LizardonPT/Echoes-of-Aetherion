using System.Collections.Generic;
using UnityEngine;

namespace EchoesOfAetherion.ObjectPooling
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject objectToPoll;
        [SerializeField] private int objectPollAmount;

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
            for (int i = 0; i < objectPollAmount; i++)
            {
                CreateNewObject();
            }
        }

        private GameObject CreateNewObject()
        {
            GameObject newObject = Instantiate(objectToPoll, parent: transform);
            objectPool.Enqueue(newObject);
            newObject.SetActive(false);
            Log($"Created new object: {newObject.name}");
            return newObject;
        }

        public GameObject GetObject()
        {
            GameObject poolObject;

            if (objectPool.Count > 0)
            {
                poolObject = objectPool.Dequeue();
                Log($"Reusing object from pool: {poolObject.name}");
            }
            else
            {
                poolObject = CreateNewObject();
            }

            IPooledObject pooledObjComponent = poolObject.GetComponent<IPooledObject>();
            if (pooledObjComponent == null)
            {
                Debug.LogError("Object does not have a PooledObject component.");
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
            if (!poolObject.TryGetComponent<IPooledObject>(out var pooledObjComponent))
            {
                Debug.LogError("Object does not have a PooledObject component.");
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
                Debug.Log(message); // Logs without object name.
            }
        }
    }
}