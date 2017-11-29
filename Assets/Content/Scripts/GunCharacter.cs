using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCharacter : MonoBehaviour {

    public GameObject bulletPrefab;
    public GameObject spawnPoint;
    public bool canShoot = false;
    private bool shootInCooldown = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Shoot() {
        if (!shootInCooldown) {
            shootInCooldown = true;
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = spawnPoint.transform.position;
            bullet.transform.localScale = new Vector3(transform.parent.localScale.x, 1, 1);
            bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(transform.parent.localScale.x * 80, 0));
            CancelInvoke("FinishCooldown");
            Invoke("FinishCooldown", 1.5f);
            spawnPoint.transform.Find("Smoke").GetChild(0).GetComponent<Animator>().SetTrigger("Impact");
        }
    }

    private void FinishCooldown() {
        shootInCooldown = false;
    }

    // El personaje está a tiro
    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "Player1") {
            canShoot = true;
        }
    }

    // El personaje no está a tiro
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player1") {
            canShoot = false;
        }
    }
}
