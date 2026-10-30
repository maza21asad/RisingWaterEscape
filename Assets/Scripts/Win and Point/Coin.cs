using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static event Action Collected;

    [SerializeField] private float spinSpeed = 120f;
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float bobSpeed = 3f;

    private Vector3 startPos;
    private bool collected;
    private Collider col;
    private Renderer[] renderers;

    private void Awake()
    {
        startPos = transform.position;
        col = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnEnable() { GameManager.LevelReset += ResetCoin; }
    private void OnDisable() { GameManager.LevelReset -= ResetCoin; }

    private void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
        Vector3 p = startPos;
        p.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = p;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (other.GetComponent<CatController>() == null) return;
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing) return;

        collected = true;
        SetVisible(false);
        if (SoundManager.Instance != null) SoundManager.Instance.PlayCoin();
        Collected?.Invoke();
    }

    private void ResetCoin()
    {
        collected = false;
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        col.enabled = visible;
        foreach (Renderer r in renderers) r.enabled = visible;
    }
}