using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScoresManager : MonoBehaviour {

    private List<SingleScore> topScores = new List<SingleScore>();
    public GameObject list; // Lista de puntuaciones
    public GameObject singleScorePrefab; // Fila de la lista de puntuaciones
    private string filePath = "";
    private string json = "";

    // Use this for initialization
    void Start() {
        filePath = Path.Combine(Application.streamingAssetsPath, "TopScores.json");
        StartCoroutine(ReadFileContent());
    }

    // Update is called once per frame
    void Update() {

    }

    // Obtiene el contenido del archivo json
    IEnumerator ReadFileContent() {
        if (filePath.Contains("://")) {
            WWW www = new WWW(filePath);
            yield return www;
            json = www.text;
        } else {
            json = File.ReadAllText(filePath);
        }
    }

    public void LoadTopScores() {
        // Limpiamos la lista
        topScores = new List<SingleScore>();
        foreach (Transform child in list.transform) {
            Destroy(child.gameObject);
        }

        // Tratar información recibida
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        var scoreParsed = JSON.Parse(json);

        List<SingleScore> disordered = new List<SingleScore>();

        for (int i = 0; i < scoreParsed["scores"].Count; i++) {
            disordered.Add(new SingleScore(0, scoreParsed["scores"][i]["name"], scoreParsed["scores"][i]["score"]));
        }

        int topScore = 0;
        int indexTopScore = 0;
        int actualRank = 1;

        while (actualRank <= 10) {
            // Buscamos la puntuación más elevada
            for (int i = 0; i < disordered.Count; i++) {
                if (topScore <= disordered[i].score) {
                    topScore = disordered[i].score;
                    indexTopScore = i;
                }
            }

            // La añadimos a las mejores puntuaciones
            SingleScore score = new SingleScore(actualRank, disordered[indexTopScore].GetName(), disordered[indexTopScore].GetScore());
            topScores.Add(score);
            actualRank += 1;
            topScore = 0;
            disordered.RemoveAt(indexTopScore);
        }

        foreach (SingleScore score in topScores) {
            // Añadimos filas de puntuación a la lista
            GameObject scoreObject = Instantiate(singleScorePrefab, list.transform);
            scoreObject.GetComponent<SingleScore>().rank = score.GetRank();
            scoreObject.GetComponent<SingleScore>().playerName = score.GetName();
            scoreObject.GetComponent<SingleScore>().score = score.GetScore();
            scoreObject.GetComponent<SingleScore>().UpdateText();
        }
    }

    // Escribe en el archivo json
    IEnumerator WriteFileContent(string json) {
        // On development
        yield return new WaitForEndOfFrame();
        //WWWForm form = new WWWForm();
        //form.AddBinaryData("file", json.bytes, "TopScores.json", "text/plain");
        //WWW www = new WWW(filePath, form);
    }

    public void SaveScore() {
        var scoreParsed = JSON.Parse(json);
        int score = GameManager.sharedInstance.GetFinalScore();
        GameObject inputField = GameObject.Find("NameInputField");
        // Asignamos valores al nuevo record
        string name = inputField.GetComponent<InputField>().text;
        string i = scoreParsed["scores"].Count.ToString();
        scoreParsed["scores"][i]["name"] = name;
        scoreParsed["scores"][i]["score"].AsInt = score;
        // Se escribe el fichero
        File.WriteAllText(filePath, scoreParsed.ToString());
        // Ocultamos el botón
        inputField.GetComponent<InputField>().interactable = false;
        GameObject.Find("ScoreSaved").transform.Find("Text").gameObject.SetActive(true);
        GameObject.Find("SubmitScoreButton").SetActive(false);
    }
}
