using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Detect when the player is close enough to the AI in order to be chased.
public class PlayerDetection : MonoBehaviour
{
    public bool       targetInRange = false;
    public GameObject player;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            targetInRange = true;
        }
    }
}
