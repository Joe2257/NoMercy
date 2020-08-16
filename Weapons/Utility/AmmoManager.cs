using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoManager : MonoBehaviour
{
    public AudioClip         ammoSound;
    public AudioSource       ammoAudioSource;
    public AmmoScriptableObj ammoValues;
    public SharedVariables   sharVarAmmo;


    [Header("Reserves text")]
    public Text pistolReserveT;
    public Text shotgunReserveT;
    public Text umpReserveT;
    public Text ak47ReserveT;
    public Text sniperReserveT;
    public Text rpgReserveT;

    private float _pistolAmmo = 0;
    private float _sgAmmo     = 0;
    private float _smgAmmo    = 0;
    private float _ak47Ammo   = 0;
    private float _sniperAmmo = 0;
    private float _rpgAmmo    = 0;

    private float _textDuration = 3.0f;
   
    private void Start()
    {
        pistolReserveT.enabled  = false;
        shotgunReserveT.enabled = false;
        umpReserveT.enabled     = false;
        ak47ReserveT.enabled    = false;
        sniperReserveT.enabled  = false;
        rpgReserveT.enabled     = false;
    }

    private void Update()
    {
        ammoValues._pistolReserve = Mathf.Clamp (ammoValues._pistolReserve, 0, 110);
        ammoValues._sgReserve     = Mathf.Clamp (ammoValues._sgReserve, 0, 350);
        ammoValues._smgReserve    = Mathf.Clamp (ammoValues._smgReserve, 0, 350);
        ammoValues._akReserve     = Mathf.Clamp (ammoValues._akReserve , 0, 210);
        ammoValues._SPReserve     = Mathf.Clamp (ammoValues._SPReserve, 0, 110);
        ammoValues._lmgReserve    = Mathf.Clamp (ammoValues._lmgReserve, 0, 350);
        ammoValues._rpgReserve    = Mathf.Clamp (ammoValues._rpgReserve, 0, 15);


        if (sharVarAmmo._resetReserves)
        {
            ResetReserves();
            sharVarAmmo._resetReserves = false;
        }
    }

    //Reset all the ammo reserves to 0.
    private void ResetReserves()
    {
        ammoValues._pistolReserve = 0;
        ammoValues._sgReserve = 0;
        ammoValues._smgReserve = 0;
        ammoValues._akReserve = 0;
        ammoValues._SPReserve = 0;
        ammoValues._lmgReserve = 0;
        ammoValues._rpgReserve = 0;

        sharVarAmmo._pistolAmmo = 0;
        sharVarAmmo._shotgunAmmo = 0;
        sharVarAmmo._umpAmmo = 0;
        sharVarAmmo._ak47Ammo = 0;
        sharVarAmmo._sniperAmmo = 0;
        sharVarAmmo._rpgAmmo = 0;
    }

    //Give a random ammount of ammo of a random weapon type and activate the UI text for 3 seconds that show which ammo have been collected.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ammo")
        {
            pistolReserveT.enabled = true;
            _pistolAmmo = Random.Range(15f, 30f);
            ammoValues._pistolReserve += _pistolAmmo;
            pistolReserveT.text = "Pistol ammo -> " + _pistolAmmo.ToString("0");

            if (sharVarAmmo._newWeaponUnlocked >= 1)
            {
                _sgAmmo = Random.Range(0f, 12f);
                ammoValues._sgReserve += _sgAmmo;
                shotgunReserveT.enabled = true;
                shotgunReserveT.text = "Shotgun ammo -> " + _sgAmmo.ToString("0");
            }
            if (sharVarAmmo._newWeaponUnlocked >= 2)
            {
                _smgAmmo = Random.Range(0f, 50f);
                ammoValues._smgReserve += _smgAmmo;
                umpReserveT.enabled = true;
                umpReserveT.text = "Ump ammo -> " + _smgAmmo.ToString("0");
            }
            if (sharVarAmmo._newWeaponUnlocked >= 3)
            {
                _ak47Ammo = Random.Range(0f, 60f);
                ammoValues._akReserve += _ak47Ammo;
                ak47ReserveT.enabled = true;
                ak47ReserveT.text = "Ak47 ammo -> " + _ak47Ammo.ToString("0");
            }
            if (sharVarAmmo._newWeaponUnlocked >= 4)
            {
                _sniperAmmo = Random.Range(0f, 6f);
                ammoValues._SPReserve += _sniperAmmo;
                sniperReserveT.enabled = true;
                sniperReserveT.text = "L_96 ammo -> " + _sniperAmmo.ToString("0");
            }
            if (sharVarAmmo._newWeaponUnlocked >= 5)
            {
                _rpgAmmo = Random.Range(0f, 2f);
                ammoValues._rpgReserve += _rpgAmmo;
                rpgReserveT.enabled = true;
                rpgReserveT.text = "Rpg ammo -> " + _rpgAmmo.ToString("0");
            }

            ammoAudioSource.PlayOneShot(ammoSound);
            Destroy(other.gameObject);

            StartCoroutine(AmmoTimer());
        }
    }

    //Disable the ammo UI text after _textDuration has Expired.
    private IEnumerator AmmoTimer()
    {
        yield return new WaitForSeconds(_textDuration);

        pistolReserveT.enabled = false;
        shotgunReserveT.enabled = false;
        umpReserveT.enabled = false;
        ak47ReserveT.enabled = false;
        sniperReserveT.enabled = false;
        rpgReserveT.enabled = false;
    }
}
