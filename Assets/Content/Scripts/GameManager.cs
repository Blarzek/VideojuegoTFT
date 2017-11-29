using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager sharedInstance; // Instancia compartida que se puede acceder desde cualquier script

    public GameObject player1;
    public GameObject player2;
    public GameObject healthBar1;
    public GameObject healthBar2;
    public GameObject damageBar1;
    public GameObject damageBar2;
    public GameObject powerBar1;
    public GameObject powerBar2;
    public GameObject youWin;
    public GameObject youLose;
    public GameObject player1Wins;
    public GameObject player2Wins;
    public GameObject menu;
    public GameObject levelCompleted;
    public GameObject resumeButton;
    public GameObject coinsPrefab;
    private GameObject killThis;
    private Animator player1Animator;
    private Animator player2Animator;
    public Animator flashAnimator;
    public AudioClip hurtSound;
    public AudioClip foodSound;
    public AudioClip koSound;
    private Character player1CharacterScript;
    private Character player2CharacterScript;
    public bool inverse; // Indica si los personajes están en su orientación inicial o no
    public bool paused; // Indica si la partida está pausada
    public int numberOfPlayers; // Número de jugadores humanos en la partida
    public int playerMoney;
    public Text moneyUI;
    public int playerScore;
    private int finalScore = 0;
    public Text scoreUI;
    public Image weaponUI;
    public Text timerUI;
    private float timer = 0f;
    public Sprite punchWeapon;
    public Sprite grenadeWeapon;
    public GameObject grenadePrefab;
    public enum GameMode { VersusMode, StoryMode, HordeMode };
    public GameMode actualMode;
    public int characterP1 = -1; // Índice del personaje escogido por el jugador 1
    public int characterP2 = -1; // Índice del personaje escogido por el jugador 2
    public enum Weapon { Punch, Grenade };
    public Weapon actualWeapon;
    public GameObject spawnP1;
    public GameObject spawnP2;
    public GameObject[] playerPrefabs;
    public Text healthValue; // Valor para la puntuación del nivel
    public Text powerValue; // Valor para la puntuación del nivel
    public Text moneyValue; // Valor para la puntuación del nivel
    public Text timeValue; // Valor para la puntuación del nivel
    public Text totalScoreValue; // Valor para la puntuación del nivel
    public GameObject devModeTag;
    public AudioClip levelCompleteBGM;
    public bool devModeEnabled = false;
    public GameObject missilePlatform;

    private void Awake() {
        sharedInstance = this; // Instancia compartida que se puede acceder desde cualquier script
        // Transformar de string a variables enumeradas desde el menu principal
        if (PlayerPrefs.GetString("GameMode") == "versusMode") {
            actualMode = GameMode.VersusMode;
        } else if (PlayerPrefs.GetString("GameMode") == "storyMode") {
            actualMode = GameMode.StoryMode;
        } else if (PlayerPrefs.GetString("GameMode") == "hordeMode") {
            actualMode = GameMode.HordeMode;
        }
    }

    private void Start() {
        numberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers");
        characterP1 = PlayerPrefs.GetInt("CharacterP1");
        characterP2 = PlayerPrefs.GetInt("CharacterP2");

        if (actualMode == GameMode.VersusMode) {
            // Se generan los jugadores
            player1 = Instantiate(playerPrefabs[characterP1], spawnP1.transform);
            player2 = Instantiate(playerPrefabs[characterP2], spawnP2.transform);
            player2.tag = "Player2";
            player1Animator = player1.GetComponent<Animator>();
            player2Animator = player2.GetComponent<Animator>();

            // Destruir AI del jugador 1
            Destroy(player1.GetComponent<AI>());

            if (numberOfPlayers == 2) {
                Destroy(player2.GetComponent<AI>());
                player2.GetComponent<PlayerControls>().player = 2;
            } else {
                Destroy(player2.GetComponent<PlayerControls>());
            }
            inverse = false;
            player1Animator.SetBool("inverse", false);
            player2Animator.SetBool("inverse", true);
        } else if (actualMode == GameMode.StoryMode) {
            playerMoney = 0;
            player1Animator = player1.GetComponent<Animator>();
            player2Animator = player2.GetComponent<Animator>();
        } else if (actualMode == GameMode.HordeMode) {
            // Se generan los jugadores
            player1 = Instantiate(playerPrefabs[characterP1], spawnP1.transform);
            player1Animator = player1.GetComponent<Animator>();

            // Se le pasa el jugador al Horde Manager
            HordeManager.sharedInstance.player = player1.GetComponent<Character>();

            // Destruir AI del jugador 1
            Destroy(player1.GetComponent<AI>());

            if (numberOfPlayers == 2) {
                // Se genera el jugador 2
                player2 = Instantiate(playerPrefabs[characterP2], spawnP2.transform);
                player2Animator = player2.GetComponent<Animator>();
                // Destruir AI del jugador 2
                Destroy(player2.GetComponent<AI>());
                player2.GetComponent<PlayerControls>().player = 2;
            }

            playerMoney = 0;
            player1Animator = player1.GetComponent<Animator>();
        }

        player1CharacterScript = player1.GetComponent<Character>();
        if (actualMode != GameMode.HordeMode) {
            player2CharacterScript = player2.GetComponent<Character>();
        }
        Time.timeScale = 1;
        paused = false;
    }

    void Update() {
        if (actualMode == GameMode.VersusMode) {
            // Comprobamos si un personaje salta por encima del otro
            // Los personajes se miran entre sí
            if (player1.GetComponent<Transform>().position.x > player2.GetComponent<Transform>().position.x) {
                player1.GetComponent<Transform>().localRotation = Quaternion.Euler(0, 180, 0);
                player2.GetComponent<Transform>().localRotation = Quaternion.Euler(0, 0, 0);
                player1Animator.SetBool("inverse", true);
                player2Animator.SetBool("inverse", false);
                inverse = true;
            } else {
                player1.GetComponent<Transform>().localRotation = Quaternion.Euler(0, 0, 0);
                player2.GetComponent<Transform>().localRotation = Quaternion.Euler(0, 180, 0);
                player1Animator.SetBool("inverse", false);
                player2Animator.SetBool("inverse", true);
                inverse = false;
            }
        } else if (actualMode == GameMode.StoryMode) {
            // Actualizar temporizador
            timer += Time.deltaTime;
            string minutes = Mathf.Floor(timer / 60).ToString("00");
            string seconds = Mathf.Floor(timer % 60).ToString("00");
            timerUI.text = minutes + ":" + seconds;
        }
    }

    private void FixedUpdate() {
        if (Input.GetKey(KeyCode.Escape)) {
            OpenMenu();
        }
    }

    public void UpdateBars() {
        healthBar1.GetComponent<RectTransform>().localScale = new Vector3(player1CharacterScript.health / 100f, 1f, 1f);
        powerBar1.GetComponent<RectTransform>().localScale = new Vector3(player1CharacterScript.power / 100f, 1f, 1f);
        if (actualMode != GameMode.HordeMode) {
            healthBar2.GetComponent<RectTransform>().localScale = new Vector3(player2CharacterScript.health / 100f, 1f, 1f);
            powerBar2.GetComponent<RectTransform>().localScale = new Vector3(player2CharacterScript.power / 100f, 1f, 1f);
        }
    }

    public void DamageBarDelay() {
        damageBar1.GetComponent<RectTransform>().localScale = new Vector3(player1CharacterScript.health / 100f, 1f, 1f);
        if (actualMode != GameMode.HordeMode) {
            damageBar2.GetComponent<RectTransform>().localScale = new Vector3(player2CharacterScript.health / 100f, 1f, 1f);
        }
    }

    public IEnumerator StopPlayerFor(float time) {
        player1CharacterScript.NoMove();
        player1CharacterScript.NoMoveVertical();
        player1CharacterScript.grounded = false;
        yield return new WaitForSeconds(time);
        player1CharacterScript.grounded = true;
    }

    public void ChangeMoney(int quantity) {
        playerMoney += quantity;
        moneyUI.text = "$" + playerMoney;
    }

    public void ChangeScore(int quantity) {
        playerScore += quantity;
        // Actualizar interfaz
        scoreUI.text = playerScore.ToString("000000");
    }

    // Función para cambiar el arma del personaje
    public void ChangeWeapon(Weapon weapon) {
        if (weapon == Weapon.Grenade) {
            weaponUI.sprite = grenadeWeapon;
        }
        if (weapon == Weapon.Punch) {
            weaponUI.sprite = punchWeapon;
        }
        actualWeapon = weapon;
    }

    public void ToggleDevMode() {
        if (!devModeEnabled) {
            // Activar etiqueta
            devModeTag.SetActive(true);

            // Mostrar posición donde caen los misiles
            Color red = new Color(255f, 0f, 0f);
            red.a = 0.5f;
            missilePlatform.GetComponent<SpriteRenderer>().color = red;

            // Mostrar rango de detección de enemigos
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player2");
            foreach (GameObject enemy in enemies) {
                enemy.transform.Find("DetectionArea").gameObject.SetActive(true);
            }

            // Mostrar posición de los proyectiles
            GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
            foreach (GameObject projectile in projectiles) {
                projectile.transform.Find("DevArea").gameObject.SetActive(true);
            }

            // Potenciar parámetros de jugador
            player1CharacterScript.power = 100f;
            player1CharacterScript.damage = 100;
            player1CharacterScript.damageMultiplier = 20;
            player1CharacterScript.speed = 3f;
            UpdateBars();
            ChangeMoney(10);
            player1CharacterScript.Block();

            devModeEnabled = true;
        } else {
            // Desactivar etiqueta
            devModeTag.SetActive(false);

            // Ocultar posición donde caen los misiles
            Color transparent = new Color(0f, 0f, 0f);
            transparent.a = 0f;
            missilePlatform.GetComponent<SpriteRenderer>().color = transparent;

            // Ocultar rango de detección de enemigos
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player2");
            foreach (GameObject enemy in enemies) {
                enemy.transform.Find("DetectionArea").gameObject.SetActive(false);
            }

            // Restablecer parámetros de jugador
            player1CharacterScript.damage = 5;
            player1CharacterScript.damageMultiplier = 2.6f;
            player1CharacterScript.speed = 1.2f;

            devModeEnabled = false;
        }
    }

    // Derrotar al jefe del nivel
    public void LevelCompleted(GameObject target) {
        // Se cambia la música
        AudioSource bgm = gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        bgm.clip = levelCompleteBGM;
        bgm.Play();
        // Se destruye el cañón
        GameObject cannon = target.transform.GetChild(0).gameObject;
        cannon.GetComponent<Animator>().SetTrigger("cannonDestroyed");
        GameObject explosion = Instantiate(target.GetComponent<Boss>().explosionPrefab);
        explosion.transform.position = cannon.transform.position;
        // Se desactivan otros elementos de la interfaz
        healthBar1.transform.parent.gameObject.SetActive(false);
        powerBar1.transform.parent.gameObject.SetActive(false);
        healthBar2.transform.parent.gameObject.SetActive(false);
        timerUI.gameObject.SetActive(false);
        moneyUI.gameObject.SetActive(false);
        weaponUI.transform.parent.gameObject.SetActive(false);
        // Se muestra pantalla de puntuación
        paused = true;
        levelCompleted.SetActive(true);
        Time.timeScale = 0;
        // Se cargan valores de puntuación
        healthValue.text = player1CharacterScript.health.ToString();
        powerValue.text = player1CharacterScript.power.ToString();
        moneyValue.text = playerMoney.ToString();
        timeValue.text = Mathf.Floor(timer).ToString();
        // Se calcula la puntuación total
        int s1 = (int)player1CharacterScript.health * 5;
        int s2 = (int)player1CharacterScript.power * 5;
        int s3 = playerMoney * 20;
        finalScore = s1 + s2 + s3 + playerScore;
        totalScoreValue.text = finalScore.ToString();
    }

    public int GetFinalScore() {
        return finalScore;
    }

    public void SetFinalScore(int value) {
        finalScore = value;
    }

    // Función que cambia la salud actual del personaje especificado por una cantidad dada
    public void ChangeHealth(GameObject target, float quantity) {
        Character character = target.GetComponent<Character>();
        if (character.health > 0) {

            // Si el Modo Dev está activado solo afectarán las curaciones al jugador
            // A los que no sean el jugador se les cambia la salud normalmente
            if (target.tag != "Player1" || devModeEnabled && quantity > 0 || !devModeEnabled) {
                character.health += quantity;
            }

            if (character.health > 100)
                character.health = 100;

            // Si la salud es negativa se reproduce animación de daño
            if (quantity < 0) {
                target.GetComponent<Animator>().SetTrigger("damaged");
                target.GetComponent<AudioSource>().PlayOneShot(hurtSound);
                // Solo si el Modo Dev está desactivado
                if (!devModeEnabled) {
                    // Si el personaje no está bloqueando, se queda aturdido
                    if (!character.blocking) {
                        character.NoMoveFor(0.5f);
                    }
                }
            } else if (quantity > 0) {
                target.GetComponent<Animator>().SetTrigger("healed");
                target.GetComponent<AudioSource>().PlayOneShot(foodSound);
            }

            if (character.health <= 0) { // El objetivo cae
                character.health = 0;
                target.GetComponent<Animator>().SetTrigger("ko");
                target.GetComponent<AudioSource>().PlayOneShot(koSound, 0.7f);

                // Si es el jefe su cañon explota
                if (target.tag == "Boss") {
                    LevelCompleted(target);
                }

                // Mostrar mensaje de derrota
                if (actualMode == GameMode.VersusMode) {
                    if (target.tag == "Player1" || target.tag == "Player2") {
                        LoserIs(target);
                    }
                } else if (actualMode == GameMode.StoryMode) {
                    if (target.tag == "Player1") {
                        LoserIs(target);
                    }
                }

                if (actualMode != GameMode.VersusMode) {
                    // La sombra desaparece
                    Destroy(target.transform.Find("Shadow").gameObject);
                }

                // Si el personaje tiene IA la destruye primero
                if (target.GetComponent<AI>()) {
                    Destroy(target.GetComponent<AI>());
                    Destroy(target.GetComponent<Rigidbody2D>());
                    Destroy(target.GetComponent<BoxCollider2D>());
                    Destroy(target.GetComponentInChildren<CapsuleCollider2D>());
                    Destroy(target.transform.Find("EnergyShield").gameObject);
                    character.NoMove();
                    if (actualMode == GameMode.StoryMode || actualMode == GameMode.HordeMode) {
                        character.NoMoveVertical();
                        ChangeScore(target.GetComponent<Character>().givenScore);
                    }
                }
                if (target.GetComponent<GunAI>()) {
                    Destroy(target.GetComponent<GunAI>());
                    Destroy(target.GetComponent<Rigidbody2D>());
                    Destroy(target.GetComponent<BoxCollider2D>());
                    Destroy(target.GetComponentInChildren<CapsuleCollider2D>());

                    // La pistola desaparece
                    Destroy(target.transform.Find("BulletSpawn").GetChild(0).gameObject);
                    // El escudo de energía desaparece
                    Destroy(target.transform.Find("EnergyShield").gameObject);
                    character.NoMove();
                    if (actualMode == GameMode.StoryMode || actualMode == GameMode.HordeMode) {
                        character.NoMoveVertical();
                        ChangeScore(target.GetComponent<Character>().givenScore);
                    }
                }

                if (actualMode == GameMode.StoryMode || actualMode == GameMode.HordeMode) {
                    if (target.tag != "Player1") {
                        // El enemigo cae y suelta dinero
                        if (target.tag == "Player2") {
                            GameObject coins = Instantiate(coinsPrefab);
                            coins.transform.position = new Vector3(character.transform.position.x, character.transform.position.y + 0.1f, character.transform.position.z);
                        }
                        if (actualMode == GameMode.HordeMode) {
                            HordeManager.sharedInstance.enemiesLeft--;
                        }

                        // Variable global para apuntar al personaje que debe desaparecer
                        killThis = target;
                        Invoke("DestroyDeathPlayer", 3);
                    }
                }
            }

            // Se actualizan las barras de salud y poder
            UpdateBars();
            Invoke("DamageBarDelay", 1f);
        }
    }

    // Función que cambia el poder actual del personaje especificado por una cantidad dada
    public void ChangePower(GameObject target, float quantity) {
        Character character = target.GetComponent<Character>();
        character.power += quantity;

        // Si el poder llega a 100, se mantiene ahí y le permite al personaje transformarse
        if (character.power >= 100) {
            character.power = 100;
            character.canTransform = true;
        }

        // Se actualizan las barras de salud y poder
        UpdateBars();
    }

    // Función que cambia el daño actual del personaje especificado por una cantidad dada
    public void ChangeDamage(GameObject target, float quantity) {
        Character character = target.GetComponent<Character>();
        character.damage = quantity;
    }

    void DestroyDeathPlayer() {
        Destroy(killThis);
    }

    public void OpenMenu() {
        paused = true;
        menu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame() {
        menu.SetActive(false);
        Time.timeScale = 1;
        paused = false;
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu() {
        SceneManager.LoadScene(0);
    }

    // Mensaje de resultado de la partida
    public void LoserIs(GameObject loser) {
        if (numberOfPlayers == 1) {
            Destroy(player2.GetComponent<AI>());
            if (loser == player1) {
                youLose.SetActive(true);
            } else {
                youWin.SetActive(true);
            }
        } else {
            if (loser == player1) {
                player2Wins.SetActive(true);
            } else {
                player1Wins.SetActive(true);
            }
        }

        // Detener el tiempo y mostrar el menu
        Time.timeScale = 0.2f;
        resumeButton.SetActive(false);
        Invoke("OpenMenu", 0.5f);
    }

    public void YouWin() {
        youWin.SetActive(true);
    }

    // Carga el nivel especificado
    public void LoadLevel(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }

    public void StopTime() {
        flashAnimator.SetTrigger("FlashIn");
        player1.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        player1CharacterScript.enabled = false;

        if (actualMode == GameMode.VersusMode) {
            player2.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            if (player2.GetComponent<AI>() != null) {
                player2.GetComponent<AI>().enabled = false;
            }
        }
    }

    public void ResumeTime() {
        player1.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        player1CharacterScript.enabled = true;

        if (actualMode == GameMode.VersusMode) {
            player2.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            if (player2.GetComponent<AI>() != null) {
                player2.GetComponent<AI>().enabled = true;
            }
        }
    }

}
