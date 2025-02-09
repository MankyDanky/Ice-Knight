using TMPro;
using UnityEngine;

public class LevelText : MonoBehaviour
{
    TMP_Text text;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = "Level " + GameManager.instance.level;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
