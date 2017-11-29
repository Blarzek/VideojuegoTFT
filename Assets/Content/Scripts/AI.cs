﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

    public Character playerTarget;
    private Character bot;
    private bool touchingPlayer = false;
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

            if (Random.Range(0f, 100f) < 70f) { // Posibilidad de acción
                if (!touchingPlayer && Vector3.Distance(transform.position, playerTarget.transform.position) > 0.3f) {

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
            if (distance < 0.6f) {
                if (Random.Range(0f, 100f) < 10f) { // Posibilidad de acción
                    // Si el jugador está saltando o golpeando
                    if (!playerTarget.grounded || !playerTarget.canPunch) {

                        performingAction = true;
                        randomTime = Random.Range(0.5f, 2f);
                        Invoke("FinishAction", randomTime);

                        while (time < randomTime) {
                            time += Time.deltaTime;
                            // El contrincante se aleja del jugador
                            if (GameManager.sharedInstance.inverse) {
                                bot.MoveLeft();
                            } else {
                                bot.MoveRight();
                            }
                        }
                        return;
                    }
                }
            }

            if (GameManager.sharedInstance.actualMode == GameManager.GameMode.VersusMode) {
                if (Random.Range(0f, 100f) < 0.5f) { // Posibilidad de acción

                    performingAction = true;
                    Invoke("FinishAction", 0.5f);

                    // El contrincante salta
                    bot.Jump();
                    return;
                }
            }

            if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
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

            if (Random.Range(0f, 100f) < 2f) { // Posibilidad de acción

                performingAction = true;
                Invoke("FinishAction", 1f);

                // El contrincante se transforma al tener el máximo de poder
                if (bot.power == 100) {
                    bot.power--;
                    bot.Transform();
                }

            }

            if (GameManager.sharedInstance.actualMode == GameManager.GameMode.VersusMode) {
                if (bot.health < 60) {
                    // Búsqueda de comida
                    if (Random.Range(0f, 100f) < 50f) { // Posibilidad de acción
                        performingAction = true;
                        if (GameObject.Find("FoodSpawn") != null) {
                            GameObject food = GameObject.Find("FoodSpawn").GetComponent<ObjectSpawn>().GetItem();

                            // El contrincante se acerca hacia la comida
                            while (time < 2) {
                                time += Time.deltaTime;
                                if (food) {
                                    // Dependiendo de la posición en la que esté
                                    if (transform.position.x > food.transform.position.x) {
                                        bot.MoveLeft();
                                    } else {
                                        bot.MoveRight();
                                    }
                                }
                            }
                            Invoke("FinishAction", 2f);
                            return;
                        } else {
                            print("AI: No food spawn found.");
                            FinishAction();
                        }
                    }
                }
            }

            touchingPlayer = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        // La IA solo funciona cuando el juego no está pausado
        if (Random.Range(0f, 100f) < 3) { // Posibilidad de acción
            // Si está cerca del jugador le golpea
            if (collision.gameObject.tag == "HitReach") {

                touchingPlayer = true;
                bot.NoMove();

                if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
                    // Se gira hacia el jugador
                    if (bot.transform.position.x < collision.gameObject.transform.position.x) {
                        bot.transform.localScale = new Vector3(1, 1, 1);
                    } else {
                        bot.transform.localScale = new Vector3(-1, 1, 1);
                    }

                    bot.NoMoveVertical();
                }

                if (Random.Range(0f, 100f) < 50) {
                    bot.RightPunch();
                } else {
                    bot.LeftPunch();
                }
            }
        }

        if (!GameManager.sharedInstance.paused && !performingAction) {
            // El contrincante bloquea los ataques
            if (Random.Range(0f, 100f) < 0.3f) {
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
