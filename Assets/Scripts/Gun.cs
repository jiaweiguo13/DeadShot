﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;

    public bool canAutoFire;

    public float fireRate;

    [HideInInspector]
    public float fireCounter;

    public int currentAmmo, pickupAmount;

    public Transform firePoint;

    public float zoomAmount;

    public string gunName;
    void Start()
    {
        
    }

    void Update()
    {
        if(fireCounter > 0)
        {
            fireCounter -= Time.deltaTime;
        }
    }

    public void GetAmmo()
    {
        currentAmmo += pickupAmount;

        UIController.instance.ammoText.text = "AMMO: " + currentAmmo;
    }
}
