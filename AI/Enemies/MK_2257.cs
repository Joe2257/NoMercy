using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MK_2257 : Main_AI
{
    [Header("General")]
    [SerializeField] private float _mkHealthPoints = 150f;
    [SerializeField] private float _walkSpeed      = 3.0f;
    [SerializeField] private float _runSpeed       = 6.0f;

    //Need to be checked to stop the ai patrol function;
    [SerializeField] private bool  _isStatic;

    [Header("Audio")]
    public  AudioClip    mkGunAudio;
    public  AudioClip    mKAudioPlayerInS;
     
    private bool _shooting      = false;
    private bool _chasing       = false;
    private bool _firstContact  = true;
    private bool _playerInSight = false;
    private bool _dead          = false;
    private bool _attacking     = false;
    private bool _randomPatrol  = false;

    private float nextShot;
    private float                  _playerDetectedRandomSound;
    [SerializeField] private int   _currentWaypoint = 0;


    [Header("Attack")]
    [SerializeField] private float _burstTime = 2.0f;
    [SerializeField] private float _range = 80.0f;
    [SerializeField] private float _spreadFactorMK = 0.5f;
    [SerializeField] private float _mk2257Damage = 7.0f;
    [SerializeField] private float _nextBurst;


    public LayerMask       _layerMask = ~1 << 12;
    public Transform       _firePoint;
    public ParticleSystem  _muzzFlash;
    public SharedVariables _sharVarMK;

    private GameObject _player;
    private AudioSource _audioSource;

    private bool PathPending            = false;
    public NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;

    private void Start()
    {
        _audioSource  = GetComponent<AudioSource>();
        _thisCollider = GetComponent<CapsuleCollider>();
        _navAgent     = GetComponent<NavMeshAgent>();
        _player       = GameObject.FindWithTag("Player");
        _anim         = GetComponent<Animator>();

        SetNextDestination(false);

        if (!_isStatic)
            _anim.SetFloat("Speed", 3.0f);   
    }

    private void Update()
    {
        StateChecks();
    }

    //Enable different state behaviours based on player distance and AI health points.
    private void StateChecks()
    {
        if (Vector3.Distance(_player.transform.position, transform.position) <= 40.0f && _mkHealthPoints > 0)
        {
            _playerInSight = true;
            Chase();
        }

        if (_mkHealthPoints <= 0)
            Die();

        if (_mkHealthPoints < 150f && !_chasing)
        { Chase(); _firstContact = true; }

        if (!_playerInSight && !_attacking && !_isStatic && !_dead)
        {
            Roam();
        }
    }

    //Set a new waypoint as destination for the navmesh once the current destination has been reached.
    void SetNextDestination(bool increment)
    {
        PathPending = _navAgent.pathPending;
        PathStatus  = _navAgent.pathStatus;

        if (!_waypointsNetwork) return;

        if (!_isStatic)
        {
            int incStep = increment ? 1 : 0;
            Transform nextWaypointTransform = null;

            int nextWaypoint = (_currentWaypoint + incStep >= _waypointsNetwork.Waypoints.Count) ? 0 : _currentWaypoint + incStep;
            nextWaypointTransform = _waypointsNetwork.Waypoints[nextWaypoint];

            if (nextWaypointTransform != null && !_dead)
            {
                _currentWaypoint = nextWaypoint;
                _navAgent.destination = nextWaypointTransform.position;
                return;
            }
            _currentWaypoint = nextWaypoint;
        }
    }

    //_______________States________________\\

    //Roam the map if able or supposed to(Eg.. not in a tower or roof).
    private void Roam()
    {
        if (_randomPatrol)
        { _currentWaypoint = Random.Range(0, _waypointsNetwork.Waypoints.Count); _randomPatrol = false; }

        _navAgent.speed = _walkSpeed;
        _anim.SetFloat("Speed", 3);

        if ((_navAgent.remainingDistance <= _navAgent.stoppingDistance && !PathPending) || PathStatus == NavMeshPathStatus.PathInvalid)
            SetNextDestination(true);
        else
            if (_navAgent.isPathStale)
            SetNextDestination(false);
    }

    //Chase the player if is close enough or if is already been detected but is too far.
    private void Chase()
    {
      if (_chasing)
      { 
        _attacking = false;
        _navAgent.speed = _runSpeed;
        _anim.SetFloat("Speed", 3.0f);
        
        
        if (_navAgent.destination != _player.transform.position)
            _navAgent.destination = _player.transform.position;

        if (_firstContact)
        {
               _audioSource.PlayOneShot(mKAudioPlayerInS);
               _firstContact = false;
        }
      }
        if (Vector3.Distance(_player.transform.position, transform.position) <= 20.0f)
        {
            Attack();
            _chasing = false;
            _attacking = true;
            _playerInSight = true;
        }
        else { _chasing = true; }
    }

    //Start shooting the player when player is close enough.
    private void Attack()
    {
        if (_attacking)
        {
            _navAgent.speed = 0.0f;
            _anim.SetFloat("Speed", 0.0f);

            Vector3 targetPosition = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
            transform.LookAt(targetPosition);

            if (!_shooting)
            { StartCoroutine(Shoot(_burstTime)); _shooting = true; _anim.SetFloat("Attack", 2.0f); }
        }
   
    }

    private void Die()
    {
        if (_mkHealthPoints <= 0)
        {
            _attacking            = false;
            _playerInSight        = false;
            _navAgent.isStopped   = true;
            _thisCollider.enabled = false;

            Destroy(gameObject, 4f);

            if (_dead)
            { _anim.SetFloat("Die", Random.Range(0, 100)); _dead = false; }
        }
    }

    //_______________Attack/DamageMechanics________________\\

    //Raycast burst of bullets toward the player, the position of every raycast is randomized to avoid full aimbot.
    private IEnumerator Shoot(float burst)
    {
        
        for (int i = 0; i < burst; i++)
        {
            Vector3 direction = _player.transform.position;
            direction.x += Random.Range(-_spreadFactorMK, _spreadFactorMK);
            direction.y += Random.Range(-_spreadFactorMK, _spreadFactorMK);
            direction.z += Random.Range(-_spreadFactorMK, _spreadFactorMK);

            RaycastHit hit;
            Ray ray = new Ray(_firePoint.transform.position, direction - _firePoint.transform.position);

            if (Physics.Raycast(ray, out hit, _range, _layerMask))
            {
                Debug.Log(hit.transform.name);
                Debug.DrawLine(_firePoint.transform.position, hit.point, Color.red);
                Debug.DrawRay(hit.point, transform.position, Color.red);

                GetComponent<AudioSource>().PlayOneShot(mkGunAudio);
                _muzzFlash.Play();

                CharControllerPortfolio playerAsTarget = hit.transform.GetComponent<CharControllerPortfolio>();

                if (playerAsTarget != null) { playerAsTarget.TakeDamageFromBB(_mk2257Damage); }

                yield return new WaitForSeconds(0.1f);
            }
        }
        _anim.SetFloat("Attack", 0.0f);
        yield return new WaitForSeconds(2.5f);
        
        _shooting = false;
    }


    //Take damage on hit, need to be called by the object that is dealing damage.
    public void RobotTakeDamage(float amount)
    {
        _mkHealthPoints -= amount;

        if (_mkHealthPoints <= 0f)
        {
            Die();
            _dead = true;
        }

    }

    //_______________Triggers________________\\

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Granade")
        {
            _mkHealthPoints = _mkHealthPoints - 100.0f;
            if (_mkHealthPoints <= 0f)
            {
                Die();
            }
        }

    }

}
