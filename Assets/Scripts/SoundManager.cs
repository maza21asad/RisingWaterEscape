using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip land;
    [SerializeField] private AudioClip coin;
    [SerializeField] private AudioClip win;
    [SerializeField] private AudioClip fail;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
    }

    // Hooked to the menu sliders
    public void SetMusicVolume(float v) => musicSource.volume = v;
    public void SetSfxVolume(float v) => sfxSource.volume = v;

    public void PlayMusic()
    {
        if (music == null || musicSource.isPlaying) return;
        musicSource.clip = music;
        musicSource.Play();
    }

    public void PlayClick() => Play(click);
    public void PlayJump() => Play(jump, true);
    public void PlayLand() => Play(land, true);
    public void PlayCoin() => Play(coin, true);
    public void PlayWin() => Play(win);
    public void PlayFail() => Play(fail);

    private void Play(AudioClip clip, bool varyPitch = false)
    {
        if (clip == null) return;
        sfxSource.pitch = varyPitch ? Random.Range(0.95f, 1.05f) : 1f;
        sfxSource.PlayOneShot(clip);
    }
}