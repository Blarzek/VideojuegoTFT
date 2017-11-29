using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour {

    public Collider2D hit;
    private GameObject playerTargetObject;
    private Character character;
    private Character characterTarget;

    // Use this for initialization
    void Start() {
        character = this.transform.parent.gameObject.GetComponent<Character>();
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Player2" || collider.tag == "Player1" || collider.tag == "Boss") {
            playerTargetObject = collider.gameObject;
            characterTarget = collider.GetComponent<Character>();

            // Se calcula el daño
            float damage = -character.damage * character.damageMultiplier;

            // Si el personaje está bloqueando se reduce el daño
            if (characterTarget.blocking) {
                damage = damage * 0.2f;
            }

            // El personaje no gana poder si está transformado
            if (!character.transformed) {
                GameManager.sharedInstance.ChangePower(character.gameObject, 10);
            }

            // El jugador dañado obtiene un poco de poder
            GameManager.sharedInstance.ChangePower(characterTarget.gameObject, 2);

            // Cancelar el ataque del personaje dañado
            characterTarget.CancelInvoke("PerformRightPunch");
            characterTarget.CancelInvoke("PerformLeftPunch");
            GameManager.sharedInstance.ChangeHealth(playerTargetObject, damage);

        }
    }
}
