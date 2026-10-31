using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    [Header("Star rating")]
    [SerializeField] private float parTime = 30f;
    [SerializeField] private Image[] starImages;     // the 3 star images on the Win panel
    [SerializeField] private Sprite starOn;
    [SerializeField] private Sprite starOff;
    [SerializeField] private TMP_Text winHintText;

    [Header("End screen delay (seconds)")]
    [SerializeField] private float winPanelDelay = 2f;
    [SerializeField] private float failPanelDelay = 1f;

    private Coroutine panelRoutine;

    private int totalCoins;     // counted automatically from the scene
    private int coins;          // collected this round

    public float ElapsedTime { get; private set; }

    private bool counting;

    private void Awake()
    {
        winRestartButton.onClick.AddListener(OnRestartClicked);
        failRestartButton.onClick.AddListener(OnRestartClicked);
        winMenuButton.onClick.AddListener(OnMenuClicked);
        failMenuButton.onClick.AddListener(OnMenuClicked);
    }

    private void Start()
    {
        totalCoins = FindObjectsByType<Coin>(FindObjectsSortMode.None).Length;
        SetCoins(0);
        winHintText.text = "3 stars: collect every coin and finish under " + parTime.ToString("0") + " seconds";
    }

    private void OnEnable()
    {
        GameManager.StateChanged += OnStateChanged;
        GameManager.LevelReset += ResetUI;
        Coin.Collected += OnCoinCollected;
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= OnStateChanged;
        GameManager.LevelReset -= ResetUI;
        Coin.Collected += OnCoinCollected;
    }

    private void Update()
    {
        if (!counting) return;
        ElapsedTime += Time.deltaTime;
        timerText.text = FormatTime(ElapsedTime);
    }

    private void OnStateChanged(GameState state)
    {
        // Cancel a pending panel (for example, if the player restarts during the delay)
        if (panelRoutine != null)
        {
            StopCoroutine(panelRoutine);
            panelRoutine = null;
        }

        hudPanel.SetActive(state != GameState.Menu);
        winPanel.SetActive(false);
        failPanel.SetActive(false);
        counting = state == GameState.Playing;

        if (state == GameState.Won)
        {
            // Calculated now, while the timer and coins are frozen; shown after the delay
            winTimeText.text = "Time: " + FormatTime(ElapsedTime) + "   Coins: " + coins + "/" + totalCoins;
            ShowStars(CalculateStars());
            panelRoutine = StartCoroutine(ShowPanelAfter(winPanel, winPanelDelay));
        }
        else if (state == GameState.Failed)
        {
            panelRoutine = StartCoroutine(ShowPanelAfter(failPanel, failPanelDelay));
        }
    }

    private IEnumerator ShowPanelAfter(GameObject panel, float delay)
    {
        yield return new WaitForSeconds(delay);
        panel.SetActive(true);
        panelRoutine = null;
    }

    private int CalculateStars()
    {
        int stars = 1;                                // finished the level
        if (coins >= totalCoins) stars++;             // collected every coin
        if (ElapsedTime <= parTime) stars++;          // finished fast enough
        return stars;
    }

    private void ShowStars(int count)
    {
        for (int i = 0; i < starImages.Length; i++)
            starImages[i].sprite = i < count ? starOn : starOff;
    }

    private void ResetUI()
    {
        ElapsedTime = 0f;
        timerText.text = FormatTime(0f);
        coins = 0;
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

    private void OnCoinCollected()
    {
        coins++;
        SetCoins(coins);
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