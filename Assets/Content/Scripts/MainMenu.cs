using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public GameObject canvasMainMenu;
    public GameObject canvasNumPlayer;
    public GameObject canvasPlayerSelect;
    public GameObject canvasDefaultControls;
    public GameObject canvasTopScores;
    public GameObject canvasNumPlayerHorde;
    public GameObject canvasPlayerSelectHorde;
    public Text characterSelectMessage;
    public Text characterSelectMessageHorde;
    private ScoresManager scoresManager;

    // Use this for initialization
    void Start() {
        ResetCharacter();
        scoresManager = GetComponent<ScoresManager>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void SelectNumberOfPlayers() {
        canvasMainMenu.SetActive(false);
        canvasPlayerSelect.SetActive(false);
        canvasNumPlayer.SetActive(true);
        ResetCharacter();
    }

    public void SelectNumberOfPlayersHorde() {
        canvasMainMenu.SetActive(false);
        canvasPlayerSelectHorde.SetActive(false);
        canvasNumPlayerHorde.SetActive(true);
        ResetCharacter();
    }

    public void StartGame(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadMainMenu() {
        canvasMainMenu.SetActive(true);
        canvasPlayerSelect.SetActive(false);
        canvasPlayerSelectHorde.SetActive(false);
        canvasNumPlayer.SetActive(false);
        canvasNumPlayerHorde.SetActive(false);
        canvasDefaultControls.SetActive(false);
        canvasTopScores.SetActive(false);
    }

    public void ShowControls() {
        canvasMainMenu.SetActive(false);
        canvasPlayerSelect.SetActive(false);
        canvasNumPlayer.SetActive(false);
        canvasDefaultControls.SetActive(true);
    }

    public void ShowTopScores() {
        canvasMainMenu.SetActive(false);
        canvasPlayerSelect.SetActive(false);
        canvasNumPlayer.SetActive(false);
        canvasDefaultControls.SetActive(false);
        // Cargar información de las puntuaciones
        scoresManager.LoadTopScores();
        canvasTopScores.SetActive(true);
    }

    public void QuitGame() {
        Application.Quit();
    }

    private void ResetCharacter() {
        PlayerPrefs.SetInt("CharacterP1", -1);
        PlayerPrefs.SetInt("CharacterP2", -1);
        characterSelectMessage.text = "Select Player 1";
        characterSelectMessageHorde.text = "Select Player 1";
    }

    public void SetCharacter(int index) {
        if (PlayerPrefs.GetInt("CharacterP1") == -1) {
            PlayerPrefs.SetInt("CharacterP1", index);
            characterSelectMessage.text = "Select Player 2";
        } else {
            if (PlayerPrefs.GetInt("CharacterP2") == -1) {
                PlayerPrefs.SetInt("CharacterP2", index);
                SetVersusMode();
            }
        }
    }

    public void SetCharacterHorde(int index) {
        if (PlayerPrefs.GetInt("NumberOfPlayers") == 1) {
            if (PlayerPrefs.GetInt("CharacterP1") == -1) {
                PlayerPrefs.SetInt("CharacterP1", index);
                SetHordeMode();
            }
        } else {
            if (PlayerPrefs.GetInt("CharacterP1") == -1) {
                PlayerPrefs.SetInt("CharacterP1", index);
                characterSelectMessageHorde.text = "Select Player 2";
            } else {
                if (PlayerPrefs.GetInt("CharacterP2") == -1) {
                    PlayerPrefs.SetInt("CharacterP2", index);
                    SetHordeMode();
                }
            }
        }
    }

    public void SetNumberOfPlayers(int number) {
        PlayerPrefs.SetInt("NumberOfPlayers", number);
        canvasNumPlayer.SetActive(false);
        canvasPlayerSelect.SetActive(true);
    }

    public void SetNumberOfPlayersHorde(int number) {
        PlayerPrefs.SetInt("NumberOfPlayers", number);
        canvasNumPlayerHorde.SetActive(false);
        canvasPlayerSelectHorde.SetActive(true);
    }

    public void SetVersusMode() {
        PlayerPrefs.SetString("GameMode", "versusMode");
        StartGame(2);
    }

    public void SetStoryMode() {
        PlayerPrefs.SetString("GameMode", "storyMode");
        PlayerPrefs.SetInt("NumberOfPlayers", 1);
        StartGame(1);
    }

    public void SetHordeMode() {
        PlayerPrefs.SetString("GameMode", "hordeMode");
        StartGame(3);
    }

}
