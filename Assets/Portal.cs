using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    bool canTransport;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        canTransport = false;
    }

    void Start() {
        StartCoroutine(setTransport());
    }

    IEnumerator setTransport() {
        yield return new WaitForSeconds(5);
        canTransport = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && canTransport)
        {
            GameManager.instance.level++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
