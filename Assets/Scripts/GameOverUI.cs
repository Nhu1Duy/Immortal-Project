using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public void OnRetryClick()
    {
        GameManager.Instance.RetryGame();
    }

    public void OnMenuClick()
    {
        GameManager.Instance.GoToMainMenu();
    }
}