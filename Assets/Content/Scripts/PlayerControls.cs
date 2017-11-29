using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    private Character character;
    public int player;
    private float moveSpeed;
    private bool devKeyPressed = false;

    // Use this for initialization
    void Start() {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update() {

    }

    void FixedUpdate() {
        // Actualizar velocidad del animator
        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.VersusMode) {
            moveSpeed = GetComponent<Rigidbody2D>().velocity.x;
        } else if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {//(PlayerPrefs.GetString("GameMode") == "storyMode" || PlayerPrefs.GetString("GameMode") == "hordeMode") {
            moveSpeed = GetComponent<Rigidbody2D>().velocity.magnitude;
        }
        GetComponent<Animator>().SetFloat("speed", moveSpeed);

        // Mover a izquierda
        // Solo si no está golpeando
        if (Input.GetAxisRaw("Horizontal Player " + player) < 0 && character.canPunch && character.grounded) {
            character.MoveLeft();
        }

        // Mover a derecha
        // Solo si no está golpeando
        if (Input.GetAxisRaw("Horizontal Player " + player) > 0 && character.canPunch && character.grounded) {
            character.MoveRight();
        }

        // Quedarse quieto
        if (Input.GetAxisRaw("Horizontal Player " + player) == 0) {
            // Deja de acelerar si sueltas la tecla en el suelo
            character.NoMove();
        }

        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.VersusMode) {
            // Saltar
            if (Input.GetAxisRaw("Vertical Player " + player) > 0) {
                character.Jump();
            }
        } else if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
            // Subir
            if (Input.GetAxisRaw("Vertical Player " + player) > 0) {
                character.MoveUp();
            }

            // Bajar
            if (Input.GetAxisRaw("Vertical Player " + player) < 0) {
                character.MoveDown();
            }

            // Quedarse quieto verticalmente
            if (Input.GetAxisRaw("Vertical Player " + player) == 0) {
                // Deja de acelerar si sueltas la tecla en el suelo
                character.NoMoveVertical();
            }
        }

        if (GameManager.sharedInstance.actualWeapon == GameManager.Weapon.Punch) {
            // Puñetazo derecho
            if (Input.GetAxisRaw("Right Punch Player " + player) > 0) {
                character.RightPunch();
            }

            // Puñetazo izquierdo
            if (Input.GetAxisRaw("Left Punch Player " + player) > 0) {
                character.LeftPunch();
            }
        } else if (GameManager.sharedInstance.actualWeapon == GameManager.Weapon.Grenade) {
            // Lanzar granada
            if (Input.GetAxisRaw("Right Punch Player " + player) > 0) {
                character.ThrowGrenade();
            }

            if (Input.GetAxisRaw("Left Punch Player " + player) > 0) {
                character.ThrowGrenade();
            }
        }

        // Bloquear daño
        if (Input.GetAxisRaw("Block Player " + player) > 0) {
            character.Block();
        } else {
            character.StopBlocking();
        }

        // Transformación
        if (Input.GetAxisRaw("Transform Player " + player) > 0 && character.power == 100f) {
            character.Transform();
        }

        // Developer Mode
        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
            if (Input.GetAxisRaw("Dev Mode") > 0) {
                if (!devKeyPressed) {
                    devKeyPressed = true;
                    GameManager.sharedInstance.ToggleDevMode();
                }
            } else {
                devKeyPressed = false;
            }
        } else {
            if (Input.GetAxisRaw("Dev Mode") > 0) {
                print("Fatal Exception: Not in Story Mode or Horde Mode.");
            }
        }
    }
}
