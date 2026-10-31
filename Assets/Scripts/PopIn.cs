using UnityEngine;

public class PopIn : MonoBehaviour
{
    [SerializeField] private float duration = 0.25f;
    private float t;

    private void OnEnable()
    {
        t = 0f;
        transform.localScale = Vector3.one * 0.8f;
    }

    private void Update()
    {
        if (t >= 1f) return;
        t += Time.deltaTime / duration;
        float k = Mathf.Clamp01(t);
        float overshoot = 1f + 0.08f * Mathf.Sin(k * Mathf.PI);    // small bounce
        transform.localScale = Vector3.one * Mathf.Lerp(0.8f, 1f, k) * overshoot;
    }
}