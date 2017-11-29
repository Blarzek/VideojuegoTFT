using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    public GameObject explosionPrefab;
    public GameObject platform;
    public GameObject shadow;
    public bool explosionOffset = true;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // Animación de la explosión
        GameObject explosion = Instantiate(explosionPrefab);
        if (explosionOffset) {
            explosion.transform.position = new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z);
        } else {
            explosion.transform.position = transform.position;
        }

        // Desaparece su propia plataforma
        Destroy(platform);

        // Desaparece su propia sombra
        Destroy(shadow);

        // Desaparece el misil
        Destroy(gameObject);
    }
}
