using System.Collections.Generic;
using UnityEngine;

public class OrbCircleScript : MonoBehaviour
{
    [SerializeField, Tooltip("Rotations around player per second")] private float orbRotationalSpeed = 0.33f;
    [SerializeField, Tooltip("Distance of orbs from player")] private float orbDistance = 1f;

    private List<Rigidbody> orbs = new List<Rigidbody>();

    // FixedUpdate is called once per physics iteration (50 times a second)
    void FixedUpdate()
    {
        SpinOrbs();
    }

    /// <summary>
    /// Makes orbs rotate in circle around this gameObject
    /// </summary>
    private void SpinOrbs()
    {
        for (int i = 0; i < orbs.Count; i++)
        {
            // Figure out how many orbs we got, and where this orb should be in the order
            // Orbs should have as much distance to each other as they can
            // Parse one or both to floats so that result also is a float
            float orbOrder = (float)i / (float)orbs.Count;

            // Translate Time.time so that 1 second goes a complete cosine period (0-2pi)
            float orbTime = Time.time * 2 * Mathf.PI;

            // Figure out position of orb based on the current time and rotational speed as well as orbOrder
            float orbPlace = orbTime * orbRotationalSpeed + orbOrder * 2 * Mathf.PI;

            // Get coordinates from this
            float orbX = Mathf.Cos(orbPlace);
            float orbZ = Mathf.Sin(orbPlace);

            // Orbs should be a certain distance away from the player
            orbX *= orbDistance;
            orbZ *= orbDistance;

            // The orbs should float around the players position
            Vector3 orbPosition = transform.position + new Vector3(orbX, 0, orbZ);

            // Move the orb to this position
            orbs[i].MovePosition(orbPosition);
        }
    }

    public void PickUpOrb(Rigidbody orbRb)
    {
        // Remember orb and make it float
        orbs.Add(orbRb);
        orbRb.useGravity = false;
    }

    public void DropOrb(Rigidbody orbRb)
    {
        // Forget orb and make it fall again
        orbs.Remove(orbRb);
        orbRb.useGravity = true;
    }
}
