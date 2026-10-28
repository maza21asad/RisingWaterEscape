using System;
using UnityEngine;

public enum GameState { Menu, Playing, Won, Failed }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; } = GameState.Menu;

    // Static events: subscribers don't depend on Awake order
    public static event Action<GameState> StateChanged;
    public static event Action LevelReset;

    [SerializeField] private MainMenuController menu;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Announce the starting state so listeners can set themselves up
        StateChanged?.Invoke(State);
    }

    // Hooked to the menu's onPlay and to the Restart button later
    public void StartGame()
    {
        LevelReset?.Invoke();
        SetState(GameState.Playing);
    }

    public void Restart() => StartGame();

    public void GoToMenu()
    {
        LevelReset?.Invoke();
        SetState(GameState.Menu);
        menu.Show();
    }

    public void Win()
    {
        if (State != GameState.Playing) return;
        SetState(GameState.Won);
    }

    public void Fail()
    {
        if (State != GameState.Playing) return;
        SetState(GameState.Failed);
    }

    private void SetState(GameState next)
    {
        State = next;
        Debug.Log($"Game state: {State}");

        var sound = SoundManager.Instance;
        if (sound != null)
        {
            if (next == GameState.Playing) sound.PlayMusic();
            else if (next == GameState.Won) sound.PlayWin();
            else if (next == GameState.Failed) sound.PlayFail();
        }

        StateChanged?.Invoke(State);
    }

    // Right-click the component header in play mode to test without any UI
    [ContextMenu("Debug Win")] private void DebugWin() => Win();
    [ContextMenu("Debug Fail")] private void DebugFail() => Fail();
}