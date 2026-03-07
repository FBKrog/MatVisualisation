using UnityEngine;

/// <summary>
/// Moves an object along a bezier curve
/// </summary>
public class BezierMovement : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField, Tooltip("How much time it takes to move from point0 to point3"), Min(0.01f)] private float moveDuration = 1f;

    [Header("Bezier Points")]
    [SerializeField, Tooltip("Equivalent to P_0. The start-point of the bezier-curve.")] private Transform point0;
    [SerializeField, Tooltip("Equivalent to P_1. The direction the curve moves from the start-point.")] private Transform point1;
    [SerializeField, Tooltip("Equivalent to P_2. The direction the curve moves from when reaching the end-point.")] private Transform point2;
    [SerializeField, Tooltip("Equivalent to P_3. The end-point of the curve.")] private Transform point3;

    [Tooltip("Equivalent to t. Where on the Bezier-Curve this currently is."), Range(0f, 1f)] private float currentLength = 0f;

    private bool isMoving = false;

    // Update is called once per frame
    void Update()
    {
        // If this is moving, it should move to the end-point (point3)
        if (isMoving)
        {
            MoveToEndPoint();
        }    
    }

    /// <summary>
    /// Moves one frame closer to end-point along bezier-curve
    /// </summary>
    private void MoveToEndPoint()
    {
        // Make t go up based on the moveDuration
        currentLength += Time.deltaTime / moveDuration;
        // if t is more than or equal to 1, we have reached the end and should stop moving
        if (currentLength >= 1f)
        {
            isMoving = false;
        }

        // Bezier-curves are calculated for one number, the position includes 3, so we calculate each separately
        float posX = BezierCalculation(point0.position.x, point1.position.x, point2.position.x, point3.position.x, currentLength);
        float posY = BezierCalculation(point0.position.y, point1.position.y, point2.position.y, point3.position.y, currentLength);
        float posZ = BezierCalculation(point0.position.z, point1.position.z, point2.position.z, point3.position.z, currentLength);

        // Set this gameObjects position to the calculated one
        transform.position = new Vector3(posX, posY, posZ);
    }

    /// <summary>
    /// Begins movement from point0 to point3 along a bezier-curve
    /// </summary>
    public void BeginMovement()
    {
        isMoving = true;
    }

    /// <summary>
    /// Resets the position of this GameObject to the point0 position
    /// </summary>
    public void ResetPositionToStart()
    {
        transform.position = point0.position;
        currentLength = 0f;
    }

    /// <summary>
    /// Calculates a point on a bezier-curve given 4 points and a t-value.
    /// </summary>
    /// <param name="numA">You know this as P_0. The start-point of the curve.</param>
    /// <param name="numB">Aka P_1. The direction the curve moves from the start-point.</param>
    /// <param name="numC">Aka P_2. The direction the curve moves from when reaching the end-point.</param>
    /// <param name="numD">Aka P_3. The end-point of the curve.</param>
    /// <param name="t">How far along the curve you are.</param>
    /// <returns></returns>
    public static float BezierCalculation(float numA, float numB, float numC, float numD, float t)
    {
        // t is a value between 0 and 1, so let's clamp that
        Mathf.Clamp01(t);

        // Equation for a point on a bezier-curve is: B(t) = (1-t)^3*P_0 + 3(1-t)^2*t*P_1 + 3(1-t)*t^2*P_2 + t^3*P_3, 0<=t<=1
        // The following line is that translated into C#, using numA->numD as P_0->P_1
        float endValue = Mathf.Pow(1-t,3)*numA + 3*Mathf.Pow(1-t,2)*t*numB + 3*(1-t)*Mathf.Pow(t,2)*numC + Mathf.Pow(t,3)*numD;

        return endValue;
    }
}
