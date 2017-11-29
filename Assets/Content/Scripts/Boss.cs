using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

    public enum CannonPosition { Down, Mid, Up };
    public CannonPosition actualPosition;
    public Animator animatorCannon;
    public GameObject explosionPrefab;
    public GameObject missilePrefab;
    public GameObject missilePrefabDown;
    public AudioClip missileShootSound;
    public GameObject spawnDown;
    public GameObject spawnMid;
    public GameObject spawnUp;
    public GameObject player;
    public GameObject skySpawn;
    public GameObject missilePlatform;
    public GameObject shadowPrefab;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Movimiento del cañón
    public void MoveUpMid() {
        actualPosition = CannonPosition.Mid;
        animatorCannon.SetTrigger("up-mid");
    }

    public void MoveUpDown() {
        actualPosition = CannonPosition.Down;
        animatorCannon.SetTrigger("up-down");
    }

    public void MoveMidUp() {
        actualPosition = CannonPosition.Up;
        animatorCannon.SetTrigger("mid-up");
    }

    public void MoveMidDown() {
        actualPosition = CannonPosition.Down;
        animatorCannon.SetTrigger("mid-down");
    }

    public void MoveDownMid() {
        actualPosition = CannonPosition.Mid;
        animatorCannon.SetTrigger("down-mid");
    }

    public void MoveDownUp() {
        actualPosition = CannonPosition.Up;
        animatorCannon.SetTrigger("down-up");
    }

    // Disparo del cañón
    public void ShootDown() {
        animatorCannon.SetTrigger("shootDown");
        // Generar misil
        GameObject missile = Instantiate(missilePrefabDown, spawnDown.transform);
        missile.GetComponent<Rigidbody2D>().gravityScale = 0;
        missile.transform.localRotation = Quaternion.Euler(0, 0, 0);
        // Se desplaza el colisionador
        missile.GetComponent<CapsuleCollider2D>().offset = new Vector2(0.15f, -0.25f);
        // Aplicar fuerza lineal
        missile.GetComponent<Rigidbody2D>().AddForce(new Vector2(-180, 0));
        // Sonido de disparo
        gameObject.GetComponent<AudioSource>().PlayOneShot(missileShootSound, 0.5f);
    }

    public void ShootMid() {
        animatorCannon.SetTrigger("shootMid");
        // Generar plataforma de contacto
        GameObject platform = Instantiate(missilePlatform);
        // Generar misil
        GameObject missile = Instantiate(missilePrefab, spawnMid.transform);
        missile.GetComponent<Missile>().explosionOffset = false;
        // Se le asigna al misil su propia plataforma
        missile.GetComponent<Missile>().platform = platform;
        platform.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        // Se agranda la plataforma
        platform.transform.localScale = new Vector3(3, 0.05f, 1);
        missile.GetComponent<Rigidbody2D>().gravityScale = 0.25f;
        missile.transform.localRotation = Quaternion.Euler(0, 0, 10);
        // Aplicar fuerza parábolica
        float distance = Vector3.Distance(transform.position, player.transform.position);
        missile.GetComponent<Rigidbody2D>().freezeRotation = false;
        missile.GetComponent<Rigidbody2D>().angularVelocity = 20;

        float distanceFactor = 0f;
        if (distance > 7f) {
            distanceFactor = 26f;
        } else if (distance > 6f) {
            distanceFactor = 25f;
        } else if (distance > 5f) {
            distanceFactor = 24f;
        } else if (distance > 4f) {
            distanceFactor = 23f;
        } else {
            distanceFactor = 22f;
        }

        float hForce = -distance * distanceFactor;
        float vForce = 80f;
        missile.GetComponent<Rigidbody2D>().AddForce(new Vector2(hForce, vForce));
        // Sonido de disparo
        gameObject.GetComponent<AudioSource>().PlayOneShot(missileShootSound, 0.5f);
    }

    public void ShootUp() {
        animatorCannon.SetTrigger("shootUp");
        // Generar misil
        GameObject missile = Instantiate(missilePrefab, spawnUp.transform);
        missile.GetComponent<Rigidbody2D>().gravityScale = 0;
        missile.GetComponent<SpriteRenderer>().flipX = true;
        // Aplicar fuerza vertical
        missile.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 150));
        // Sonido de disparo
        gameObject.GetComponent<AudioSource>().PlayOneShot(missileShootSound, 0.5f);
        // Aparece un misil desde el cielo
        Invoke("SpawnFromSky", 2f);
    }

    private void SpawnFromSky() {
        // Obtener posición en la que generar misil basándose en la posición del jugador
        Vector3 skyPosition = new Vector3(player.transform.position.x, skySpawn.transform.position.y, skySpawn.transform.position.z);
        // Generar misil
        skySpawn.transform.position = skyPosition;
        GameObject missile = Instantiate(missilePrefab);
        missile.transform.position = skySpawn.transform.position;
        // Aplicar fuerza vertical
        missile.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -50));
        // Generar plataforma
        GameObject platform = Instantiate(missilePlatform);
        platform.transform.position = new Vector3(skySpawn.transform.position.x, player.transform.position.y, player.transform.position.z);
        // Generar sombra
        GameObject shadow = Instantiate(shadowPrefab);
        shadow.transform.position = new Vector3(skySpawn.transform.position.x, player.transform.position.y, player.transform.position.z);
        // Se le asigna al misil su propia plataforma
        missile.GetComponent<Missile>().platform = platform;
        // Se le asigna al misil su propia sombra
        missile.GetComponent<Missile>().shadow = shadow;
        // Se hace pequeña
        missile.GetComponent<Missile>().platform.transform.localScale = new Vector3(0.4f, 0.05f, 1);
    }
}
