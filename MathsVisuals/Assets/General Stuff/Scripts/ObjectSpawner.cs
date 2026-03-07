using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public void SpawnObject(GameObject obj)
    {
        Instantiate(obj, transform.position, Quaternion.identity, transform);
    }

    public void SpawnObjectRandomRotation(GameObject obj)
    {
        Instantiate(obj, transform.position, Random.rotation, transform);
    }
}
