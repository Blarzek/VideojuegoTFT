using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    public AudioClip foodSound;
    public enum BoostType { Health, Power, Damage };
    public BoostType boostType;
    public float quantity = 20f;
    public float boostTime = 10f;
    private GameObject characterObject;
    private float previousDamage;
    public GameObject platform;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player1" || collision.gameObject.tag == "Player2") {
            characterObject = collision.gameObject;
            if (boostType == BoostType.Health) {
                GameManager.sharedInstance.ChangeHealth(characterObject, quantity);
            } else if (boostType == BoostType.Power) {
                GameManager.sharedInstance.ChangePower(characterObject, quantity);
            } else if (boostType == BoostType.Damage) {
                previousDamage = characterObject.GetComponent<Character>().damage;
                GameManager.sharedInstance.ChangeDamage(characterObject, previousDamage + quantity);
                Invoke("RestoreDamage", boostTime);
            }
            // Reproducir sonido
            characterObject.GetComponent<AudioSource>().PlayOneShot(foodSound);

            // Desaparece su propia plataforma
            Destroy(platform);

            // La comida desaparece al comerla
            Destroy(gameObject);
        }
    }

    private void RestoreDamage() {
        GameManager.sharedInstance.ChangeDamage(characterObject, previousDamage);
    }
}
