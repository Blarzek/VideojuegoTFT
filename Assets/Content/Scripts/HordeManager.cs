using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HordeManager : MonoBehaviour {

    public static HordeManager sharedInstance; // Instancia compartida que se puede acceder desde cualquier script

    private string path;
    private List<RoundConfig> roundsConfig = new List<RoundConfig>();

    private int currentRound = -1;
    public int enemiesLeft = 0;
    private float roundTime = 60;
    private int finalScore = 0;
    private bool gameOver = false;

    public Character player;
    public GameObject[] spawns;
    public GameObject[] enemiesPrefabs;
    public GameObject gameOverUI;
    public Text roundScoreValue;
    public Text round;
    public GameObject timeOver;

    private void Awake() {
        sharedInstance = this; // Instancia compartida que se puede acceder desde cualquier script
    }

    // Use this for initialization
    void Start () {
        path = Application.dataPath + "/StreamingAssets/HordeConfig.json";
        LoadRoundsConfig();
        round.text = (currentRound + 1).ToString();
        InvokeRepeating("ManageHorde", 0f, 0.1f);
    }
	
	// Update is called once per frame
	void Update () {
        // Actualizar temporizador
        roundTime -= Time.deltaTime;
        if (roundTime <= 0) {
            roundTime = 0;
        }
        string minutes = Mathf.Floor(roundTime / 60).ToString("00");
        string seconds = Mathf.Floor(roundTime % 60).ToString("00");
        GameManager.sharedInstance.timerUI.text = minutes + ":" + seconds;
    }
    
    private void ManageHorde() {
        // Si no quedan enemigos, se pasa a la siguiente ronda
        if (enemiesLeft <= 0) {
            StartNextRound();
        }

        if (!gameOver) {
            // Si el personaje muere o el tiempo se acaba, se termina el modo horda
            if (roundTime <= 0) {
                timeOver.SetActive(true);
            }
            if (player.health <= 0 || roundTime <= 0) {
                Time.timeScale = 0.2f;
                Invoke("GameOver", 0.5f);
            }
        }
    }

    private void GameOver() {
        timeOver.SetActive(false);
        gameOver = true;
        CalculateScore();
        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }

    private void StartNextRound() {
        currentRound++;
        if (currentRound == roundsConfig.Count) {
            GameOver(); // Si se llega al final del JSON se acaba el modo horda
        } else {
            roundTime = roundsConfig[currentRound].RoundTime;
            round.text = (currentRound + 1).ToString();

            // Se generan los enemigos
            enemiesPrefabs[0].GetComponent<Character>().transformable = false;
            enemiesPrefabs[0].GetComponent<Rigidbody2D>().gravityScale = 0;

            int spawnSelector;

            for (int i = 0; i < roundsConfig[currentRound].PunchEnemies; i++) {
                if (Random.Range(0f, 1f) < 0.5f) {
                    spawnSelector = 0;
                } else {
                    spawnSelector = 1;
                }
                // Se genera un enemigo a puños en una posición aleatoria
                Instantiate(enemiesPrefabs[0], spawns[spawnSelector].transform);
                enemiesLeft++;
            }

            for (int i = 0; i < roundsConfig[currentRound].GunEnemies; i++) {
                if (Random.Range(0f, 1f) < 0.5f) {
                    spawnSelector = 0;
                } else {
                    spawnSelector = 1;
                }
                // Se genera un enemigo con pistola en una posición aleatoria
                Instantiate(enemiesPrefabs[1], spawns[spawnSelector].transform);
                enemiesLeft++;
            }

            // Se cura un poco al jugador al alcanzar ciertas rondas
            if (roundsConfig[currentRound].Heal) {
                GameManager.sharedInstance.ChangeHealth(player.gameObject, 20);
            }
        }
    }

    private void CalculateScore() {
        // Se cambia la música
        AudioSource bgm = gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        bgm.clip = GameManager.sharedInstance.levelCompleteBGM;
        bgm.Play();

        // Se desactivan otros elementos de la interfaz
        GameManager.sharedInstance.healthBar1.transform.parent.gameObject.SetActive(false);
        GameManager.sharedInstance.powerBar1.transform.parent.gameObject.SetActive(false);
        GameManager.sharedInstance.healthBar2.transform.parent.gameObject.SetActive(false);
        GameManager.sharedInstance.timerUI.gameObject.SetActive(false);
        GameManager.sharedInstance.moneyUI.gameObject.SetActive(false);
        //GameManager.sharedInstance.scoreUI.gameObject.SetActive(false);
        GameManager.sharedInstance.weaponUI.transform.parent.gameObject.SetActive(false);

        // Se muestra pantalla de puntuación
        GameManager.sharedInstance.paused = true;
        Time.timeScale = 0;

        // Se cargan valores de puntuación
        GameManager.sharedInstance.healthValue.text = player.health.ToString();
        GameManager.sharedInstance.powerValue.text = player.power.ToString();
        GameManager.sharedInstance.moneyValue.text = GameManager.sharedInstance.playerMoney.ToString();
        GameManager.sharedInstance.timeValue.text = Mathf.Floor(roundTime).ToString();
        roundScoreValue.text = round.text;

        // Se calcula la puntuación total
        int s1 = (int)player.health * 5;
        int s2 = (int)player.power * 5;
        int s3 = GameManager.sharedInstance.playerMoney * 20;
        finalScore = s1 + s2 + s3 + GameManager.sharedInstance.playerScore;
        GameManager.sharedInstance.SetFinalScore(finalScore);
        GameManager.sharedInstance.totalScoreValue.text = finalScore.ToString();
    }

    // Cargar parámetros de configuración de las rondas
    private void LoadRoundsConfig() {
        var jsonParsed = JSON.Parse(File.ReadAllText(path));
        roundsConfig = new List<RoundConfig>();

        for (int i = 0; i < jsonParsed["rounds"].Count; i++) {
            roundsConfig.Add(new RoundConfig(jsonParsed["rounds"][i]["time"], 
                                            jsonParsed["rounds"][i]["punchEnemy"], 
                                            jsonParsed["rounds"][i]["gunEnemy"], 
                                            jsonParsed["rounds"][i]["heal"]));
        }
    }
}
