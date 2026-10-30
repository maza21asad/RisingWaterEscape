using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject failPanel;

    [Header("HUD texts")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text coinsText;

    [Header("Win screen")]
    [SerializeField] private TMP_Text winTimeText;

    [Header("Buttons")]
    [SerializeField] private Button winRestartButton;
    [SerializeField] private Button winMenuButton;
    [SerializeField] private Button failRestartButton;
    [SerializeField] private Button failMenuButton;

    [SerializeField] private int totalCoins = 3;

    public float ElapsedTime { get; private set; }

    private bool counting;

    private void Awake()
    {
        winRestartButton.onClick.AddListener(OnRestartClicked);
        failRestartButton.onClick.AddListener(OnRestartClicked);
        winMenuButton.onClick.AddListener(OnMenuClicked);
        failMenuButton.onClick.AddListener(OnMenuClicked);
    }

    private void OnEnable()
    {
        GameManager.StateChanged += OnStateChanged;
        GameManager.LevelReset += ResetUI;
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= OnStateChanged;
        GameManager.LevelReset -= ResetUI;
    }

    private void Update()
    {
        if (!counting) return;
        ElapsedTime += Time.deltaTime;
        timerText.text = FormatTime(ElapsedTime);
    }

    private void OnStateChanged(GameState state)
    {
        hudPanel.SetActive(state != GameState.Menu);
        winPanel.SetActive(state == GameState.Won);
        failPanel.SetActive(state == GameState.Failed);
        counting = state == GameState.Playing;

        if (state == GameState.Won)
            winTimeText.text = "Time: " + FormatTime(ElapsedTime);
    }

    private void ResetUI()
    {
        ElapsedTime = 0f;
        timerText.text = FormatTime(0f);
        SetCoins(0);
    }

    // The coin system will call this in the next step
    public void SetCoins(int value)
    {
        coinsText.text = value + " / " + totalCoins;
    }

    private void OnRestartClicked()
    {
        PlayClick();
        GameManager.Instance.Restart();
    }

    private void OnMenuClicked()
    {
        PlayClick();
        GameManager.Instance.GoToMenu();
    }

    private void PlayClick()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
    }

    private static string FormatTime(float t)
    {
        return ((int)(t / 60f)).ToString("00") + ":" + ((int)(t % 60f)).ToString("00");
    }
}