using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Метод для запуска игры
    public void PlayGame()
    {
        SceneManager.LoadScene("Pierce"); // Замените "GameScene" на имя вашей игровой сцены
    }

    // Метод для выхода из игры
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Игра закрыта"); // Для тестирования в редакторе
    }
}
