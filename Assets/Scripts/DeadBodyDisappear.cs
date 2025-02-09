using UnityEngine;

public class DeadBodyDisappear : MonoBehaviour
{
    [SerializeField] public float waitTime = 3.0f;
    private float _originalScaleX = 1.0f;
    private float _originalScaleY = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _originalScaleX = transform.localScale.x;
        _originalScaleY = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;

        if (waitTime <= 0.0f)
        {
            _originalScaleX -= _originalScaleX * Time.deltaTime * 5f;
            _originalScaleY -= _originalScaleY * Time.deltaTime * 5f;

            transform.localScale = new Vector3(_originalScaleX, _originalScaleY, 1);
            if (_originalScaleY <= 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }
}