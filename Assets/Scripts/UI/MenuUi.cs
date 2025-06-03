using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuUi : MonoBehaviour
{
    public static UnityAction onPressedPlay;
    public void PlayPressed() 
    {
        RoundManager.SetGameRunning(true);
        SceneManager.LoadScene(1);
    }
}
