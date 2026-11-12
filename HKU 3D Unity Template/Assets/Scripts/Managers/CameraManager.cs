using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    [SerializeField] private Transform cameraShakeRoot;

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

    public void PlayImpact(
        float? timeScale = null,
        float? duration = null,
        float? shakeStrength = null,
        float? shakeDuration = null,
        GameObject overrideVFX = null)
    {
        if (impactRoutine != null)
            StopCoroutine(impactRoutine);

        impactRoutine = StartCoroutine(ImpactRoutine(
            timeScale ?? defaultTimeScale,
            duration ?? defaultDuration,
            shakeStrength ?? defaultShakeStrength,
            shakeDuration ?? defaultShakeDuration
        ));
    }

    private IEnumerator ImpactRoutine(
        float timeScale,
        float duration,
        float shakeStrength,
        float shakeDuration)
    {
        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;

        cameraShakeRoot.DOKill();
        cameraShakeRoot.DOShakePosition(shakeDuration, shakeStrength).SetUpdate(true); //Shake em cam :)

        impactRoutine = null;
    }
}
