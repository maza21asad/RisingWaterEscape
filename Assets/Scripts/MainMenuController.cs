using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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
    }

    private void Start()
    {
        // Load saved volumes, then push them to the sound system once
        float music = PlayerPrefs.GetFloat(MusicKey, 0.8f);
        float sfx = PlayerPrefs.GetFloat(SfxKey, 0.8f);

        musicSlider.SetValueWithoutNotify(music);
        sfxSlider.SetValueWithoutNotify(sfx);

        onMusicVolumeChanged.Invoke(music);
        onSfxVolumeChanged.Invoke(sfx);

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
        Click();
        Hide();
        onPlay.Invoke();
    }

    private void OnMusicChanged(float value)
    {
        PlayerPrefs.SetFloat(MusicKey, value);
        onMusicVolumeChanged.Invoke(value);
    }

    private void OnSfxChanged(float value)
    {
        PlayerPrefs.SetFloat(SfxKey, value);
        onSfxVolumeChanged.Invoke(value);
    }

    private void Click() => onButtonClick.Invoke();
}