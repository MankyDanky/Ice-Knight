using UnityEngine;
using System.Collections; // for coroutine

public class WeaponClass : MonoBehaviour
{
    [SerializeField] private GameObject AttackInstance;
    [SerializeField] private float FireRate;
    public float Angle = 0;
    private bool isCoolDown;
    //[SerializeField] private float AttackAngle; // serialized field for testing, will make private later

    public void Update() // here for testing, the character should directly call Attack() rather than this checking for left click
    {

        if(Input.GetMouseButtonDown(0) && !isCoolDown)
        {
            Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 Direction = MousePosition - transform.position; // local coords
            float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Attack(angle);
        }
    }


    public void Attack(float Angle)
    {
        GameObject NewAttack = Instantiate(AttackInstance, transform.position, Quaternion.Euler(0, 0, Angle));
        StartCoroutine(CooldownCoroutine());
    }


    private IEnumerator CooldownCoroutine() // track cooldowns separate from logic for waiting
    {
        isCoolDown = true;


        yield return new WaitForSeconds(FireRate);

        isCoolDown = false;
    }

}
