using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour {

    private Character player1;
    private Character bot;
    private Boss boss;
    private float moveSpeed;
    private float time = 0f;
    private float randomTime;
    private bool performingAction = false;
    private bool allowChangeAim = true;

    // Use this for initialization
    void Start() {
        player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Character>();
        bot = GetComponent<Character>();
        boss = GetComponent<Boss>();
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnEnable() {
        InvokeRepeating("PerformAI", 3f, 0.2f);
    }

    private void FinishAction() {
        performingAction = false;
        bot.NoMove();
        bot.NoMoveVertical();
        time = 0f;
    }

    private void PerformAI() {
        // La IA solo funciona cuando el juego no está pausado
        if (!GameManager.sharedInstance.paused && !performingAction) {

            if (Random.Range(0f, 100f) < 30f) { // Posibilidad de acción
                performingAction = true;
                randomTime = Random.Range(0.4f, 1f);
                Invoke("FinishAction", randomTime);

                // El contrincante se acerca verticalmente
                while (time < randomTime) {
                    time += Time.deltaTime;
                    // Dependiendo de la posición del jugador
                    if (transform.position.y > player1.transform.position.y) {
                        bot.MoveDown();
                    } else {
                        bot.MoveUp();
                    }
                }
                return;
            }

            // El jefe dispara con el cañón en medio
            if (boss.actualPosition == Boss.CannonPosition.Mid) {
                if (Random.Range(0f, 100f) < 60f) { // Posibilidad de acción
                    performingAction = true;
                    randomTime = Random.Range(1f, 2f);
                    Invoke("FinishAction", randomTime);

                    allowChangeAim = true;
                    boss.ShootMid();
                    return;
                }
            }

            // El jefe dispara con el cañón por abajo
            if (boss.actualPosition == Boss.CannonPosition.Down) {
                // Si se detecta al jugador delante
                if (player1.transform.position.y > transform.position.y - 0.25f && player1.transform.position.y < transform.position.y + 0.25f) {
                    if (Random.Range(0f, 100f) < 60f) { // Posibilidad de acción
                        performingAction = true;
                        randomTime = Random.Range(1f, 2f);
                        Invoke("FinishAction", randomTime);

                        allowChangeAim = true;
                        boss.ShootDown();
                        return;
                    }
                }
            }

            // El jefe dispara con el cañón por arriba
            if (boss.actualPosition == Boss.CannonPosition.Up) {
                if (Random.Range(0f, 100f) < 60f) { // Posibilidad de acción
                    performingAction = true;
                    randomTime = Random.Range(1f, 2f);
                    Invoke("FinishAction", randomTime);

                    allowChangeAim = true;
                    boss.ShootUp();
                    return;
                }
            }

            if (allowChangeAim) {
                allowChangeAim = false;
                if (boss.actualPosition == Boss.CannonPosition.Mid) {

                    // El jefe mueve el cañón hacia abajo
                    if (Random.Range(0f, 100f) < 30f) { // Posibilidad de acción
                        performingAction = true;
                        randomTime = Random.Range(0.4f, 1f);
                        Invoke("FinishAction", randomTime);

                        boss.MoveMidDown();
                        return;
                    }

                    // El jefe mueve el cañón hacia arriba
                    if (Random.Range(0f, 100f) < 30f) { // Posibilidad de acción
                        performingAction = true;
                        randomTime = Random.Range(0.4f, 1f);
                        Invoke("FinishAction", randomTime);

                        boss.MoveMidUp();
                        return;
                    }
                }

                if (boss.actualPosition == Boss.CannonPosition.Down) {

                    // El jefe mueve el cañón hacia el medio
                    if (Random.Range(0f, 100f) < 30f) { // Posibilidad de acción
                        performingAction = true;
                        randomTime = Random.Range(0.4f, 1f);
                        Invoke("FinishAction", randomTime);

                        boss.MoveDownMid();
                        return;
                    }

                    // El jefe mueve el cañón hacia arriba
                    if (Random.Range(0f, 100f) < 30f) { // Posibilidad de acción
                        performingAction = true;
                        randomTime = Random.Range(0.4f, 1f);
                        Invoke("FinishAction", randomTime);

                        boss.MoveDownUp();
                        return;
                    }
                }

                if (boss.actualPosition == Boss.CannonPosition.Up) {

                    // El jefe mueve el cañón hacia el medio
                    if (Random.Range(0f, 100f) < 30f) { // Posibilidad de acción
                        performingAction = true;
                        randomTime = Random.Range(0.4f, 1f);
                        Invoke("FinishAction", randomTime);

                        boss.MoveUpMid();
                        return;
                    }

                    // El jefe mueve el cañón hacia abajo
                    if (Random.Range(0f, 100f) < 30f) { // Posibilidad de acción
                        performingAction = true;
                        randomTime = Random.Range(0.4f, 1f);
                        Invoke("FinishAction", randomTime);

                        boss.MoveUpDown();
                        return;
                    }
                }
            }
        }
    }
}
