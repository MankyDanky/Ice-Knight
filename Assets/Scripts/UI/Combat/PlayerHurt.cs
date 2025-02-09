using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class PlayerHurt : MonoBehaviour
{
    private Image _image;

    private float _t = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _t = 0;
        _image = GetComponent<Image>();
        _image.color = Color.clear;
        var camObj = GameObject.Find("Main Camera");
        if (camObj != null)
        {
            var comp = camObj.GetComponent<BasicCamFollow>();
            if (comp != null)
            {
                comp.ApplyShake(UnityEngine.Random.value * 360, magnitude: 4);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        _t += Time.deltaTime * 5;
        _t = Mathf.Clamp(_t, 0, 1);
        float m = 2 * (_t - 0.5f);
        float v = 0.2f * (1 - m * m);
        _image.color = Color.red * v;
        if (_t >= 0.99f)
        {
            Destroy(gameObject);
        }
    }
}