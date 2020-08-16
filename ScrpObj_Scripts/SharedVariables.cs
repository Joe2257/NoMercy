using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This scriptable Object is used to keep track of some key variables of the game, like player position or level completion,
// it basically works as a sort of game manager.
[CreateAssetMenu(fileName = "SharedVar", menuName = "ScriptableObjects/SharedVar")]
public class SharedVariables : ScriptableObject
{
    [Header("General")]

    public float   _playerHp;
    public int     _granadeCount;
    public Vector3 _playerPos;
    public bool    _playerIsLoaded = false;
    public bool    _gameIsSaved = false;
    
    public float _mechHealthPoints    = 0;
    public float _araknidHealthPoints = 0;
    public bool  _isBossDead;
    public bool  _hit;
    public bool  _isDestroyed = false;
    public int   _enginesDestroyed1 = 0;
    public int   _enginesDestroyed2 = 0;


    [Header("Weapons")]
    public bool _isSniperActive = false;
    public bool _isReloading = false;
    public bool _rpgFound = false;
    public bool _isReloadingRpg;
    
    public int _newWeaponUnlocked;


    [Header("UI & Settings")]
    public float _volumeSlider = 0;

    public bool _1stObjectiveComplete= false;
    public bool _2ndObjectiveComplete= false;
    public bool _3rdObjectiveComplete = false;

    [Header("Level Manager")]
    public bool _saveGame = false;
    public bool _gameIsPaused = false;
    public bool _resetReserves = false;
    public int _currentStage = 0;


    public bool _cityLevelComplete      = false;
    public bool _2ndLevelUnlocked       = false;
    public bool _sewersLevelComplete    = false;
    public bool _3rdLevelUnlocked       = false;
    public bool _wastelandLevelComplete = false;
    public bool _easterEggLevelComplete = false;
    public bool _isGameReady            = false;
    public bool _startGame              = false;
    public bool _gameDataFileCreated    = false;

    public bool _mainMenuActive = false;

    [Header("AmmoCache")]
    public float _pistolAmmo = 0;
    public float _shotgunAmmo = 0;
    public float _umpAmmo = 0;
    public float _ak47Ammo = 0;
    public float _sniperAmmo = 0;
    public float _rpgAmmo = 0;


}
