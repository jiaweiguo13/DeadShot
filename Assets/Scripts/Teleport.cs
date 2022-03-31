using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform target;
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        player.transform.position = target.transform.position;
    }
}
