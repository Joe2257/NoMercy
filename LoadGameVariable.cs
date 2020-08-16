using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Load the game saved Data.
public class LoadGameVariable : MonoBehaviour
{
    public SharedVariables _sharVar;

    void Start()
    {
        GameData gameData = SaveSystem.LoadGameData();

        _sharVar._gameDataFileCreated = gameData._gameDataFileCreated;
    }

    
}
