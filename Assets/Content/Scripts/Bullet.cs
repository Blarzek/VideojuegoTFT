using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private AudioSource audioSource;
    public AudioClip shootImpactSound;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        if (GameManager.sharedInstance.devModeEnabled) {
            transform.Find("DevArea").gameObject.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.gameObject.tag == "Player1") {
            // Restarle vida al personaje
            if (collision.gameObject.GetComponent<Character>().blocking) {
                GameManager.sharedInstance.ChangeHealth(collision.gameObject, -8);
            } else {
                GameManager.sharedInstance.ChangeHealth(collision.gameObject, -20);
            }

            // Sonido de impacto
            audioSource.PlayOneShot(shootImpactSound);
            // Se desactiva el collider
            GetComponent<BoxCollider2D>().enabled = false;
            // Se activa la animacion de impacto
            GetComponent<Animator>().SetTrigger("Impact");
            // Se para la bala
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            // Se desactiva el area de desarrollador
            if (GameManager.sharedInstance.devModeEnabled) {
                transform.Find("DevArea").gameObject.SetActive(false);
            }
        }
    }
}
