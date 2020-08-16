using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

//This is an example of how the weapons are structured in the game, every weapon has his own script.
public class Sniper : MonoBehaviour
{
    [Header("General")]
    public ParticleSystem muzzleFlash;
    public GameObject     impactEffect;
    public Camera         fpsCam;
    public Camera         sniperCamera;
    public LayerMask      layerMask;
    public Transform      firePoint = null;

    [Header("WeaponStats")]
    [SerializeField] private float damage         = 100.0f;
    [SerializeField] private float fireRate       = 1.0f;
    [SerializeField] private float range          = 150.0f;
    [SerializeField] private float nextTimeToFire = 1f;

    [Header("Audio/UI")]
    public AudioClip sniperAudio       = null;
    public AudioClip sniperReloadAudio = null;
    public Text      ammoTx            = null;
    public Image     sniperMode;

    public AmmoScriptableObj ammoValues;
    public SharedVariables   sharVarWeapon;
    

    private float _inMagBullets;
    private float _maxCapacity = 6;
    private float _reloadTime  = 3f;
    private bool  _reloading   = false;

    private float _bulletsToReload = 0;


    private Animator anim;
    

    private Zombie_AI zombieTarget;
    private MechDamagePoint mechTarget;
    private MK_2257 mk2257Target;
    private Araknid_Boss araknidTarget;
    private DestructibleItem itemTarget;
    private DestructibleItem_4Engine itemTarget4;

    [SerializeField] public UnityStandardAssets.Characters.FirstPerson.MouseLook _mouseLook;

    private void OnEnable()
    {
        _reloading = false;
        sharVarWeapon._isSniperActive = true;
    }

    void Start()
    {
        _inMagBullets = _maxCapacity;
    }

    void Update()
    {
        AmmoCount();
        Fire();
        Reload();
        Aiming();

        _inMagBullets = Mathf.Clamp(_inMagBullets, 0, _maxCapacity);
    }

    //Raycast from firepoint and apply damage if target has been hit.
    //Instantiate weapon impact particle/ Play muzzleflash particle.
    private void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        _bulletsToReload = _maxCapacity - _inMagBullets;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, layerMask))
        {
            Debug.Log(hit.transform.name);

            mechTarget    = null;
            mk2257Target  = null;
            araknidTarget = null;
            zombieTarget  = null;
            itemTarget    = null;
            itemTarget4   = null;

            if (hit.transform.tag == "Mech")
                mechTarget = hit.transform.GetComponent<MechDamagePoint>();
            else if (hit.transform.tag == "MK2257")
                mk2257Target = hit.transform.GetComponent<MK_2257>();
            else if (hit.transform.tag == "Boss")
                araknidTarget = hit.transform.GetComponent<Araknid_Boss>();
            else if (hit.transform.tag == "Zombie")
                zombieTarget = hit.transform.GetComponent<Zombie_AI>();
            else if (hit.transform.tag == "Interactive")
                itemTarget = hit.transform.GetComponent<DestructibleItem>();
            else if (hit.transform.tag == "Interactive4")
                itemTarget4 = hit.transform.GetComponent<DestructibleItem_4Engine>();

            if (mechTarget != null) { mechTarget.MechTakeDamage(damage); }
            else if (zombieTarget != null) { zombieTarget.ZombieTakeDamage(damage); }
            else if (mk2257Target != null) { mk2257Target.RobotTakeDamage(damage); }
            else if (araknidTarget != null) { araknidTarget.AraknidTakeDamage(damage); }
            else if (itemTarget != null) { itemTarget.ItemTakeDamage(damage); }
            else if (itemTarget4 != null) { itemTarget4.ItemTakeDamage(damage); }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 0.1f);
        }
    }

    //Call Shoot method on mouse input.
    //Play animation.
    //Decrease ammo in magazine.
    private void Fire()
    {
        if (Input.GetButton("Fire1") && _inMagBullets > 0 && Time.time >= nextTimeToFire && !_reloading && !sharVarWeapon._gameIsPaused)
        {
            _inMagBullets--;
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            GetComponent<AudioSource>().PlayOneShot(sniperAudio);
            anim.SetBool("ShootSP", true);
        }
        else if (Input.GetButtonUp("Fire1") || _inMagBullets == 0)
        {
            anim.SetBool("ShootSP", false);

        }

    }

    //Display the correct ammo ammount in mag and reserve in the UI.
    void AmmoCount()
    {
        ammoTx.text = "Ammo  " + _inMagBullets.ToString("0") + " /" + ammoValues._SPReserve.ToString("0");
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && _inMagBullets < 6 && ammoValues._SPReserve > 0)
        {

            if (ammoValues._SPReserve > 8)
            {
                ammoValues._SPReserve += _inMagBullets;
                _inMagBullets = _maxCapacity;
                ammoValues._SPReserve -= _maxCapacity;
            }
            else if (ammoValues._SPReserve <= 8)
            {
                _inMagBullets += ammoValues._SPReserve;
                ammoValues._SPReserve -= _bulletsToReload;
            }
            _reloading = true;
            sharVarWeapon._isReloading = true;
            GetComponent<AudioSource>().PlayOneShot(sniperReloadAudio);
            anim.SetTrigger("SpReload");
            StartCoroutine(Reloaded());
        }
    }

    //Play Aim animation on mouse input activating the scope texture and increasing the FOV.
    private void Aiming()
    {
        if (Input.GetButtonDown("Aim"))
        {
            anim.SetBool("AimingSp", true);

            sniperCamera.gameObject.SetActive(true);
            sniperMode.gameObject.SetActive(true);
        }

        if (Input.GetButtonUp("Aim"))
        {
           anim.SetBool("AimingSp", false);

           sniperMode.gameObject.SetActive(false);
           sniperCamera.gameObject.SetActive(false);
        }
    }

    //Disable shooting while reloading.
    private IEnumerator Reloaded()
    {
        yield return new WaitForSeconds(_reloadTime);

        _reloading = false;
        sharVarWeapon._isReloading = false;

    }

    private void OnDisable()
    {
        sharVarWeapon._isSniperActive = false;
    }
}
