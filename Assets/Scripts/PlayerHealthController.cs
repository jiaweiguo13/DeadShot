using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    public int maxHealth, currentHealth = 100;
    private float invincCounter;
    public float invincibleLength;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (invincCounter > 0)
        {
            invincCounter -= Time.deltaTime;
        }

    }

    public void DamagePlayer(int damageAmount)
    {

        currentHealth = currentHealth - damageAmount;
        invincCounter = invincibleLength;

        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }

}

