using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyViking : MonoBehaviour
{
    [SerializeField] GameObject healthBar;
    [SerializeField] float currentBlood, maxBlood;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject CoinPrefab,CoinCollection;
    Animator animator;
    float distance;
    float movingSpeed = 1.7f;
    bool findPlayer = false;
    bool stiff = false;
    float stiff_t = 0f;
    public bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        healthBar.GetComponent<Slider>().value = currentBlood / maxBlood;
    }

    // Update is called once per frame
    void Update()
    {
        if(dead) return;
        if (stiff)
        {
            stiff_t += Time.deltaTime;
            if(stiff_t >= 2f)
            {
                stiff = false;
                stiff_t = 0f;
            }
            return;
        }
        animator.SetBool("Run", findPlayer);

        distance = Vector3.Distance(transform.position, Player.transform.position);
        if(distance < 10f)
        {
            findPlayer = true;
            if(distance < 1.8f)
            {
                stiff = true;
                animator.SetTrigger("Attack"); 
                Player.GetComponent<PlayerMove>().GetDamage();
                return;
            }
            Vector3 currentPosition = transform.position;
            currentPosition.x += (movingSpeed * Time.deltaTime * (Player.transform.position.x - transform.position.x));
            currentPosition.z += (movingSpeed * Time.deltaTime * (Player.transform.position.z - transform.position.z));
            Vector3 lookDirection = (new Vector3((Player.transform.position.x - transform.position.x), 0f, (Player.transform.position.z - transform.position.z))).normalized;
            transform.LookAt(transform.position + lookDirection);
            transform.position = currentPosition;
        }
        else 
            findPlayer = false;
    }
    public void GetDamage(int damage)
    {
        if(dead) return;
        currentBlood -= damage;
        healthBar.GetComponent<Slider>().value = currentBlood / maxBlood;
        if(currentBlood <= 0)
        {
            // Enemy Die
            dead = true;
            animator.SetBool("Dead", true);
            healthBar.SetActive(false);
            GameObject coin = Instantiate(CoinPrefab, CoinCollection.transform);
            Vector3 coinPos = transform.position;
            coinPos.y += 1f;
            coin.transform.position = coinPos;
        }
    }
}
