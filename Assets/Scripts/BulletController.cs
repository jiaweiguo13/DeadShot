using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float moveSpeed, lifeTime;

    public Rigidbody theRB;

    public GameObject impactEffect;

    public int damage = 1;

    public bool damageEnemy, damagePlayer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        theRB.velocity = transform.forward * moveSpeed;

        lifeTime -= Time.deltaTime;

        if(lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player" && damagePlayer)
        {
            //Debug.Log("Hit player at " + transform.position);
            PlayerHealthController.instance.DamagePlayer(damage); 
        }

        Destroy(gameObject);
        Instantiate(impactEffect, transform.position + (transform.forward * (-moveSpeed * Time.deltaTime)), transform.rotation);
    }
}
