using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHolder : MonoBehaviour 
{
	public int selectedWeapon = 0;
    
    private GameObject _newWeapon;
    private Animator _animator;

    public GameObject pistol;
    public GameObject shotGun;
    public GameObject ump;
    public GameObject ak47;
    public GameObject sniper;
    public GameObject rpg;

    public SharedVariables sharedVar;

    private string _weaponImageKey;
    public Text     weaponImageText;

	void Start ()
	{
        _animator = GetComponent<Animator>();

        selectedWeapon = -1;

        SelectWeapon ();
        sharedVar._isReloading = false;
    }

	private void Update ()
	{
        SetWeaponOnInput();
    }

    //Change weapon on Input if there is any stronger or weaker weapon in the weapon inventory.
    //Change the weapon Icon to match the current weapon.
    private void SetWeaponOnInput()
    {
        int previousSelectedWeapon = selectedWeapon;
        weaponImageText.text = _weaponImageKey;

        if (selectedWeapon == 0)
        { _weaponImageKey = "t"; }
        else if (selectedWeapon == 1)
        { _weaponImageKey = "v"; }
        else if (selectedWeapon == 2)
        { _weaponImageKey = "z"; }
        else if (selectedWeapon == 3)
        { _weaponImageKey = "d"; }
        else if (selectedWeapon == 4)
        { _weaponImageKey = "u"; }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && !sharedVar._isReloading)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else if (sharedVar._newWeaponUnlocked > selectedWeapon && selectedWeapon < 4)
            {
                _animator.SetTrigger("TakeWeapon");
                selectedWeapon++;
            }

        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && !sharedVar._isReloading)
        {

            if (selectedWeapon > 0 && selectedWeapon < 5)
            {
                _animator.SetTrigger("TakeWeapon");
                selectedWeapon--;
            }
        }
        if (previousSelectedWeapon != selectedWeapon)
        {

            SelectWeapon();
        }

        if (Input.GetKeyDown(KeyCode.V) && sharedVar._rpgFound == true && !sharedVar._isReloading)
        {
            _animator.SetTrigger("TakeWeapon");
            pistol.SetActive(false);
            shotGun.SetActive(false);
            ump.SetActive(false);
            ak47.SetActive(false);
            sniper.SetActive(false);
            rpg.SetActive(true);
        }
    }
    

	public void SelectWeapon ()
	{
       int _weaponNumber = 0;

        foreach (Transform weapon in transform) 
		{
			if (_weaponNumber == selectedWeapon)
				weapon.gameObject.SetActive (true);
			else
				weapon.gameObject.SetActive (false);

            _weaponNumber++;
		}
	}


    //Pick up weapons.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DroppableWeapon")
        {
            sharedVar._newWeaponUnlocked++;
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "RPG")
        {
            sharedVar._rpgFound = true;
            Destroy(other.gameObject);
            sharedVar._newWeaponUnlocked++;
        }
    }

}
