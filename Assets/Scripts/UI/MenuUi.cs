using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuUi : MonoBehaviour
{
    public static UnityAction onPressedPlay;
    public void PlayPressed() 
    {
        RoundManager.SetGameRunning(true);

        if (RoundManager.Instance.firstGame)
        {
            SceneManager.LoadScene("FirstTimeScene");
        }
        else
        {
            SceneManager.LoadScene("RoundScene");
        }
    }
}
