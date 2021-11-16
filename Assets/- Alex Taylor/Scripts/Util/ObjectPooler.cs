using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Utility
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public Transform spawnParent;
            public int size;
        }

        public static ObjectPooler Instance;

        [SerializeField]
        private List<Pool> pools;

        private Dictionary<string, Queue<GameObject>> poolDictionary;
        private Dictionary<string, int> tagInstances;

        private void Awake()
        {
            Instance = this;
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            tagInstances = new Dictionary<string, int>();

            foreach (Pool pool in pools)
            {
                AddPool(pool);
            }
        }

        void Start()
        {
        }

        public void AddPool(Pool p)
        {
            //Makes Sure None Of Pools Have Same Tag
            p.tag += "_";
            if (tagInstances.ContainsKey(p.tag))
                tagInstances[p.tag]++;
            else
                tagInstances.Add(p.tag, 0);
            p.tag += tagInstances[p.tag];
            pools.Add(p);

            p.spawnParent.gameObject.name = p.tag;

            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < p.size; i++)
            {
                GameObject obj = Instantiate(p.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                obj.transform.SetParent(p.spawnParent);
            }
            poolDictionary.Add(p.tag, objectPool);

        }

        //Spawns Object From Pool
        public GameObject SpawnFromPool(string tag, Transform parent, float xOffset)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.SetParent(parent);
            objectToSpawn.transform.localPosition = new Vector3(0.0f + xOffset, 0.0f, 0.0f);
            objectToSpawn.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

            poolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        //Gets GameObjects From Pool
        public GameObject[] GetFromPool(string tag)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            return poolDictionary[tag].ToArray();
        }

        public int ReturnInstanceNum(string tag)
        {
            tag += "_";
            if (tagInstances.ContainsKey(tag))
                return tagInstances[tag];
            else return 0;
        }

        public void RemovePool(Pool p)
        {
            pools.Remove(p);
            poolDictionary[p.tag].Clear();
            Destroy(p.spawnParent.gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
