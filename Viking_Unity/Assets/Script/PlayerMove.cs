using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class PlayerMove : MonoBehaviour
{
    [SerializeField] float sensitivity = 5.0f; // ·Æ¹«ÆF±Ó«×
    [SerializeField] Transform Camera;
    [SerializeField] GameObject AttackSpt;
    [SerializeField] GameObject GroundPrefab,GroundCollection;
    [SerializeField] GameObject EnemyPrefab, EnemyCollection;
    [SerializeField] GameObject HealthBar;
    [SerializeField] GameObject EscPanel;
    [SerializeField] GameObject PlayTime, Coin;
    [SerializeField] GameObject AudioPlayer;
    float rotationX = 0f;
    bool playerState = true; // true: alive, false: dead
    float movingSpeed = 6f;
    float AttackCD = 0f;
    float blood = 200, maxBlood = 200;
    int coin = 0;

    bool FootStop = true;
    float time_f = 0f, time_f2 = 0f, playTime = 0.0f; // f: recover, f2: nextGroundGeneratorTime
    bool onground = true, walking = false, running = false;
    Animator animator;

    Vector3 raycastOrigin;
    Vector3 raycastDirection;
    float raycastLength,raycastLen2;
    // Start is called before the first frame update
    void Start()
    {
        AudioPlayer.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("voiceParameter");
        transform.GetComponent<AudioSource>().volume = 0.6f;
        Cursor.lockState = CursorLockMode.Locked; 
        playerState = true;
        onground = true; walking = false; running = false;
        animator = GetComponent<Animator>();
        AttackCD = 0f;

        raycastLength = 0.1f; // Detect whether in the air
        raycastLen2 = 10f; // Detect whether generate the ground
    }

    // Update is called once per frame
    void Update()
    {
        if (blood <= 0) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (EscPanel.activeSelf == false)
            {
                PlayTime.GetComponent<TextMeshProUGUI>().text = ((int)playTime).ToString();
                Coin.GetComponent<TextMeshProUGUI>().text = (coin).ToString();
                Cursor.lockState = CursorLockMode.None;
                EscPanel.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                EscPanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }
        if(EscPanel.activeSelf == true) {
            return;
        }
        playTime += Time.deltaTime;
        if (AttackCD <= 2.0f )
            AttackCD += Time.deltaTime;
        if (time_f2 <= 2f) time_f2 += Time.deltaTime;
        // rotate camera
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -60f, 50f);
        Camera.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // If player is alive, rotate the player direction.
        if (playerState == true)
        {
            transform.Rotate(Vector3.up * mouseX);
        }

        // player move 
        Vector3 playerForward = Camera.forward;
        playerForward.y = 0f;
        walking = false;
        if (Input.GetKey(KeyCode.C))
        {
            ClearDeadBody();
        }
        if(Input.GetKey(KeyCode.W)) {

            walking = true;
            transform.localPosition += movingSpeed * playerForward.normalized * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.S))
        {
            walking = true;
            transform.localPosition -= movingSpeed * playerForward.normalized * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            walking = true;
            transform.localPosition -= movingSpeed * Vector3.Cross(Vector3.up, playerForward) * Time.deltaTime;
        }
        else if( Input.GetKey(KeyCode.D))
        {
            walking = true;
            transform.localPosition += movingSpeed * Vector3.Cross(Vector3.up, playerForward) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space) && onground) 
        {
            float upwardSpeed = 5f; 
            Vector3 currentVelocity = GetComponent<Rigidbody>().velocity;
            currentVelocity.y = upwardSpeed;
            GetComponent<Rigidbody>().velocity = currentVelocity;
            animator.SetTrigger("Jumpping");
        }

        // check whether player is on Ground 
        raycastOrigin = transform.position;
        raycastDirection = Vector3.down;
        if (Physics.Raycast(raycastOrigin, raycastDirection, out RaycastHit hit, raycastLength))
        {
            if(running == false)
                movingSpeed = 3f;
            onground = true;
        }
        else if(Physics.Raycast(raycastOrigin, raycastDirection, out RaycastHit hit2, raycastLen2))
        {
            movingSpeed = 1f;
            onground = false;
        }
        else if(time_f2 >= 1.5f){
            time_f2 = 0f;
            GameObject enemy = Instantiate(EnemyPrefab,EnemyCollection.transform);
            Vector3 EnemyPos = transform.position + playerForward * 13;
            enemy.transform.position = EnemyPos;
            enemy.SetActive(true);
            GameObject nextground = Instantiate(GroundPrefab, GroundCollection.transform);
            Vector3 pos = transform.position;
            pos.y -= 0.2f;
            pos += playerForward * 3;
            nextground.transform.position = pos;
        }

        // running 
        if (Input.GetKey(KeyCode.LeftShift))
        {
            running = true;
            movingSpeed = 6f;
            time_f = 0f;
        }
        else
        {
            time_f += Time.deltaTime;
            if(time_f > 3f)
            {
                time_f = 0f;
                blood = (blood + 10 > maxBlood) ? (maxBlood) : (blood + 10);
                HealthBar.GetComponent<Slider>().value = blood / maxBlood;
            }
            running = false;
            movingSpeed = 3f;
        }
        if(Input.GetMouseButtonDown(0)) {
            if( AttackCD >= 1.5f)
            {
                AttackCD = 0f;
                AttackSpt.SetActive(true);
                AttackSpt.transform.GetComponent<AttackArea>().throwWeapon();
                animator.SetTrigger("Attack");
            }
        }
        if (running)
            transform.GetComponent<AudioSource>().pitch = 1f;
        else
            transform.GetComponent<AudioSource>().pitch = 0.6f;
        if (walking && FootStop == true)
        {
            FootStop = false;
            transform.GetComponent<AudioSource>().Play();
        }
        else if(walking == false)
        {
            FootStop = true;
            transform.GetComponent<AudioSource>().Stop();
        }
        animator.SetBool("Walking", walking);
        animator.SetBool("Running", running);
    }

    public void GetDamage()
    {
        blood -= 30;
        HealthBar.GetComponent<Slider>().value = blood / maxBlood;
        if(blood <= 0)
        {
            animator.SetTrigger("Dead");
            PlayTime.GetComponent<TextMeshProUGUI>().text = ((int)playTime).ToString();
            Coin.GetComponent<TextMeshProUGUI>().text = (coin).ToString();
            Cursor.lockState = CursorLockMode.None;
            EscPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void GetCoin()
    {
        coin++;
    }

    private void ClearDeadBody()
    {
        foreach(Transform enemy in EnemyCollection.GetComponentsInChildren<Transform>(false)) 
        {
            if (enemy.name.Contains("Viking"))
            {
                EnemyViking spt = enemy.GetComponent<EnemyViking>();
                if (spt.dead) Destroy(enemy.gameObject);
            }
        }
    }
    public void ExitBtn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void RetryBtn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("VikingGame");
    }
}
