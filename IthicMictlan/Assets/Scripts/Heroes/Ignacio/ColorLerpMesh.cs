using UnityEngine;

public class ColorLerpMesh : MonoBehaviour
{
    public Color startColor = Color.yellow;
    public Color endColor = Color.magenta;
    public float lerpDuration = 2f;

    private MeshRenderer meshRenderer;
    private Color originalColors;
    private float lerpTime = 0f;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // Store the original vertex colors
        originalColors = meshRenderer.materials[0].color;

        // Set the initial color
        SetColor(startColor);
    }

    void Update()
    {
        // Increment the lerpTime based on the elapsed time
        lerpTime += Time.deltaTime;

        // Calculate the interpolation factor between 0 and 1
        float lerpFactor = Mathf.Clamp01(lerpTime / lerpDuration);

        // Lerp between the start and end colors
        Color lerpedColor = Color.Lerp(startColor, endColor, lerpFactor);

        // Set the lerped color to the mesh
        SetColor(lerpedColor);

        // Optionally, reset lerpTime or perform additional logic when lerping is complete
        if (lerpTime >= lerpDuration)
        {
            lerpTime = 0f;
            // Optionally, perform additional logic when the lerping is complete
        }
    }

    void SetColor(Color color)
    {
        // Set the color to the mesh renderer
        meshRenderer.materials[0].color = color;
    }

    // Reset the color to the original color
    public void ResetColor()
    {
        SetColor(originalColors);
    }
}
