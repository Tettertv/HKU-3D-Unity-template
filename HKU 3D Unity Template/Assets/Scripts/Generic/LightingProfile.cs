using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "LightingProfile", menuName = "Rendering/Lighting Profile")]
public class LightingProfile : ScriptableObject
{
    [Header("Global Light")]
    public Color globalLightColor = Color.white;
    [Range(0f, 2f)]
    public float globalLightIntensity = 1f;

    [Header("Player Accent Light")]
    [Range(0f, 2f)]
    public float accentLightIntensityMultiplier = 1f;

    [Header("Post Processing")]
    public VolumeProfile volumeProfile;

    [Header("Transition")]
    public float transitionDuration = 0.5f; //Seconds to blend into this profile
}
