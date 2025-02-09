using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int level;
    public int highscore;
    
    void Awake() 
    {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        level = 0;
        highscore = PlayerPrefs.GetInt("highscore", 0);
    }

    public void SaveHighscore() {
        if (level > highscore) {
            highscore = level;
            PlayerPrefs.SetInt("highscore", highscore);
        }
    }
}
