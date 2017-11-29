using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadesBox : MonoBehaviour {

    private bool detectingPlayer = false;
    private bool keyPressed = false;
    private bool canPick = true;
    private AudioSource audioSource;
    public AudioClip boxSound;
    public AudioClip reloadSound;
    public Sprite fullBox;
    public Sprite emptyBox;
    public float timer = 0f;
    public float reloadTime = 10f;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (canPick) {
            if (Input.GetAxisRaw("Action") > 0 && !keyPressed) {
                if (detectingPlayer) {
                    keyPressed = true;
                    canPick = false;
                    gameObject.transform.parent.GetComponent<SpriteRenderer>().sprite = emptyBox;
                    // Reproducir sonido de caja
                    audioSource.PlayOneShot(boxSound);
                    // Obtener granada
                    GameManager.sharedInstance.ChangeWeapon(GameManager.Weapon.Grenade);
                }
            }
            if (Input.GetAxisRaw("Action") == 0) {
                keyPressed = false;
            }
        } else {
            // La caja se llena después del tiempo de recarga
            if (timer < reloadTime) {
                timer += Time.deltaTime;
            } else {
                timer = 0f;
                canPick = true;
                audioSource.PlayOneShot(reloadSound, 0.4f);
                gameObject.transform.parent.GetComponent<SpriteRenderer>().sprite = fullBox;
            }
        }


    }

    // Funcionamiento de la caja de granadas
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
