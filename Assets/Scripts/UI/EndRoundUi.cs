using UnityEngine;
using UnityEngine.SceneManagement;

public class EndRoundUi : MonoBehaviour
{
    [SerializeField] GameObject victoryLabel;
    [SerializeField] GameObject defeatLabel;

    private void OnEnable()
    {
        RoundManager.OnLose += PlayerLost;
        RoundManager.OnWin += PlayerWon;
    }

    private void OnDisable()
    {
        RoundManager.OnLose -= PlayerLost;
        RoundManager.OnWin -= PlayerWon;
    }

    private void PlayerWon()
    {
        victoryLabel.SetActive(true);
    }

    private void PlayerLost()
    {
        defeatLabel.SetActive(true);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
