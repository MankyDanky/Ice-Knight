using TMPro;
using UnityEngine;

public class TMPUnderlay : MonoBehaviour
{
    public TMP_Text text;
    public TMP_Text parentText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();   
    }

    // Update is called once per frame
    void Update()
    {
        text.text = parentText.text;
    }
}
