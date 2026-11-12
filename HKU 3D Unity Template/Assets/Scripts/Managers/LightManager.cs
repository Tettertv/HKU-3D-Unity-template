using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Light2D playerAccentLight;
    [SerializeField] private Volume globalVolume;

    [Header("DefaulLight")]
    [SerializeField] private LightingProfile defaultProfile;

    private LightingProfile currentProfile;
    private Coroutine transitionRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        ApplyProfileImmediate(defaultProfile);
    }

    public void SetProfile(LightingProfile profile)
    {
        if (profile == null)
            return;

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(
            BlendToProfile(profile, profile.transitionDuration)
        );
    }

    public void SetProfileImmediate(LightingProfile profile)
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        ApplyProfileImmediate(profile);
    }

    private void ApplyProfileImmediate(LightingProfile profile)
    {
        currentProfile = profile;

        globalLight.color = profile.globalLightColor;
        globalLight.intensity = profile.globalLightIntensity;

        if (playerAccentLight != null)
            playerAccentLight.intensity = profile.accentLightIntensityMultiplier;

        if (profile.volumeProfile != null)
            globalVolume.profile = profile.volumeProfile;
    }

    private System.Collections.IEnumerator BlendToProfile(
        LightingProfile target,
        float duration
    )
    {
        currentProfile = target;

        Color startColor = globalLight.color;
        float startIntensity = globalLight.intensity;
        float startAccent = playerAccentLight != null
            ? playerAccentLight.intensity
            : 0f;

        VolumeProfile startVolume = globalVolume.profile;

        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            globalLight.color = Color.Lerp(
                startColor,
                target.globalLightColor,
                t
            );

            globalLight.intensity = Mathf.Lerp(
                startIntensity,
                target.globalLightIntensity,
                t
            );

            if (playerAccentLight != null)
            {
                playerAccentLight.intensity = Mathf.Lerp(startAccent, target.accentLightIntensityMultiplier,t);
            }

            time += Time.deltaTime;
            yield return null;
        }

        ApplyProfileImmediate(target);
    }
}
