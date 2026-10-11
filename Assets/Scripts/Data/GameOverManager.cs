using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{

    public GameObject gameOverBackground; // Сюда перетащи GameOverBackground из иерархии

    private void Awake()
    {
        if (gameOverBackground != null)
            gameOverBackground.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverBackground == null)
        {
            Debug.LogError("[GameOverManager] Не назначен GameOverBackground в инспекторе!");
            return;
        }

        gameOverBackground.SetActive(true);
        Time.timeScale = 0f; // Останавливаем время
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
