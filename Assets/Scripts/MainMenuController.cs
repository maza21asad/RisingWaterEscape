using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button quitButton;

    [Header("Sliders (0 to 1)")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Hooks (wire these in the Inspector)")]
    public UnityEvent onPlay;
    public UnityEvent onButtonClick;
    public UnityEvent<float> onMusicVolumeChanged;
    public UnityEvent<float> onSfxVolumeChanged;

    private const string MusicKey = "vol_music";
    private const string SfxKey = "vol_sfx";

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        settingsButton.onClick.AddListener(() => { Click(); ShowSettings(); });
        backButton.onClick.AddListener(() => { Click(); ShowMain(); });
        quitButton.onClick.AddListener(() => { Click(); Application.Quit(); });

        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);

#if UNITY_WEBGL
        // Application.Quit does nothing in a browser, so hide the button
        quitButton.gameObject.SetActive(false);
#endif

        // Play a click when the SFX slider is released, so the player hears the new volume
        var trigger = sfxSlider.gameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        entry.callback.AddListener(_ => Click());
        trigger.triggers.Add(entry);
    }

    private void Start()
    {
        // Load saved volumes, then push them to the sound system once
        float music = PlayerPrefs.GetFloat(MusicKey, 0.8f);
        float sfx = PlayerPrefs.GetFloat(SfxKey, 0.8f);

        musicSlider.SetValueWithoutNotify(music);
        sfxSlider.SetValueWithoutNotify(sfx);

        //onMusicVolumeChanged.Invoke(music);
        //onSfxVolumeChanged.Invoke(sfx);
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(music);
            SoundManager.Instance.SetSfxVolume(sfx);
        }

        ShowMain();
    }

    // Public so the game can bring the menu back (e.g. from the win/fail screen)
    public void Show()
    {
        gameObject.SetActive(true);
        ShowMain();
    }

    public void Hide() => gameObject.SetActive(false);

    private void ShowMain()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    private void ShowSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void OnPlayClicked()
    {
        Hide();
        onPlay.Invoke();
        Click();
    }

    private void OnMusicChanged(float value)
    {
        PlayerPrefs.SetFloat(MusicKey, value);
        if (SoundManager.Instance != null) SoundManager.Instance.SetMusicVolume(value);
    }

    private void OnSfxChanged(float value)
    {
        PlayerPrefs.SetFloat(SfxKey, value);
        if (SoundManager.Instance != null) SoundManager.Instance.SetSfxVolume(value);
    }

    private void Click()
    {
        onButtonClick.Invoke();
        // Browsers only allow audio after a click, so the first click starts the music.
        // PlayMusic ignores the call if it is already playing.
        if (SoundManager.Instance != null) SoundManager.Instance.PlayMusic();
    }
}