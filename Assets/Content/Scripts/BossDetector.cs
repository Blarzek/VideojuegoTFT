using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDetector : MonoBehaviour {

    public AudioSource bgm;
    public AudioClip bossTheme;
    public GameObject healthBar;
    public GameObject bossCamera;
    public GameObject bossMessage;
    public Character player;
    public GameObject boss;
    private Character bossCharacter;
    public GameObject missileSpawn;
    public float messageDelay = 3;

    // Use this for initialization
    void Start () {
        bossCharacter = boss.GetComponent<Character>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player1") {
            // Bloquear camino
            gameObject.transform.parent.GetComponent<BoxCollider2D>().enabled = true;
            // Cambiar música
            bgm.Stop();
            bgm.clip = bossTheme;
            bgm.Play();
            // Desactivar spawn de misiles
            missileSpawn.SetActive(false);
            // Aparece la barra de vida del jefe
            healthBar.SetActive(true);
            // Cambiar la cámara
            bossCamera.SetActive(true);
            // Activar la IA al jefe
            boss.GetComponent<BossAI>().enabled = true;
            // Mostrar texto de jefe
            Invoke("ShowMessage", 2f);
            // Esperar unos segundos
            player.NoMoveFor(2.5f);
            bossCharacter.NoMoveFor(2.5f);
            gameObject.SetActive(false);
        }
    }

    private void ShowMessage() {
        bossMessage.GetComponent<Animator>().SetTrigger("ShowDialog");
        Invoke("HideMessage", messageDelay);
    }

    private void HideMessage() {
        bossMessage.GetComponent<Animator>().SetTrigger("HideDialog");
    }
}
