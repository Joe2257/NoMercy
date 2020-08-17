using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//This script is used by 2 diffent AI's. The first is a common weak enemy("Zombie"),
//the second is a stronger version of the Zombie ("Reptile") the _isReptile box need to be checked in the Editor
// to enable the reptile behaviors.
//If the _isInPack bool is true, there will be a single playerDetection collider for multiple AI that when triggered 
// will alert all the AI's of the same group.
//If the _randomPatrol bool is true, the AI will not follow move between random waypoints.
// (I made this feature to make some AI's of the same waypoint group spread in different point of the stage and not follow always the same path as the others).
public class Zombie_AI : Main_AI
{
    public SharedVariables _zombieSharVar;

    [Header("Movement/General")]
    [SerializeField] private bool  _randomPatrol;
    [SerializeField] private bool  _isStatic;
    [SerializeField] private bool  _isInPack;
    [SerializeField] private bool  _isReptile;
    [SerializeField] private float _walkSpeed = 3.0f;
    [SerializeField] private float _runSpeed  = 5.0f;

    [SerializeField] private float _deathTimer = 3.0f;
    [SerializeField] private float _attackTime;

    private int   _currentWaypoint = 0;

    private bool  _attacking  = false;
    private bool  _chasing    = false;
    private bool  _playerSeen = false;

    public GameObject      player;
    public PlayerDetection playerDetec;

    private bool  _isDead       = false;
    private bool  _firstContact = true;
    private float _attackTimer  = 1f;
    

    [Header("Attack")]
    [SerializeField] private float  _zombieHealthPoints = 100f;
    [SerializeField] private string _attackParamenterR  = "";
    [SerializeField] private string _attackParamenterL  = "";

     private int _attackHashR = -1;
     private int _attackHashL = -1;

    public GameObject damageTriggerL = null;
    public GameObject damageTriggerR = null;

    [Header("Audio")]
    public AudioClip _zombieDeath    = null;
    public AudioClip _attackSound    = null;
     //PlayerInSight.
    public AudioClip _zombieAudioPIS = null;

    private AudioSource _zombieAudioSource;


    private bool PathPending = false;
    public NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;
    


    private void Start()
    {
        _anim              = GetComponent<Animator>();
        _navAgent          = GetComponent<NavMeshAgent>();
        _thisCollider      = GetComponent<CapsuleCollider>();
        player             = GameObject.FindWithTag("Player");
        _zombieAudioSource = GetComponent<AudioSource>();

        _attackHashR = Animator.StringToHash(_attackParamenterR);
        _attackHashL = Animator.StringToHash(_attackParamenterL);

        if (!_isInPack)
        playerDetec = GetComponentInChildren<PlayerDetection>();

        if (_waypointsNetwork == null) return;

        SetNextDestination(false);

        if (!_isStatic)
        _anim.SetFloat("Speed", 5.0f);

        damageTriggerL.SetActive(false);
        damageTriggerR.SetActive(false);

    }

    //_______________Updates________________\\

    private void Update()
    {

        ActivateDamageTrigger();
        StateChecks();
    }

    //Set a new waypoint as destination for the navmesh once the current destination has been reached.
    void SetNextDestination(bool increment)
    {
        if (!_waypointsNetwork) return;

        if (!_isStatic)
        {
            int incStep = increment ? 1 : 0;
            Transform nextWaypointTransform = null;

            int nextWaypoint = (_currentWaypoint + incStep >= _waypointsNetwork.Waypoints.Count) ? 0 : _currentWaypoint + incStep;
            nextWaypointTransform = _waypointsNetwork.Waypoints[nextWaypoint];

            if (nextWaypointTransform != null && !_isDead)
            {
                _currentWaypoint = nextWaypoint;
                _navAgent.destination = nextWaypointTransform.position;
                return;
            }
            _currentWaypoint = nextWaypoint;
        }
    }

