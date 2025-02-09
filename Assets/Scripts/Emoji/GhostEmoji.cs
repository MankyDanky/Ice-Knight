using UnityEngine;

public class GhostEmoji : MonoBehaviour
{
    private float _x, _y;
    private float _velY = 0.35f;
    private float _timer = 0.5f;
    
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _x = transform.position.x;
        _y = transform.position.y;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            float a = Mathf.Max(0, 1 + _timer * 1.5f);
            _spriteRenderer.color = new Color(1, 1, 1, a);
            if (a < 0.01)
            {
                Destroy(gameObject);
            }
        }
        
        _velY -= _velY * Time.deltaTime * 0.5f; // Controlled decay
        _y += _velY * Time.deltaTime;
        
        // Correct position update
        transform.position = new Vector3(_x, _y, 0f);
    }
}