using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour {

    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player1" || collision.gameObject.tag == "Player2" || collision.gameObject.tag == "Boss") {
            // Restarle vida al personaje
            if (collision.gameObject.GetComponent<Character>().blocking) {
                GameManager.sharedInstance.ChangeHealth(collision.gameObject, -5);
            } else {
                GameManager.sharedInstance.ChangeHealth(collision.gameObject, -20);
            }
        }
    }

    private void DisableTrigger() {
        // Desactivar el trigger
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
