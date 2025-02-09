using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour { 
    public void StartGame()
    {
        print("start pressed");
        SceneManager.LoadScene("Dungeon");
        
    }

    public void ExitGame()
    {
        Application.Quit(); // only works for the built game
    }
}
