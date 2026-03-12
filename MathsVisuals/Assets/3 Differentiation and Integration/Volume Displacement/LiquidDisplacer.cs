using UnityEngine;
using UnityEngine.Rendering;

public class LiquidDisplacer : MonoBehaviour
{
    [Tooltip("Volume of liquid in mL, as defined by unity meters")] private float liquidVolume = 50f;
    [Tooltip("Angle at the bottom of the glass, equivalent to alpha in the third workshop")] private float glassBottomAngle = 46.483f;

    [Header("Component references")]
    [SerializeField] private Transform liquidBottom;
    [SerializeField] private Transform liquidTop;
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private TextFiller oliveVolumeText;

    private float maxLiquidHeight = 0f;

    private float extraVolumeInLiquid = 0f;

    private void Start()
    {
        glassBottomAngle *= Mathf.Deg2Rad;

        // Basing scale on y, figure out what height is the max for the liquid
        maxLiquidHeight = Vector3.Distance(liquidBottom.position, liquidTop.position) / liquidBottom.localScale.y;

        if (slider != null)
            PourLiquid(slider.value);
    }

    // Is called once per physics iteration for every collider touching this trigger
    private void OnTriggerStay(Collider other)
    {
        // This code will assume all volumes entering are spheres
        // Get the radius of the sphere entering based on its scale
        float otherRadius = other.transform.lossyScale.x / 2f;

        // Get the distance to which this is submerged inside of the liquid
        Vector3 lowestPoint = other.ClosestPoint(liquidBottom.position);
        Vector3 highestPoint = other.ClosestPoint(liquidTop.position);
        float otherSubmergedLength = Vector3.Distance(highestPoint, lowestPoint);

        // Get the volume of the amount submerged
        float submergedVolume = VolumeInLiquid(otherRadius, otherSubmergedLength);
        
        // Add to the sum of volumes submerged in the liquid
        extraVolumeInLiquid += submergedVolume;
    }

    // FixedUpdate is called once per physics iteration
    private void FixedUpdate()
    {
        // Since OnTriggerStay has been called for every collider in this this frame, extraVolumeInLiquid should have the combined m^3 volume of those colliders
        // We'll write it out in mL
        oliveVolumeText.FillText(extraVolumeInLiquid * 1000f * 1000f);

        // And make the liquid adjust its size
        SetSize(extraVolumeInLiquid);

        // Reset extra volume to recount it next frame
        extraVolumeInLiquid = 0f;
    }

    /// <summary>
    /// Gets the volume of a (partially) submerged sphere
    /// </summary>
    /// <param name="sphereRadius"></param>
    /// <param name="amountIn"></param>
    /// <returns></returns>
    private float VolumeInLiquid(float sphereRadius, float amountIn)
    {
        float volume = 0f;

        // If the sphere is fully submerged, get the volume of the entire sphere
        if (amountIn >= 2f * sphereRadius)
            volume = (4f / 3f) * Mathf.PI * Mathf.Pow(sphereRadius, 3f);
        else
            // Calculating the volume of the dome that is the part of the sphere in the liquid
            volume = (Mathf.PI / 3f) * Mathf.Pow(amountIn, 2f) * (3f * sphereRadius - amountIn);

        return volume;
    }

    /// <summary>
    /// Sets the amount of ml liquid in the glass
    /// </summary>
    /// <param name="mlInGlass"></param>
    public void PourLiquid(float mlInGlass)
    {
        liquidVolume = mlInGlass;
        SetSize(extraVolumeInLiquid);
    }

    /// <summary>
    /// Sets the size of the liquid in the glass based on the volume of displaced liquid
    /// </summary>
    /// <param name="extraVolume"></param>
    private void SetSize(float extraVolume = 0f)
    {
        // Get how much space the liquid should take up
        float liquidInGlass = liquidVolume / 1000f / 1000f + extraVolume;

        // Using the formula for volume of cone, isolate the height
        // V = (1/3) * pi * r^2 * h

        // Using tan(alpha) = b/a we can define the radius by height and angle. b is radius, a is height
        // r/h = tan(alpha)
        // r = h * tan(alpha)

        // We can input this definition of r in the formula for the volume of a cone, and then isolate the height
        // V = (1/3) * pi * (h * tan(alpha))^2 * h
        // V = (1/3) * pi * tan(alpha)^2 * h^3
        // V / ( (1/3) * pi * tan(alpha)^2 ) = h^3
        // ( V / ( (1/3) * pi * tan(alpha)^2 ) )^(1/3) = h

        // Rewrite to be Unity math compatible
        // Mathf.Pow( V / ( (1/3) * pi * tan(alpha)^2 ), 1/3 ) = h
        // Mathf.Pow( V / ( (1/3) * pi * Mathf.Pow(tan(alpha), 2) ), 1/3 ) = h 

        // Get the height
        float targetLiquidHeight = Mathf.Pow(liquidInGlass / ((1f / 3f) * Mathf.PI * Mathf.Pow(Mathf.Tan(glassBottomAngle), 2f)), 1f/3f);

        // Scale the liquid based on the target height, so that it takes up the correct volume
        liquidBottom.localScale = Vector3.one * ( targetLiquidHeight < maxLiquidHeight ? targetLiquidHeight / maxLiquidHeight : 1f );
    }
}
