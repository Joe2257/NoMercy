using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

//The level manager perform all the level related tasks like:
// Loading Scenes and keeping track of every objective completed during a level or a full level completion, 
// Loading screens, objectives and related images.
// Start the game saving process when checkpoints are reached and loading checkpoints upon death or from the main menu.
// Pause and Unpause the game.
public class Level_Manager : MonoBehaviour
{
    [Header("LoadingScreen")]
    public Text       _loadingProgressText;
    public Slider     _progressSlider;
    public GameObject _loadingScreen;
    public GameObject _pauseScreen;
    public RawImage   _loadingScreenCurrentImg;
    public Texture    _cityTexture;
    public Texture    _sewerTexture;
    public Texture    _wastelandTexture;
    public Button     _loadingScreenButton;

    [Header("Mission")]
    public Slider   _araknidHealthSlider;
    public Slider   _mechHealthSlider;
    public RawImage _missionImage;
    public Text     _missionText;
    public Text     _ctulluText;

    public Texture _fuseImage;
    public Texture _generatorImage;
    public Texture _sewerImage;
    public Texture _genesysImage;

    [SerializeField] private string _destroyFuse      = "";
    [SerializeField] private string _destroyGenerator = "";
    [SerializeField] private string _getOutOfSewers   = "";
    [SerializeField] private string _killGenesys      = "";
    [SerializeField] private string _cthulluCity      = "";
    [SerializeField] private string _cthulluSewers    = "";
    [SerializeField] private string _cthulluWasteland = "";

    public SharedVariables _sharVarLM;
    public AudioMixer      _mixer;

    [Header("Save&LoadSystemValues")]
    public AmmoScriptableObj _ammoValues;
    private bool             _isStartingNewGame = false;


    private void Start()
    {
        _sharVarLM._startGame = false;
    }

    //_______________Updates________________\\

    private void Update()
    {
        if (_sharVarLM._cityLevelComplete == true)
        { StartCoroutine(NextLevel("Sewers")); _sharVarLM._cityLevelComplete = false; _loadingScreenCurrentImg.texture = _sewerTexture; _ctulluText.text = _cthulluSewers; _sharVarLM._2ndLevelUnlocked = true; }
        if (_sharVarLM._sewersLevelComplete == true)
        { StartCoroutine(NextLevel("WasteLand")); _sharVarLM._sewersLevelComplete = false; _loadingScreenCurrentImg.texture = _wastelandTexture; _ctulluText.text = _cthulluWasteland; _sharVarLM._3rdLevelUnlocked = true; }
        if (_sharVarLM._wastelandLevelComplete == true)
        { SceneManager.LoadScene("Credits"); _sharVarLM._wastelandLevelComplete = false;}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu();
        }

        if (_missionText)
        {
            MissionTracking();
        }
        else
            return;

        if (_sharVarLM._currentStage == 2 && _sharVarLM._araknidHealthPoints > 0)
        {
            _araknidHealthSlider.gameObject.SetActive(true);
            _araknidHealthSlider.value = _sharVarLM._araknidHealthPoints;     
        }
        else { _araknidHealthSlider.gameObject.SetActive(false); }

