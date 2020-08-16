using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Araknid_Boss : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float _health = 1500.0f;
    [SerializeField] private float _speed  = 4.0f;

    public GameObject poisonExplosion;
    public GameObject mouth;
    public GameObject poisonBreath;

    public PlayerDetection _playerDetection;

    private GameObject     _poisonBreathClone;

    private Animator     _anim;
    private NavMeshAgent _navAgent;
    private GameObject   _player;

    private bool _isDead;

    [Header("Attack")]
    public GameObject  damageTriggerR;
    public GameObject  damageTriggerL;
    public Transform   poisonAttackPos;
    public GameObject  poisonAttack;

    private GameObject _poisonBlast;

    public string _rightPar         = "";
    public string _leftPar          = "";
    public string _poisonParamenter = "";

    private int   _rightHash  = -1;
    private int   _leftHash   = -1;
    private int   _poisonHash = -1;

    [Header("Audio")]
    public  AudioClip    nastyScream;
    public  AudioClip    nastySmash;
    public  AudioClip    nastySpawn;


    private float _specialAttackTimer;
    private float _specialAttack;
    private bool  _doSpecialAttack = false;

    public SharedVariables _araknidSharVar;

    private AudioSource _audioSource;

    private void OnEnable()
    {
        if (poisonExplosion)
        {
            poisonExplosion.SetActive(true);
            Destroy(poisonExplosion, 2.5f);
        }
    }

    private void Start()
    {

        _anim            = GetComponent<Animator>();
        _navAgent        = GetComponent<NavMeshAgent>();
        _player          = GameObject.FindWithTag("Player");
        _audioSource     = GetComponent<AudioSource>();
        _playerDetection = GetComponentInChildren<PlayerDetection>();

        _audioSource.PlayOneShot(nastySpawn);

        _poisonHash = Animator.StringToHash(_poisonParamenter);
        _rightHash  = Animator.StringToHash(_rightPar);
        _leftHash   = Animator.StringToHash(_leftPar);

        damageTriggerL.SetActive(false);
        damageTriggerR.SetActive(false);

        _isDead = false;
    }

    //_______________Updates________________\\

    private void Update()
    {
        ActivateDamageTrigger();

        if (_playerDetection._targetInRange && !_isDead)
        {
            Chase();
        }

        if (Vector3.Distance(transform.position, _player.transform.position) <= 20.0f && !_isDead)
            Attack();

        _araknidSharVar._araknidHealthPoints = _health;
    }

    //Activate damage trigger when animation is in position.
    private void ActivateDamageTrigger()
    {
        if (_anim.GetFloat(_poisonHash) > 0f)
        {
            _poisonBlast = Instantiate(poisonAttack, poisonAttackPos.transform.position, poisonAttackPos.transform.rotation);
        }

        if (_anim.GetFloat(_leftHash) > 0f)
        { damageTriggerR.SetActive(true); _audioSource.PlayOneShot(nastySmash); }
        else if (_anim.GetFloat(_rightHash) < 1f)
        { damageTriggerR.SetActive(false); }

        if (_anim.GetFloat(_rightHash) > 0f)
        { damageTriggerL.SetActive(true); _audioSource.PlayOneShot(nastySmash); }
        else if (_anim.GetFloat(_rightHash) < 1f)
        { damageTriggerL.SetActive(false); }
    }

    //Take damage when weak spot has been hit by a bullet.
    public void AraknidTakeDamage(float amount)
    {
        _health -= amount;

        if (_health <= 0f)
        {
            Die();
        }
    }


    //_______________States________________\\

    //Chase the player.
    private void Chase()
    {
        _navAgent.speed = _speed;

        if (_navAgent.destination != _player.transform.position && !_isDead)
            _navAgent.destination = _player.transform.position;

        _anim.SetFloat("Speed", 3.0f);
    }

    //Attack the player when is in range, start special attack CoRoutine.
    private void Attack()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) <= 12.0f)
        {
            _navAgent.speed = 0;

            _anim.SetFloat("Attack", Random.Range(0, 100));

            this.transform.LookAt(_player.transform.position, Vector3.up);
        }
        else { _anim.SetFloat("Attack", 0);}

        if (!_doSpecialAttack )
        {
            _specialAttackTimer = Random.Range(10.0f, 15.0f);
            StartCoroutine(SpecialAttack(_specialAttackTimer));
        }
    }

    private IEnumerator SpecialAttack(float _specialAttack)
    {
        _anim.SetTrigger("Scream");
        _doSpecialAttack = true;

        yield return new WaitForSeconds(_specialAttack);

        _doSpecialAttack = false;
    }

    private void Die()
    {
        if (_health <= 0)
        {
            _isDead = true;
            _navAgent.isStopped = true;
            _anim.SetTrigger("Die");
            _audioSource.Stop();
            _araknidSharVar._isBossDead = true;

            _poisonBreathClone = Instantiate(poisonBreath,mouth.transform.position, mouth.transform.rotation);
        }
    }
}
