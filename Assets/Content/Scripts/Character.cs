using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public float health = 100f;
    public float power = 0f;
    public float damage = 5f;
    public float damageMultiplier = 1;
    public float speed = 1.2f; // Velocidad a la que se mueve
    private float jumpForce = 180f; // Fuerza con la que salta
    public bool grounded;
    public bool canPunch;
    public bool canTransform;
    public bool transformed;
    public bool transformable = true;
    public bool blocking;
    public int givenScore = 100;

    public float horizontalThrowingStrength = 2f;
    public float verticalThrowingStrength = 2f;
    private Vector3 grenadeSpawn;

    private AudioSource audioSourcePlayer;
    public AudioSource audioSourceShield;
    public AudioClip punch1Sound;
    public AudioClip punch2Sound;
    public AudioClip jumpSound;
    public AudioClip transformSound;
    public AudioClip detransformSound;
    public Animator animatorPlayer;
    public Animator animatorShield;
    public BoxCollider2D punchHitbox;
    private Rigidbody2D rigidBody;

    void Start() {
        grounded = true;
        canPunch = true;
        canTransform = false;
        transformed = false;
        blocking = false;
        audioSourcePlayer = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody2D>();

        if (GameManager.sharedInstance) {
            // Los nuevos enemigos generados activan su área de detección si el modo desarrolador está activado
            if (tag != "Player1") {
                if (GameManager.sharedInstance.devModeEnabled) {
                    transform.Find("DetectionArea").gameObject.SetActive(true);
                }
            }
        }
    }

    void Update() {
        if (transformed) {
            // La barra de poder disminuye gradualmente al estar transformado
            power -= Time.deltaTime * 10;
            if (power <= 0) {
                // Al llegar a 0 se destransforma
                Detransform();
            }
            GameManager.sharedInstance.UpdateBars();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.VersusMode) {
            if (collision.gameObject.tag == "Ground") {
                grounded = true;
                animatorPlayer.SetBool("jumping", false);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.VersusMode) {
            ContactPoint2D[] collisions = collision.contacts;
            if (collisions == null) {
                if (collision.gameObject.tag == "Ground" && collisions.GetLength(1) == 0) {
                    grounded = false;
                    animatorPlayer.SetBool("jumping", true);
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (gameObject.tag == "Player2" && collision.gameObject.tag == "Player1") {
            // Detener el cuerpo para que los personajes no se puedan empujar tanto
            if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            } else if (GameManager.sharedInstance.actualMode == GameManager.GameMode.VersusMode) {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0f, GetComponent<Rigidbody2D>().velocity.y);
            } 
        }
    }

    public void MoveLeft() {
        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
            transform.localScale = new Vector2(-1, 1);
        }

        if (!blocking) {
            rigidBody.velocity = new Vector2(-speed, rigidBody.velocity.y);
        }
    }

    public void MoveRight() {
        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
            transform.localScale = new Vector2(1, 1);
        }

        if (!blocking) {
            rigidBody.velocity = new Vector2(speed, rigidBody.velocity.y);
        }
    }

    public void Jump() {
        if (grounded && !blocking) {
            rigidBody.AddForce(new Vector2(rigidBody.velocity.x, jumpForce));
            audioSourcePlayer.PlayOneShot(jumpSound); // Reproducir sonido
            grounded = false;
            animatorPlayer.SetBool("jumping", true);
        }
    }

    public void MoveUp() {
        if (grounded && !blocking) {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, speed * 0.5f);
        }
    }

    public void MoveDown() {
        if (grounded && !blocking) {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, -speed * 0.5f);
        }
    }

    public void LeftPunch() {
        if (grounded && canPunch && !blocking) {
            canPunch = false;
            NoMove();
            if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
                NoMoveVertical();
            }
            grounded = false;
            animatorPlayer.SetTrigger("punchL"); // Activa trigger del animator
            Invoke("PerformLeftPunch", 0.2f);
            Invoke("DisablePunchHitbox", 0.35f);
            Invoke("EnableCanPunch", 0.5f);
        }
    }

    public void RightPunch() {
        if (grounded && canPunch && !blocking) {
            canPunch = false;
            NoMove();
            if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
                NoMoveVertical();
            }
            grounded = false;
            animatorPlayer.SetTrigger("punchR"); // Activa trigger del animator
            Invoke("PerformRightPunch", 0.4f);
            Invoke("DisablePunchHitbox", 0.55f);
            Invoke("EnableCanPunch", 0.6f);
        }
    }

    private void DisablePunchHitbox() {
        punchHitbox.enabled = false;
    }

    private void EnableCanPunch() {
        canPunch = true;
        grounded = true;
    }

    private void PerformRightPunch() {
        // Función para aplicarle retraso
        damage = 5f * damageMultiplier;
        punchHitbox.enabled = true;
        audioSourcePlayer.PlayOneShot(punch1Sound, 0.35f); // Reproducir sonido
    }

    private void PerformLeftPunch() {
        // Función para aplicarle retraso
        damage = 3f * damageMultiplier;
        punchHitbox.enabled = true;
        audioSourcePlayer.PlayOneShot(punch2Sound, 0.35f); // Reproducir sonido
    }

    // Lanzar granada
    public void ThrowGrenade() {
        if (grounded && canPunch && !blocking) {
            canPunch = false;
            NoMove();
            if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
                NoMoveVertical();
            }
            grounded = false;
            animatorPlayer.SetTrigger("throwGrenade"); // Activa trigger del animator
            Invoke("SpawnGrenade", 0.5f);
            Invoke("EnableCanPunch", 0.6f);
        }
    }

    private void SpawnGrenade() {
        grenadeSpawn = new Vector3(transform.position.x + 0.2f, transform.position.y + 0.3f, transform.position.z);
        GameManager.sharedInstance.ChangeWeapon(GameManager.Weapon.Punch);
        GameObject grenade = Instantiate(GameManager.sharedInstance.grenadePrefab, grenadeSpawn, transform.rotation);
        grenade.transform.parent = null;
        grenade.GetComponent<Rigidbody2D>().AddForce(new Vector2(horizontalThrowingStrength * gameObject.transform.localScale.x, verticalThrowingStrength), ForceMode2D.Impulse);
        grenade.GetComponent<Rigidbody2D>().angularVelocity = -700;
    }

    public void Block() {
        // Función para bloquear daño
        if (!blocking) {
            audioSourceShield.Play(); // Reproducir sonido
            NoMove();
            if (GameManager.sharedInstance.actualMode != GameManager.GameMode.VersusMode) {
                NoMoveVertical();
            }
            // Animación aparece escudo
            animatorShield.SetBool("blocking", true);
            blocking = true;
        }
    }

    public void StopBlocking() {
        // Función para bloquear daño
        blocking = false;
        audioSourceShield.Stop();
        // Animación desaparece escudo
        animatorShield.SetBool("blocking", false);
    }

    public void Transform() {
        if (transformable && !transformed && !blocking) {
            GameManager.sharedInstance.paused = true;
            transformed = true;
            audioSourcePlayer.PlayOneShot(transformSound);
            GameManager.sharedInstance.StopTime();
            canPunch = false;
            animatorPlayer.SetTrigger("transformMonster"); // Activa trigger del animator
            animatorPlayer.SetBool("transformed", true);
            damageMultiplier = 2;
            speed = 1.6f;
            Invoke("EnableCanPunch", 1.5f);
            GameManager.sharedInstance.Invoke("ResumeTime", 1.0f);
            GameManager.sharedInstance.paused = false;
        }
    }

    private void Detransform() {
        GameManager.sharedInstance.paused = true;
        audioSourcePlayer.PlayOneShot(detransformSound);
        GameManager.sharedInstance.StopTime();
        animatorPlayer.SetTrigger("transformHuman"); // Activa trigger del animator
        animatorPlayer.SetBool("transformed", false);
        power = 0;
        damageMultiplier = 1;
        speed = 1.2f;
        transformed = false;
        GameManager.sharedInstance.Invoke("ResumeTime", 0.4f);
        GameManager.sharedInstance.paused = false;
    }

    public void NoMove() {
        if (grounded) {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }
    }

    public void NoMoveVertical() {
        if (grounded) {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        }
    }

    public void NoMoveFor(float time) {
        Invoke("FinishWait", time);
        NoMove();
        NoMoveVertical();
        grounded = false;
    }

    private void FinishWait() {
        grounded = true;
    }
}