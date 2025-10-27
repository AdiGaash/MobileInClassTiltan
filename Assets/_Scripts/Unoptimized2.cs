using UnityEngine;
using System.Collections.Generic;

namespace Shooter
{
    public class Unoptimized2 : MonoBehaviour
    {
        public GameObject prefab;
        public List<GameObject> spawnedObjects = new List<GameObject>();
        public List<Transform> spawnedObjectsTransform = new List<Transform>();
        public Transform target;

        private void Start()
        {
            for (int i = 0; i < 1000; i++)
            {
                GameObject go = Instantiate(prefab);
                go.transform.position = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
                spawnedObjects.Add(go);
            }
        }

        private void Update()
        {
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                spawnedObjects[i].transform.position = new Vector3(
                    spawnedObjects[i].transform.position.x,
                    Mathf.Sin(Time.time * i) * 2f,
                    spawnedObjects[i].transform.position.z
                );
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                {
                    if (go.name.Contains("Enemy"))
                    {
                        go.SetActive(false);
                    }
                }
            }

            if (target != null)
            {
                transform.LookAt(GameObject.Find(target.name).transform.position);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Resources.Load<GameObject>("EnemyPrefab");
            }

            foreach (GameObject go in spawnedObjects)
            {
                go.transform.localScale = new Vector3(
                    go.transform.localScale.x + Time.deltaTime,
                    go.transform.localScale.y + Time.deltaTime,
                    go.transform.localScale.z + Time.deltaTime
                );
            }
        }
    }

}