using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public enum CameraState //States nog invoeren, zodat je tussen focus en rotational kan swappen, of automatisch.
    {
        Rotational,
        Focus,
        State3
    }
    public static CameraManager Instance { get; private set; }

    [Header("Default Hitstop")]
    [SerializeField] private float defaultTimeScale = 0.1f;
    [SerializeField] private float defaultDuration = 0.06f;

    [Header("Default Screen Shake")]
    [SerializeField] private float defaultShakeStrength = 0.25f;
    [SerializeField] private float defaultShakeDuration = 0.12f;

    [Header("References")]
    [SerializeField] private CinemachineImpulseSource impulseSource; //Probs wanna keep it like this directly as i might have to grab the same sort of component if i want different sources.

    private Coroutine impactRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayImpact(float? timeScale = null, float? duration = null,float? shakeStrength = null,float? shakeDuration = null,GameObject overrideVFX = null)
    {
        if (impactRoutine != null)
        {
            StopCoroutine(impactRoutine);
        }

        impactRoutine = StartCoroutine(ImpactRoutine(timeScale ?? defaultTimeScale,duration ?? defaultDuration,shakeStrength ?? defaultShakeStrength,shakeDuration ?? defaultShakeDuration));
    }

    private IEnumerator ImpactRoutine(float timeScale,float duration,float shakeStrength,float shakeDuration) //Might want to add a different parameter here for impulse shapes
    {
        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;

        //I would select the chosen impulse here, maybe have a "currentImpulseSource" get assigned the correct one in a switch case. Could use ints or floats (more readable) for that in the SO

        if (impulseSource != null)
        {
            impulseSource.ImpulseDefinition.ImpulseDuration = shakeDuration;
            
            impulseSource.GenerateImpulseWithForce(shakeStrength);
        }

        impactRoutine = null;
    }
}