    //Decide when to change state based on the current healthPoints and player distance.
    private void StateChecks()
    {

        if (_zombieHealthPoints < 75 && !_chasing)
        {
            _navAgent.speed = _runSpeed;
            _anim.SetFloat("Speed", 3);
            _chasing = true;
            Chase();
        }
        else if (_zombieHealthPoints < 100 && !_chasing && _isReptile)
        {
            _navAgent.speed = _runSpeed;
            _anim.SetFloat("Speed", 3);
            _chasing = true;
            Chase();
        }

        if (!_playerSeen && !_attacking && !_isStatic && !_isDead)
        {
            Roam();
        }

        if (playerDetec._targetInRange || _playerSeen)
        {
            _playerSeen = true;
            _chasing = true;
            Chase();
        }

        if (_zombieHealthPoints <= 0.0f)
        {
            Die();
        }
    }

    //Activate damage trigger when animation is in position.
    private void ActivateDamageTrigger()
    {
        if (_attacking)
        {
            if (_anim.GetFloat(_attackHashR) > 0.9f)
            {
                damageTriggerR.SetActive(true);
            }
            else
            { damageTriggerR.SetActive(false); }


            if (_anim.GetFloat(_attackHashL) > 0.9f)
            {
                damageTriggerL.SetActive(true);
            }
            else
            { damageTriggerL.SetActive(false); }
        }
    }

    //Take damage on hit, need to be called by the object that is dealing damage.
    public void ZombieTakeDamage(float amount)
    {
        _zombieHealthPoints -= amount;

        if (_zombieHealthPoints <= 0f)
        {
            Die();
        }
    }

    //_______________States________________\\

    //Roam the map if able or supposed to.
    private void Roam()
    {
        if (!_chasing)
        {
            Debug.Log("Roaming");

            if (_randomPatrol)
            { _currentWaypoint = Random.Range(0, _waypointsNetwork.Waypoints.Count); _randomPatrol = false; }

            _navAgent.speed = _walkSpeed;
            _anim.SetFloat("Speed", 2);

            PathPending = _navAgent.pathPending;
            PathStatus = _navAgent.pathStatus;

            if ((_navAgent.remainingDistance <= _navAgent.stoppingDistance && !PathPending) || PathStatus == NavMeshPathStatus.PathInvalid)
                SetNextDestination(true);
            else
                if (_navAgent.isPathStale)
                SetNextDestination(false);
        }
    }

    //Chase the player if is close enough or if is already been detected but is too far.
    private void Chase ()
    {
        if (_chasing && !_isDead)
        {
            SetNextDestination(false);
            _navAgent.speed = _runSpeed;
            if (_navAgent.destination != player.transform.position && !_isDead)
                _navAgent.SetDestination(player.transform.position);

            if (_firstContact)
            {
              _zombieAudioSource.PlayOneShot(_zombieAudioPIS);
            }

            float _velocity = _navAgent.velocity.magnitude;

            if (_velocity > 0.5f)
            _anim.SetFloat("Speed", 5.0f);

            _firstContact = false;

            Debug.Log("Chasing");
        }
            
    }

    void Die()
    {
        if (_zombieHealthPoints <= 0)
        {
            SetNextDestination(false);

            _isDead               = true;
            _attacking            = false;
            _chasing              = false;
            _navAgent.enabled     = false;
            _thisCollider.enabled = false;

            damageTriggerL.SetActive(false);
            damageTriggerR.SetActive(false);

            Destroy(gameObject, _deathTimer);

            if (_anim.GetFloat ("Die") == 0)
            {_anim.SetFloat("Die", Random.Range(0, 100)); _zombieAudioSource.PlayOneShot(_zombieDeath);} 
        }

    }

    //_______________Triggers________________\\

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Granade")
        {
            _zombieHealthPoints -= 100.0f;
        }
        else
        if (other.gameObject.tag == "AttackTrigger")
        {
            _chasing = false;
            _navAgent.speed = 0.0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "AttackTrigger")
        {
            _anim.SetBool("Attacking", true);
            _anim.SetFloat("Attack", Random.Range(0, 100));

            _navAgent.speed = 0.0f;
            _attacking      = true;

            transform.LookAt(player.transform, Vector3.up);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "AttackTrigger")
        {
            _anim.SetBool("Attacking", false);

            _navAgent.isStopped = false;
            _attacking          = true;
            _chasing            = true;
        }
    }
}
