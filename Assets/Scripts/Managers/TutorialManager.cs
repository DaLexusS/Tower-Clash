using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TowerManager towerManager;

    private TutorialStep currentStep;

    private bool waitingForInput = false;

    private enum TutorialStep
    {
        None,
        Intro,
        SpawnEnemy,
        ExplainCombat,
        AllowPlayerAction,
        Done
    }

    private void Start()
    {
        // Disable automatic gameplay loop
        RoundManager.Instance.autoSpawnEnabled = false;

        // Start paused
        RoundManager.Instance.PauseGame();

        SetStep(TutorialStep.Intro);
    }

    private void Update()
    {
        // Example input trigger (replace with UI button later)
        if (waitingForInput && Input.GetMouseButtonDown(0))
        {
            waitingForInput = false;
            ContinueStep();
        }
    }

    // =========================
    // STEP CONTROLLER
    // =========================
    private void SetStep(TutorialStep step)
    {
        currentStep = step;

        switch (currentStep)
        {
            case TutorialStep.Intro:
                Step_Intro();
                break;

            case TutorialStep.SpawnEnemy:
                Step_SpawnEnemy();
                break;

            case TutorialStep.ExplainCombat:
                Step_ExplainCombat();
                break;

            case TutorialStep.AllowPlayerAction:
                Step_AllowPlayerAction();
                break;

            case TutorialStep.Done:
                Step_Done();
                break;
        }
    }

    private void ContinueStep()
    {
        switch (currentStep)
        {
            case TutorialStep.Intro:
                SetStep(TutorialStep.SpawnEnemy);
                break;

            case TutorialStep.SpawnEnemy:
                SetStep(TutorialStep.ExplainCombat);
                break;

            case TutorialStep.ExplainCombat:
                SetStep(TutorialStep.AllowPlayerAction);
                break;

            case TutorialStep.AllowPlayerAction:
                SetStep(TutorialStep.Done);
                break;
        }
    }

    // =========================
    // STEPS
    // =========================

    private void Step_Intro()
    {
        Debug.Log("Tutorial: Welcome! Click to continue.");
        waitingForInput = true;
    }

    private void Step_SpawnEnemy()
    {
        Debug.Log("Tutorial: Enemy incoming!");

        RoundManager.Instance.UnPauseGame();

        // Spawn 1 enemy in lane 0
        //towerManager.SpawnEnemyMinion(0);

        // Pause again after 2 seconds
        StartCoroutine(PauseAfterDelay(2f, TutorialStep.ExplainCombat));
    }

    private void Step_ExplainCombat()
    {
        Debug.Log("Tutorial: This is how combat works.");
        waitingForInput = true;
    }

    private void Step_AllowPlayerAction()
    {
        Debug.Log("Tutorial: Your turn! Spawn a unit.");

        RoundManager.Instance.UnPauseGame();

        // Here you can enable UI interaction instead of auto-continue
        // Example: summonUi.Enable();

        waitingForInput = true;
    }

    private void Step_Done()
    {
        Debug.Log("Tutorial complete!");

        //towerManager.autoSpawnEnabled = true; // back to normal game
        RoundManager.Instance.UnPauseGame();
    }

    // =========================
    // HELPERS
    // =========================

    private IEnumerator PauseAfterDelay(float delay, TutorialStep nextStep)
    {
        float timer = 0f;

        while (timer < delay)
        {
            if (!RoundManager.Instance.gamePaused)
                timer += Time.deltaTime;

            yield return null;
        }

        RoundManager.Instance.PauseGame();
        SetStep(nextStep);
    }
}