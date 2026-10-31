using UnityEngine;

public class SplashFx : MonoBehaviour
{
    [SerializeField] private ParticleSystem splash;
    [SerializeField] private Transform cat;

    private void OnEnable() { GameManager.StateChanged += OnStateChanged; }
    private void OnDisable() { GameManager.StateChanged -= OnStateChanged; }

    private void OnStateChanged(GameState state)
    {
        if (state != GameState.Failed) return;
        splash.transform.position = cat.position;   // the cat is at the water's surface when it fails
        splash.Play();
    }
}