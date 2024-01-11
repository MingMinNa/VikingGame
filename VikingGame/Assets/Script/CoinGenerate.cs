using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerate : MonoBehaviour
{
    // Start is called before the first frame update
    float rotationSpeed = 50f; // ±ÛÂà³t«×
    void Update()
    {   
        transform.Rotate(Vector3.left, rotationSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Player") )
        {
            PlayerMove player = other.transform.GetComponent<PlayerMove>();
            player.GetCoin();
            Destroy(gameObject);
        }
    }
}