        if (_sharVarLM._currentStage == 10 && _sharVarLM._mechHealthPoints > 0)
        {
            _mechHealthSlider.gameObject.SetActive(true);
            _mechHealthSlider.value = _sharVarLM._mechHealthPoints;
        }
        else { _mechHealthSlider.gameObject.SetActive(false); }

    }

    public void LateUpdate()
    {
        if (_sharVarLM._saveGame)
        { SaveGame(); _sharVarLM._saveGame = false; }
    }

    public void MissionTracking()
    {

        if (!_sharVarLM._1stObjectiveComplete && !_sharVarLM._2ndObjectiveComplete && !_sharVarLM._3rdObjectiveComplete)
        {
            _missionImage.texture = _fuseImage;
            _missionText.text     = _destroyFuse;
        }
        if (_sharVarLM._1stObjectiveComplete && !_sharVarLM._2ndObjectiveComplete && !_sharVarLM._3rdObjectiveComplete)
        {
          _missionImage.texture = _generatorImage;
          _missionText.text     = _destroyGenerator;
        }
        if (_sharVarLM._1stObjectiveComplete && _sharVarLM._2ndObjectiveComplete && !_sharVarLM._3rdObjectiveComplete)
        {
          _missionImage.texture = _sewerImage;
          _missionText.text     = _getOutOfSewers;
        }
        if (_sharVarLM._1stObjectiveComplete && _sharVarLM._2ndObjectiveComplete && _sharVarLM._3rdObjectiveComplete)
        {
            _missionImage.texture = _genesysImage;
            _missionText.text     = _killGenesys;
        }
    }

    private void PauseMenu()
    {
        _pauseScreen.SetActive(true);

        Time.timeScale = 0f;

        AudioListener.pause = true;
        Cursor.visible = true;
        _sharVarLM._gameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;
    }

    //_______________Button________________\\

    public void OnStartButtonClick()
    {
        ResetSharedVariables();
        StartCoroutine(NextLevel("City"));
        _loadingScreenCurrentImg.texture = _cityTexture;
        _ctulluText.text = _cthulluCity;
        Time.timeScale = 0f;
    }

    public void OnSettingsButtonClick()
    {
        SceneManager.LoadScene("Settings");
    }

    public void OnCreditsButtonClick()
    {
        SceneManager.LoadScene("Credits");
    }

    public void OnLoadCheckpointButtonClick()
    {
        LoadGame();
    }

    public void OnStartGameButtonClick()
    {
        _loadingScreenButton.gameObject.SetActive(false);
        _loadingScreen.SetActive(false);

        _sharVarLM._startGame    = true;
        _sharVarLM._gameIsPaused = false;

        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnReloadLevelButtonClick( string levelName)
    {
        if (!_sharVarLM._2ndLevelUnlocked && !_sharVarLM._3rdLevelUnlocked)
        {
            _sharVarLM._1stObjectiveComplete = false;
            _sharVarLM._2ndObjectiveComplete = false;
            _sharVarLM._3rdObjectiveComplete = false;
            _sharVarLM._newWeaponUnlocked = -1;
            _sharVarLM._enginesDestroyed1 = 0;
            _sharVarLM._enginesDestroyed2 = 0;
            _sharVarLM._isBossDead = false;
        }

        if (!_sharVarLM._3rdLevelUnlocked)
        {
            _sharVarLM._3rdObjectiveComplete = false;
        }

        _sharVarLM._cityLevelComplete = false;
        _sharVarLM._sewersLevelComplete = false;
        _sharVarLM._wastelandLevelComplete = false;
        _sharVarLM._easterEggLevelComplete = false;

        _sharVarLM._rpgFound = false;
        StartCoroutine(NextLevel(levelName));

        if (!_sharVarLM._cityLevelComplete)
        {
            _sharVarLM._resetReserves = true;
        }
    }

    public void OnResumeButtonClick()
    {
        _pauseScreen.SetActive(false);
        _sharVarLM._gameIsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    //_______________Save&Load________________\\

    public void SaveGame()
    {
        
        SaveSystem.SaveSharedData(_sharVarLM);

        Debug.Log ("GameSaved");
    }

    public void LoadGame()
    {
       if (_sharVarLM._gameDataFileCreated)
       {
          GameData gameData = SaveSystem.LoadGameData();
         
          _sharVarLM._cityLevelComplete      = gameData._cityLevel;
          _sharVarLM._sewersLevelComplete    = gameData._sewersLevel;
          _sharVarLM._wastelandLevelComplete = gameData._wastelandLevel;
          _sharVarLM._2ndLevelUnlocked       = gameData._2ndLevelUnlocked;
          _sharVarLM._3rdLevelUnlocked       = gameData._3rdLevelUnlocked;
          _sharVarLM._currentStage           = gameData._currentStage;
          _sharVarLM._isDestroyed            = false;


          _sharVarLM._pistolAmmo  = gameData._pistolAmmo;
          _sharVarLM._shotgunAmmo = gameData._shotgunAmmo;
          _sharVarLM._umpAmmo     = gameData._umpAmmo;
          _sharVarLM._ak47Ammo    = gameData._ak47Ammo;
          _sharVarLM._sniperAmmo  = gameData._sniperAmmo;
          _sharVarLM._rpgAmmo     = gameData._rpgAmmo;
         
         
          if (!_sharVarLM._2ndLevelUnlocked && !_sharVarLM._3rdLevelUnlocked)
          {StartCoroutine(NextLevel("City")); _loadingScreenCurrentImg.texture = _cityTexture; _ctulluText.text = _cthulluCity;}
          if (_sharVarLM._2ndLevelUnlocked && !_sharVarLM._3rdLevelUnlocked)
          {StartCoroutine(NextLevel("Sewers")); _loadingScreenCurrentImg.texture = _sewerImage; _ctulluText.text = _cthulluSewers;} 
          if (_sharVarLM._3rdLevelUnlocked)
          { StartCoroutine(NextLevel("Wasteland")); _loadingScreenCurrentImg.texture = _wastelandTexture; _ctulluText.text = _cthulluWasteland;} 
         
         
          Vector3 position;
          position.x = gameData._playerPos[0];
          position.y = gameData._playerPos[1];
          position.z = gameData._playerPos[2];
         
          _ammoValues._pistolReserve = _sharVarLM._pistolAmmo;
          _ammoValues._sgReserve     = _sharVarLM._shotgunAmmo;
          _ammoValues._smgReserve    = _sharVarLM._umpAmmo;
          _ammoValues._akReserve     = _sharVarLM._ak47Ammo;
          _ammoValues._SPReserve     = _sharVarLM._sniperAmmo;
          _ammoValues._rpgReserve    = _sharVarLM._rpgAmmo;
         
          _sharVarLM._playerPos = position;
          _sharVarLM._playerHp  = gameData._playerHP;
         
          _sharVarLM._granadeCount      = gameData._granadeCount;
          _sharVarLM._newWeaponUnlocked = gameData._weaponsUnlocked;
          _sharVarLM._enginesDestroyed1 = gameData._engineDestroyedS1;
          _sharVarLM._enginesDestroyed2 = gameData._engineDestroyedS2;
         
         
          _sharVarLM._1stObjectiveComplete   = gameData._1stObjective;
          _sharVarLM._2ndObjectiveComplete   = gameData._2ndObjective;
          _sharVarLM._3rdObjectiveComplete   = gameData._3rdObjective;
         
          _sharVarLM._playerIsLoaded = true;

       }
    }

    private IEnumerator NextLevel(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        _loadingScreen.SetActive(true);


        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            _progressSlider.value     = progress;
            _loadingProgressText.text = progress * 100f + "%";

            yield return null;

            if (operation.isDone)
            { _sharVarLM._isGameReady = true; }
        }
    }

    //Reset the whole SharedVariables scriptable object to it's default values.
    private void ResetSharedVariables()
    {
        _sharVarLM._currentStage           = 0;
        _sharVarLM._1stObjectiveComplete   = false;
        _sharVarLM._2ndObjectiveComplete   = false;
        _sharVarLM._3rdObjectiveComplete   = false;
        _sharVarLM._cityLevelComplete      = false;
        _sharVarLM._sewersLevelComplete    = false;
        _sharVarLM._wastelandLevelComplete = false;
        _sharVarLM._easterEggLevelComplete = false;
        _sharVarLM._2ndLevelUnlocked       = false;
        _sharVarLM._3rdLevelUnlocked       = false;
        _sharVarLM._enginesDestroyed1      = 0;
        _sharVarLM._enginesDestroyed2      = 0;
        _sharVarLM._newWeaponUnlocked      = -1;
        _sharVarLM._isBossDead             = false;
        _sharVarLM._rpgFound               = false;
        _sharVarLM._resetReserves          = true;
    }

}
