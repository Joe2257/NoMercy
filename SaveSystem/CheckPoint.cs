using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script is assigned to every checkpoint and it communicates with the SharedVariables scriptableObject to start the game saving process.
public class CheckPoint : MonoBehaviour
{

    [SerializeField] private int _stage;

    public SharedVariables _sharVar;

    public  Text  _checkPointReached;
    private float _checkPointTextTime  = 5.0f;
    private float _checkPointTextTimer = 5.0f;

    private BoxCollider _boxColl;

    void Start()
    {
        _boxColl = GetComponent<BoxCollider>();

        if (_sharVar._currentStage == 1)
        { _sharVar._enginesDestroyed1 = 3; }

        if (_sharVar._currentStage >= _stage)
            _boxColl.enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _sharVar._currentStage++;
            _sharVar._gameIsSaved = true;
            _sharVar._saveGame    = true;

            StartCoroutine(Deactivate(2.0f));
        }
    }

    private IEnumerator Deactivate(float time)
    {
        _checkPointReached.gameObject.SetActive(true);

        yield return new WaitForSeconds(time);

        _checkPointReached.gameObject.SetActive(false);
        _boxColl.enabled = false;
    }
}
