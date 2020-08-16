using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharControllerPortfolio : MonoBehaviour
{

    [Header("Movement/Health")]
    [SerializeField] private float _speed      = 6.0F;
    [SerializeField] private float _walkSpeed  = 6.0F;
    [SerializeField] private float _runSpeed   = 12.0F;
    [SerializeField] private float _jumpSpeed  = 8.0F;
    [SerializeField] private float _gravity    = 20.0F;
    [SerializeField] private float _timeDamage = 4.0f;

    public float _currentHealth;

    //The poison cloud from the Reptile is instantiated by the player object upon hit.
    [Header("ReptilePoison")]
    public GameObject _reptilePoison;

    private float _health             = 100f;
    private float _bloodScreenTrshold = 0;
    private float _controllerHeight   = 0.0f;
    private bool  _isCrouching        = false;
    private bool  _isRunning          = false;
    private bool  _isDead             = false;
    private bool  _hasBeenHit         = false;


    private Camera              _camera = null;
    private Vector3             moveDirection = Vector3.zero;
    private CharacterController _controller;
    private AudioSource         _audioSource;

    //Animator is assigned in the inspector.
    public Animator _fpsAnim;

    [Header("UI")]
    public Slider     healthBar;
    public Text       granades;
    public GameObject gameOver;
    public RawImage   bloodScreen;
    public float      bloodScreenTimer;

    private int _granadeCount = 3;
    
    [Header("Granade")]
    public  GameObject granade;
    public  GameObject granadeLauncher;
    private GameObject _gLauncher;

    [Header("FlashLight")]
    public GameObject  flashLight;
    private bool      _lightOn;

    [Header("Audio")]
    public AudioClip medSound;
    public AudioClip painSound1;
    public AudioClip painSound2;

    private float _painRange;

    public SharedVariables _sharVarPlayer;
    public AmmoScriptableObj _ammoValues;

    //Connect the Player Controller script to the Mouse look script from unity Standard Assets in order to enable mouse pointer rotation.
    [SerializeField] public UnityStandardAssets.Characters.FirstPerson.MouseLook _mouseLook;

    private void Start()
    {
        _currentHealth    = Mathf.Clamp(_currentHealth, 0, 100f);
        _controller       = GetComponent<CharacterController>();
        _audioSource      = GetComponent<AudioSource>();
        _controllerHeight = _controller.height;

        _camera = Camera.main;
        _mouseLook.Init(transform, _camera.transform);

        _currentHealth = _health;

        _speed = _walkSpeed;

        bloodScreen.gameObject.SetActive(false);
        flashLight.SetActive (false);
        _lightOn = false;
    }

    //_______________Updates________________\\

    void Update()
    {
        DataToSave();

        if (!_isDead)
        {
            Inputs();
            Movement();
            HealthAndGranadesCounter();
            GranadeLaunch();
           
            if (_currentHealth <= 0)
                   Die();
        }

        if (_currentHealth > 100.0f)
               _currentHealth = 100.0f;

        if (!_sharVarPlayer._startGame || _isDead)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    //A collection of all the inputs that need constant checks in Update().
    private void Inputs()
    {
        if (Input.GetButtonDown("Aim") && _sharVarPlayer._isSniperActive)
        {
            _mouseLook.XSensitivity = .5f;
            _mouseLook.YSensitivity = .5f;
        }
        if (Input.GetButtonUp("Aim"))
        {
            _mouseLook.XSensitivity = 2;
            _mouseLook.YSensitivity = 2;
        }



        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            SwapWeapon();
        }

        if (Input.GetButtonDown("Crouch"))
        {
            _isCrouching = !_isCrouching;
            _controller.height = _isCrouching == true ? _controllerHeight / 2.0f : _controllerHeight;
        }

        if (Input.GetKeyDown(KeyCode.F))
            FlashLight();
    }

    private void Movement()
    {
         
        if (_controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection).normalized;
            moveDirection *= _speed;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) && _speed == 6.0f)
            {_fpsAnim.SetBool("Walk", true);}
            else
            { _fpsAnim.SetBool("Walk", false);}

            if (Input.GetKey(KeyCode.A))
            {
                _fpsAnim.SetBool("CameraLeft", true);
            }
            else
            { _fpsAnim.SetBool("CameraLeft", false);}


            if (Input.GetKey(KeyCode.D))
            {
                _fpsAnim.SetBool("CameraRight", true);
            }
            else
            { _fpsAnim.SetBool("CameraRight", false);}



            if (Input.GetButton("Jump"))
                moveDirection.y = _jumpSpeed;

        }
        moveDirection.y -= _gravity * Time.deltaTime;
        _controller.Move(moveDirection * Time.deltaTime);

        if (Input.GetButtonDown("Run"))
        {
            _fpsAnim.SetBool("Walk", false);
            _fpsAnim.SetBool("Run", true);
            _speed = _runSpeed;
            _isRunning = true;
        }else if (Input.GetButtonUp("Run"))
        {
            _fpsAnim.SetBool("Run", false);
            _speed = _walkSpeed;
            _isRunning = false;
        }

        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Time.timeScale > Mathf.Epsilon)
            _mouseLook.LookRotation(transform, _camera.transform);

        
    }

    //Display the current ammount of healthpoints and granades in the player UI.
    private void HealthAndGranadesCounter()
    {
        granades.text = "Granades " + _granadeCount.ToString();

        _health -= _currentHealth;
        healthBar.value = _currentHealth;
    }

    //Throw granades.
    private void GranadeLaunch()
    {
        if (Input.GetKeyDown(KeyCode.G) && _granadeCount > 0)
        {
            _gLauncher = Instantiate(granade, granadeLauncher.transform.position, granadeLauncher.transform.rotation) as GameObject;
           _granadeCount--;
        }
    }

    private void Die()
    {
        _isDead = true;

        gameOver.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //_______________Triggers________________\\

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DamageTrigger")
        {
            PainSound();
            _currentHealth -= 15;
            _hasBeenHit = true;
            StartCoroutine(DamageScreen(bloodScreenTimer));
        }
        else if (other.gameObject.tag == "ReptileDamageTrigger")
        {
            PainSound();
            _hasBeenHit = true;

            GameObject _poisonClone = Instantiate(_reptilePoison, transform.position, transform.rotation);
            _currentHealth -= 15;
            Destroy(_poisonClone, 3.0f);
            StartCoroutine(DamageScreen(bloodScreenTimer));
        }

        if (other.gameObject.tag == "MedKit" && _currentHealth < 100)
        {
            _currentHealth += 50.0f;
            _audioSource.PlayOneShot(medSound);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Med" && _currentHealth < 100)
        {
            _currentHealth += 25.0f;
            _audioSource.PlayOneShot(medSound);
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "GranadeAmmo" && _granadeCount <3)
        {
            _granadeCount += 1;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "TimeDamage")
        {
            _currentHealth -= _timeDamage * Time.deltaTime;
            _walkSpeed  = 3;
            _speed = _walkSpeed;
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "TimeDamage" && _isRunning)
        {
            _speed = _runSpeed;
        }
        _walkSpeed = 4;
    }


    //_______________Audio________________\\

    private void PainSound()
    {
        if (_hasBeenHit)
        {
            _painRange = Random.Range(0.0f, 50.0f);
            if (_painRange >= 0 && _painRange <= 25.0f)
                _audioSource.PlayOneShot(painSound1);
            else if (_painRange > 25.0f && _painRange <= 50.0f)
                _audioSource.PlayOneShot(painSound2);
        }

        _hasBeenHit = false;
    }

    //_______________Miscellaneus________________\\

    public void TakeDamageFromAI(float damage)
    {
        _currentHealth -= damage;
        PainSound();
        StartCoroutine(DamageScreen(bloodScreenTimer));
    }

    private void FlashLight()
    {

        if (_lightOn == true)
        {
            flashLight.SetActive(false);

            _lightOn = false;

            Debug.Log("Off");
        }
        else if (_lightOn == false)
        {
            flashLight.SetActive(true);

            Debug.Log("On");

            _lightOn = true;
        }        
    }

    //Trigger the Swap Weapon animation when changing weapons.
    private void SwapWeapon()
    {
        _fpsAnim.SetTrigger("SWPweapon");
    }

    //Enable the bloodscreen when taking damage
    private IEnumerator DamageScreen(float screenTimer)
    {
        bloodScreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(screenTimer);

        bloodScreen.gameObject.SetActive(false);
    }

    //_______________Data________________\\

    //Save the player data that need to be saved.
    private void DataToSave()
    {
        if (_sharVarPlayer._gameIsSaved)
        {
            _sharVarPlayer._playerHp     = _currentHealth;
            _sharVarPlayer._playerPos    = transform.position;
            _sharVarPlayer._granadeCount = _granadeCount;
            _sharVarPlayer._pistolAmmo   = _ammoValues._pistolReserve;
            _sharVarPlayer._shotgunAmmo  = _ammoValues._sgReserve;
            _sharVarPlayer._umpAmmo      = _ammoValues._smgReserve;
            _sharVarPlayer._ak47Ammo     = _ammoValues._akReserve;
            _sharVarPlayer._sniperAmmo   = _ammoValues._sgReserve;
            _sharVarPlayer._rpgAmmo      = _ammoValues._rpgReserve;

            _sharVarPlayer._gameIsSaved = false;
        }

        if (_sharVarPlayer._playerIsLoaded)
        {
            _currentHealth     = _sharVarPlayer._playerHp;
            _granadeCount      = _sharVarPlayer._granadeCount;
            transform.position = _sharVarPlayer._playerPos;

            _sharVarPlayer._playerIsLoaded = false;
        }
    }
}
