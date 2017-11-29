using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour {

    public AudioClip coinsSound;
    public int minMoney = 1;
    public int maxMoney = 10;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player1") {
            // Recoger monedas
            AudioSource.PlayClipAtPoint(coinsSound, transform.position);
            int quantity = Random.Range(minMoney, maxMoney);
            GameManager.sharedInstance.ChangeMoney(quantity);

            Destroy(gameObject);
        }
    }
}
