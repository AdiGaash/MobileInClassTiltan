using UnityEngine;

namespace Shooter
{
   public class Unoptimized1 : MonoBehaviour
    {
        public GameObject[] objectsToFind;
        public Transform player;

        private void Start()
        {
            objectsToFind = FindObjectsOfType<GameObject>();
            player = GameObject.Find("Player")?.transform;
        }

        private void Update()
        {
            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1));
            }

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
            foreach (var hit in hitColliders)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Debug.Log("Found enemy: " + hit.name);
                }
            }
        }
    }

}