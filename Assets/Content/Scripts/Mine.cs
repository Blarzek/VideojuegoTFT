using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour {

    public GameObject explosionPrefab;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player1") {
            // Animación de la explosión
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            // Desaparece la mina
            Destroy(gameObject);
        }
    }
}
