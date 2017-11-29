using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

    public GameObject explosionPrefab;
    public GameObject grenadePlatformPrefab;
    public float duration = 3f;
    private float timer;
    private bool exploding = false;
    private GameObject platform;

    // Use this for initialization
    void Start() {
        timer = duration;
        // Crear plataforma que mantendrá la granada a los pies del personaje
        platform = Instantiate(grenadePlatformPrefab);
        platform.transform.position = GameManager.sharedInstance.player1.transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (timer > 0) {
            timer -= Time.deltaTime;
        } else {
            if (!exploding) {
                exploding = true;
                Explode();
            }
        }
    }

    private void Explode() {
        // Instanciar prefab de explosión
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = transform.position;
        // Destruir la granada
        Destroy(gameObject);
        Destroy(platform);
    }
}
