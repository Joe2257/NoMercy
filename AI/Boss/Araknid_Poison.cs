using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Move poison blast when instantiated, instantiate poison cloud when the poison blast collide with an object, destroy the cloud after 7 seconds.
public class Araknid_Poison : MonoBehaviour
{
    public GameObject poisonEffect;

    public SharedVariables sharedVar;

    private float _poisonSpeed = 70.0f;

    private bool _spawnPoisonEffect = true;
    private GameObject _poisonSpawn;

    void Update()
    {
       this.transform.Translate(0, 0, _poisonSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        PoisonExplosion();
        Destroy(gameObject, 1f);
    }

    private void PoisonExplosion()
    {
        if (_spawnPoisonEffect)
        {
            _poisonSpawn = Instantiate(poisonEffect, this.transform.position, Quaternion.identity);

            _spawnPoisonEffect = false;

            Destroy(_poisonSpawn, 7f);
        }
    }
}

