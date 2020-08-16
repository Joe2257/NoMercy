using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//All the variables that will be converted into binary during the game saving process.
[System.Serializable]
public class GameData
{
    public bool _cityLevel;
    public bool _sewersLevel;
    public bool _wastelandLevel;
    public bool _1stObjective;
    public bool _2ndObjective;
    public bool _3rdObjective;
    public bool _2ndLevelUnlocked;
    public bool _3rdLevelUnlocked;
    public bool _gameDataFileCreated;

    public float[] _playerPos;
    public float   _playerHP;

    public float _pistolAmmo;
    public float _shotgunAmmo;
    public float _umpAmmo;
    public float _ak47Ammo;
    public float _sniperAmmo;
    public float _rpgAmmo;

    public int _granadeCount;
    public int _weaponsUnlocked;
    public int _currentStage;
    public int _engineDestroyedS1;
    public int _engineDestroyedS2;

    public GameData(SharedVariables sharedVarData)
    {
        _playerHP = sharedVarData._playerHp;

        _playerPos    = new float[3];
        _playerPos[0] = sharedVarData._playerPos.x;
        _playerPos[1] = sharedVarData._playerPos.y;
        _playerPos[2] = sharedVarData._playerPos.z;

        _granadeCount      = sharedVarData._granadeCount;
        _weaponsUnlocked   = sharedVarData._newWeaponUnlocked;
        _engineDestroyedS1 = sharedVarData._enginesDestroyed1;
        _engineDestroyedS2 = sharedVarData._enginesDestroyed2;

        _pistolAmmo  = sharedVarData._pistolAmmo;
        _shotgunAmmo = sharedVarData._shotgunAmmo;
        _umpAmmo     = sharedVarData._umpAmmo;
        _ak47Ammo    = sharedVarData._ak47Ammo;
        _sniperAmmo  = sharedVarData._sniperAmmo;
        _rpgAmmo     = sharedVarData._rpgAmmo;

        _cityLevel           = sharedVarData._cityLevelComplete;
        _sewersLevel         = sharedVarData._sewersLevelComplete ;
        _wastelandLevel      = sharedVarData._wastelandLevelComplete;
        _1stObjective        = sharedVarData._1stObjectiveComplete;
        _2ndObjective        = sharedVarData._2ndObjectiveComplete;
        _3rdObjective        = sharedVarData._3rdObjectiveComplete;
        _currentStage        = sharedVarData._currentStage;
        _2ndLevelUnlocked    = sharedVarData._2ndLevelUnlocked;
        _3rdLevelUnlocked    = sharedVarData._3rdLevelUnlocked;
        _gameDataFileCreated = sharedVarData._gameDataFileCreated;
    }

    
}
