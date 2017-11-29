using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAI : MonoBehaviour {

    private Character playerTarget;
    private Character bot;
    public GunCharacter gunCharacter;
    private float time = 0f;
    private float moveSpeed;
    private bool playerDetected = false;
    private float detectionDistance = 2.5f;
    private bool performingAction = false;
    private float randomTime;

    // Use this for initialization
    void Start() {
        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
            if (GameManager.sharedInstance.numberOfPlayers == 2) {
                int random = Random.Range(0, 2);
                playerTarget = GameObject.FindGameObjectsWithTag("Player1")[random].GetComponent<Character>();
            } else {
                playerTarget = GameObject.FindGameObjectWithTag("Player1").GetComponent<Character>();
            }
        } else {
            playerTarget = GameObject.FindGameObjectWithTag("Player1").GetComponent<Character>();
        }

        bot = GetComponent<Character>();

        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.VersusMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
            // Ejecutar IA
            InvokeRepeating("PerformAI", 0.5f, 0.1f);
        }
    }

    // Update is called once per frame
    void Update() {
        // Actualizar velocidad del animator
        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.VersusMode) {
            moveSpeed = GetComponent<Rigidbody2D>().velocity.x;
            GetComponent<Animator>().SetFloat("speed", moveSpeed);
        } else if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode) {
            moveSpeed = GetComponent<Rigidbody2D>().velocity.magnitude;
            GetComponent<Animator>().SetFloat("speed", moveSpeed);

            if (!playerDetected) {
                // Calcular la distancia hasta el jugador
                if (Vector3.Distance(transform.position, playerTarget.transform.position) < detectionDistance) {
                    playerDetected = true;
                    // Ejecutar IA
                    InvokeRepeating("PerformAI", 0.5f, 0.1f);
                }
            }
        } else if (GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
            moveSpeed = GetComponent<Rigidbody2D>().velocity.magnitude;
            GetComponent<Animator>().SetFloat("speed", moveSpeed);
        }
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

            // Se gira hacia el jugador
            if (bot.transform.position.x < playerTarget.gameObject.transform.position.x) {
                bot.transform.localScale = new Vector3(1, 1, 1);
            } else {
                bot.transform.localScale = new Vector3(-1, 1, 1);
            }

            if (Random.Range(0f, 100f) < 70f) { // Posibilidad de acción
                float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.transform.position);
                if (distanceToPlayer > 2f) { // Distancia a la que dispara

                    performingAction = true;
                    randomTime = Random.Range(0.5f, 2f);
                    Invoke("FinishAction", randomTime);

                    while (time < randomTime) {
                        time += Time.deltaTime;
                        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.VersusMode) {
                            // El contrincante se acerca hacia el jugador horizontalmente
                            if (GameManager.sharedInstance.inverse) {
                                bot.MoveRight();
                            } else {
                                bot.MoveLeft();
                            }
                        } else if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
                            if (transform.position.x < playerTarget.transform.position.x) { // Está a la izquierda del jugador
                                bot.MoveRight();
                            } else {
                                bot.MoveLeft();
                            }
                        }
                    }
                }
                return;
            }

            if (Random.Range(0f, 100f) < 70f) { // Posibilidad de acción
                if (gunCharacter.canShoot) { // Personaje a tiro
                    // El contrincante dispara con la pistola
                    bot.NoMove();
                    gunCharacter.Shoot();
                    return;
                }
            }

            if (Random.Range(0f, 100f) < 1f) { // Posibilidad de acción
                performingAction = true;
                randomTime = Random.Range(0.6f, 0.8f);
                Invoke("FinishAction", randomTime);

                while (time < randomTime) {
                    time += Time.deltaTime;
                    if (GameManager.sharedInstance.actualMode == GameManager.GameMode.VersusMode) {
                        // El contrincante se aleja del jugador
                        if (GameManager.sharedInstance.inverse) {
                            bot.MoveLeft();
                        } else {
                            bot.MoveRight();
                        }
                    } else if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
                        if (transform.position.x < playerTarget.transform.position.x) {
                            bot.MoveLeft();
                        } else {
                            bot.MoveRight();
                        }
                    }
                }
                return;
            }

            // Comprueba si está a una distancia cercana al jugador para alejarse
            float distance = Vector3.Distance(playerTarget.transform.position, transform.position);
            if (distance < 1f) {
                if (Random.Range(0f, 100f) < 60f) { // Posibilidad de acción
                    performingAction = true;
                    randomTime = Random.Range(0.5f, 2f);
                    Invoke("FinishAction", randomTime);

                    while (time < randomTime) {
                        time += Time.deltaTime;
                        // El contrincante se aleja del jugador
                        if (transform.position.x < playerTarget.transform.position.x) {
                            bot.MoveLeft();
                        } else {
                            bot.MoveRight();
                        }
                    }
                    return;
                }
            }

            if (Random.Range(0f, 100f) < 60f) { // Posibilidad de acción

                performingAction = true;
                randomTime = Random.Range(0.3f, 0.4f);
                Invoke("FinishAction", randomTime);

                // El contrincante se acerca verticalmente
                while (time < randomTime) {
                    time += Time.deltaTime;
                    // Dependiendo de la posición del jugador
                    if (transform.position.y > playerTarget.transform.position.y) {
                        bot.MoveDown();
                    } else {
                        bot.MoveUp();
                    }
                }
                return;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!GameManager.sharedInstance.paused && !performingAction) {
            // El contrincante bloquea los ataques
            if (Random.Range(0f, 100f) < 0.2f) {
                // Si el jugador está cerca y está golpeando
                if (Vector3.Distance(transform.position, playerTarget.transform.position) < 1f && !playerTarget.canPunch) {
                    performingAction = true;
                    randomTime = Random.Range(0.4f, 0.8f);
                    Invoke("FinishAction", randomTime);
                    Invoke("BotStopBlocking", randomTime);

                    while (time < randomTime) {
                        time += Time.deltaTime;
                        bot.Block();
                    }
                    return;
                }
            }
        }
    }

    private void BotStopBlocking() {
        bot.StopBlocking();
    }

}
