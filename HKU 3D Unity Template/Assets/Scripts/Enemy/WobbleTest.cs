using UnityEngine;
using System.Collections;
public class WobbleTest : MonoBehaviour
{
    [SerializeField] private float wobbleAngle = 15f;
    [SerializeField] private float wobbleSpeed = 15f;
    [SerializeField] private float wobbleDuration = 0.4f;

    private Quaternion originalRotation;
    private Coroutine wobbleCoroutine;

    void Awake()
    {
        originalRotation = transform.localRotation;
    }

    public void Hit()
    {
        if (wobbleCoroutine != null)
        {
            StopCoroutine(wobbleCoroutine);
        }

        wobbleCoroutine = StartCoroutine(Wobble());
    }

    IEnumerator Wobble()
    {
        float timer = 0f;

        while (timer < wobbleDuration)
        {
            timer += Time.deltaTime;

            float progress = timer / wobbleDuration;
            float angle = Mathf.Sin(timer * wobbleSpeed) * wobbleAngle * (1f - progress);

            transform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        transform.localRotation = originalRotation;
        wobbleCoroutine = null;
    }
}
