using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public bool firstGame = true;

    public bool gamePaused = false;

    public static UnityAction OnLose;
    public static UnityAction OnWin;
    public static UnityAction<bool> onPaused;

    public static RoundManager Instance { get; private set; }
    public static bool GameRunning { get; private set; } = true;

    public static void SetGameRunning(bool value)
    {
        GameRunning = value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        BaseHealthManager.BaseDied += BaseDestroyed;
    }

    private void OnDisable()
    {
        BaseHealthManager.BaseDied -= BaseDestroyed;
    }

    private void BaseDestroyed(bool isPlayer)
    {
        GameRunning = false;

        if (isPlayer)
        {
            OnLose.Invoke();
        }
        else
        {
            OnWin.Invoke();
        }

        firstGame = false;
    }

    public void StartGame()
    {
        GameRunning = true;
    }

    public void ResetFirstTime()
    {
        firstGame = false;
    }

    public void NotFirstTime()
    {
        firstGame = true;
    }

    public void PauseGame()
    {
        gamePaused = true;
        onPaused?.Invoke(gamePaused);
        Debug.Log("Game Paused");
    }

    public void UnPauseGame()
    {
        gamePaused = false;
        onPaused?.Invoke(gamePaused);
        Debug.Log("Game Unpaused");
    }

    private void Update()
    {
        // Debug input: Press "I" to toggle pause
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (gamePaused)
                UnPauseGame();
            else
                PauseGame();
        }
    }
}
