using UnityEngine;

public class MoveObjectSlider : MonoBehaviour
{
    public void MoveYAxis(float value)
    {
        transform.position = new Vector3(transform.position.x, value, transform.position.z);
    }
}
