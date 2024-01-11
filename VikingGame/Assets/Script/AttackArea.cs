using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject Player, Camera;
    float time_f;
    Vector3 initPos;
    void Start()
    {
        initPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (time_f <= 1.8f)
            time_f += Time.deltaTime;
        else
        {
            time_f = 0.0f;
            gameObject.SetActive(false);
            transform.localPosition = initPos;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Contains("Viking") && !other.name.Contains("Player"))
        {
            EnemyViking enemy = other.transform.GetComponent<EnemyViking>();
            enemy.GetDamage(10);
        }
    }

    public void throwWeapon()
    {
        gameObject.SetActive(true);
        transform.localPosition = initPos;
        time_f = 0.0f;
        Vector3 direction = Camera.transform.forward;
        direction.y = 0f;
        transform.GetComponent<Rigidbody>().velocity = direction * 2.6f;
    }
}
