using UnityEngine;

public class PickableObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D Target)
    {
        if (Target.CompareTag("Player"))
        {
            //GameObject playerInstance = Target.GetComponent<HealthComponent> 
            print("a");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
