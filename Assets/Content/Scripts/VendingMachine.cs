using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour {

    public GameObject foodPrefab;
    public GameObject spawnPoint;
    private AudioSource audioSourceMachine;
    public AudioClip machineSound;
    public AudioClip errorSound;
    public bool detectingPlayer;
    public int price = 100;
    private bool keyPressed;

	// Use this for initialization
	void Start () {
        detectingPlayer = false;
        keyPressed = false;
        audioSourceMachine = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxisRaw("Action") > 0 && !keyPressed) {
            keyPressed = true;
            if (detectingPlayer) {
                // Comprobar si el jugador tiene dinero suficiente
                if (GameManager.sharedInstance.playerMoney >= price) {
                    // Restarle dinero
                    GameManager.sharedInstance.ChangeMoney(-price);
                    // Reproducir sonido de dinero
                    audioSourceMachine.PlayOneShot(machineSound);
                    // Esperar unos segundos
                    IEnumerator stopPlayerFor = GameManager.sharedInstance.StopPlayerFor(1.5f);
                    StopAllCoroutines();
                    StartCoroutine(stopPlayerFor);
                    // Sacar comida
                    Invoke("SpawnFood", 2.1f);
                    // Reproducir sonido de máquina
                } else {
                    // Si no tiene dinero suficiente
                    // Reproducir sonido de error
                    audioSourceMachine.PlayOneShot(errorSound);
                }
            }
        }
        if (Input.GetAxisRaw("Action") == 0) {
            keyPressed = false;
        }
    }

    private void SpawnFood() {
        Instantiate(foodPrefab, spawnPoint.transform);
    }

    // Funcionamiento de la máquina expendedora
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "Player1") {
            detectingPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player1") {
            detectingPlayer = false;
        }
    }
}
