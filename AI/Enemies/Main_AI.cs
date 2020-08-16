using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Inheritance Test.
public class Main_AI : MonoBehaviour
{


    [Header("Generic")]
    public Waypoints_Network _waypointsNetwork = null;
    public Animator _anim;
    public CapsuleCollider _thisCollider;



     public float _attackDist = 3.0f;
     public NavMeshAgent _navAgent = null;
}
