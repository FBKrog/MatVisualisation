using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class LagrangeLineDrawer : MonoBehaviour
{
    [SerializeField, Tooltip("How many line positions are between each interest point"), Range(0, 50)] private int pathSmoothness = 10;

    [SerializeField, Tooltip("A transform with a bunch of children that will be used as interest points")] private Transform interestPointParent;
    [SerializeField, Tooltip("A Line Renderer that will draw a path between each interest point")] private LineRenderer pathRenderer;

    private float[] lagrangeCoefficients = null;

    /// <summary>
    /// Makes the LineRenderer show a path that connects all the interest points
    /// </summary>
    public void DrawPath()
    {
        int interestPointCount = interestPointParent.childCount;

        // If there aren't at least 2 interest points, there's nothing to draw a path between
        if (interestPointCount < 2)
            return;

        // Get a number of points equal to pathSmoothness * the amount of paths between interestPoints + 1 point per interest point
        pathRenderer.positionCount = (interestPointCount - 1) * pathSmoothness + interestPointCount;

        // Create a matrix with a height of 2 for the X and Z coordinate, and a length of the amount of interestPoints
        float[][] interestPointXZPositions = new float[][] { new float[interestPointCount],
                                                             new float[interestPointCount] };

        // Go through all the interestpoints and get their coordinates
        for (int i = 0; i < interestPointCount; i++)
        {
            // Get the coordinates of this interest point
            Vector3 coordinates = interestPointParent.GetChild(i).position;
            // Remember both the X and Z coordinate
            interestPointXZPositions[0][i] = coordinates.x;
            interestPointXZPositions[1][i] = coordinates.z;
        }

        Vector3[] pointPositions = new Vector3[pathRenderer.positionCount];

        // Calculate coordinates for every point on the line renderer
        for (int i = 0; i < pathRenderer.positionCount; i++)
        {
            float tValue = (float)i / (float)(pathSmoothness + 1);

            pointPositions[i] = new Vector3(LagrangeCalculation(interestPointXZPositions[0], tValue),
                                            0f,
                                            LagrangeCalculation(interestPointXZPositions[1], tValue));
        }

        pathRenderer.SetPositions(pointPositions);
    }

    /// <summary>
    /// Interpolate using lagrange
    /// </summary>
    /// <param name="points">The points to interpolate from. Every point will have a t value equal to its array index</param>
    /// <param name="t">What value to interpolate at</param>
    /// <returns>The interpolated t value based on the points</returns>
    private float LagrangeCalculation(float[] points, float t)
    {
        int n = points.Length;

        float[] lagrange = new float[n];

        for (int i = 0; i < n; i++)
        {
            lagrange[i] = 1;

            for (int j = 0; j < n; j++)
            {
                // Only do this for i =/= j
                if (i == j)
                    continue;

                lagrange[i] *= (float)(t - j) / (float)(i - j);
            }
        }

        float y = 0;
        for (int i = 0; i < n; i++)
        {
            y += lagrange[i] * points[i];
        }

        return y;
    }

    /// <summary>
    /// Sets the path smoothness
    /// </summary>
    /// <param name="newSmoothness">The amount of interpolation points between interest points</param>
    public void AlterSmoothness(float newSmoothness)
    {
        AlterSmoothness((int)newSmoothness);
    }

    /// <summary>
    /// Sets the path smoothness
    /// </summary>
    /// <param name="newSmoothness">The amount of interpolation points between interest points</param>
    public void AlterSmoothness(int newSmoothness)
    {
        pathSmoothness = newSmoothness;
    }

    /// <summary>
    /// Sets the amount of points on the line renderer to 0
    /// </summary>
    public void RemovePath()
    {
        pathRenderer.positionCount = 0;
    }
}
