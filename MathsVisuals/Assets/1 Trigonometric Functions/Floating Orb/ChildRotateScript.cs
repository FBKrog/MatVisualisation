using UnityEngine;

public class ChildRotateScript : MonoBehaviour
{
    [SerializeField, Tooltip("Time in seconds to make a full rotation")] private float rotationLength = 0.33f;
    [SerializeField, Tooltip("Distance of orbs from player")] private float orbDistance = 2f;
    
    // Update is called once per frame
    void Update()
    {
        // Get the current rotation around the y-axis
        float currentRotation = transform.rotation.eulerAngles.y;
        // Add extra rotation, also take into account that it takes 360 degrees to reach an entire rotation
        float alteredRotation = currentRotation + Time.deltaTime * rotationLength * 360;
        // Set the current rotation to this
        transform.rotation = Quaternion.Euler(0, alteredRotation, 0);
    }

    private void OnEnable()
    {
        transform.GetChild(0).localPosition = new Vector3(0, 0, orbDistance);
    }
}
