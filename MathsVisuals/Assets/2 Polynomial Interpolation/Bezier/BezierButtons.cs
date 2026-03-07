using UnityEngine;

/// <summary>
/// Defines functionality of buttons in the bezier scene
/// </summary>
public class BezierButtons : MonoBehaviour
{
    [SerializeField] private BezierMovement bezierBall;

    [Header("Robot Kyle")]
    [SerializeField] private GameObject kyleRobot;
    [SerializeField] private BezierMovement leftFootTarget;
    [SerializeField, Tooltip("The BezierMovement of the right foots target. Used to initiate movement and reset it.")] private BezierMovement rightFootTarget;
    [SerializeField, Tooltip("Used to set the hip X and Z values to the average of the feets positions")] private Transform hipTarget;
    [Tooltip("Value used to keep hip at its default offset from the feet")] private Vector3 hipOffset = Vector3.zero;

    // References to the transforms of the feet so we save some computation power from getting the Transform component every frame at the expense of using more memory
    private Transform rightFootTrans;
    private Transform leftFootTrans;

    // Start is called once before the first frame update
    private void Start()
    {
        // Get the hip offset of the default foot position
        hipOffset = hipTarget.position - GetAverageFoot();
    }

    // Update is called every frame
    private void Update()
    {
        // If the model is disabled, don't do anything in update
        if (!kyleRobot.activeSelf)
            return;

        // If the model is active, make sure that the hip is centered between the feet
        MakeHipCentered();
    }

    public void BeginBezierMovement()
    {
        // Reset ball position in case it isn't already
        bezierBall.ResetPositionToStart();
        // Make the ball move along the bezier curve
        bezierBall.BeginMovement();

        // If the model is disabled, don't do anything else
        if (!kyleRobot.activeSelf)
            return;
        // Reset the leg position in case it isn't already
        rightFootTarget.ResetPositionToStart();
        // Make the leg move along the bezier curve
        rightFootTarget.BeginMovement();
    }

    public void ResetBezierPosition()
    {
        // Move the ball to its starting position
        bezierBall.ResetPositionToStart();
        // If the model is disabled, don't do anything else
        if (!kyleRobot.activeSelf)
            return;
        // Reset the leg position
        rightFootTarget.ResetPositionToStart();
    }

    /// <summary>
    /// Swaps the active state of Kyle the robot
    /// </summary>
    public void SwapKyleState()
    {
        ResetBezierPosition();
        kyleRobot.SetActive(!kyleRobot.activeSelf);
    }

    /// <summary>
    /// Moves the hips to the average foot position + the default offset to keep them at a reasonable place
    /// </summary>
    private void MakeHipCentered()
    {
        hipTarget.position = GetAverageFoot() + hipOffset;
    }

    /// <summary>
    /// Gets the average world position of Kyle's feet
    /// </summary>
    /// <returns>The average world position of Kyle's feet</returns>
    private Vector3 GetAverageFoot()
    {
        return (leftFootTarget.transform.position + rightFootTarget.transform.position) / 2f;
    }
}
