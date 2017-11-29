using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawn : MonoBehaviour {

    public GameObject player;
    public GameObject prefab;
    private GameObject newFood;
    public float minTime;
    public float maxTime;
    private float timer;
    private Vector3 randomPosition;
    public float spawnRange = 4f;
    public bool singleSpawn = false; // Indica si se genera un sólo objeto cada vez, hasta que éste desaparece
    private GameObject item; // Objeto generado
    public GameObject platformPrefab;
    private GameObject platform;

    // Use this for initialization
    void Start() {
        // Calculamos el tiempo en el que se generará el primer objeto
        timer = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update() {
        if (timer > 0) {
            timer -= Time.deltaTime;
        } else {
            if (singleSpawn && item == null || !singleSpawn) {
                // Se obtiene una posición aleatoria en un rango alrededor del objeto que tenga el script
                randomPosition = new Vector3(Random.Range(player.transform.position.x - spawnRange, player.transform.position.x + spawnRange), transform.position.y, transform.position.z);
                item = Instantiate(prefab);
                item.transform.position = randomPosition;
                // Calculamos el tiempo en el que se generará el siguiente objeto
                timer = Random.Range(minTime, maxTime);

                if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
                    // El objeto caerá depositado en esta plataforma invisible
                    platform = Instantiate(platformPrefab);
                    platform.transform.position = new Vector3(item.transform.position.x, player.transform.position.y, player.transform.position.z);
                    // Se le asigna al misil su propia plataforma
                    if (item.GetComponent<Missile>()) {
                        item.GetComponent<Missile>().platform = platform;
                    } else if (item.GetComponent<Food>()) {
                        item.GetComponent<Food>().platform = platform;
                    }

                    // Se activa la zona donde caerá el misil si el modo Dev está activado
                    if (GameManager.sharedInstance.devModeEnabled) {
                        platform.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }
        }
    }

    public GameObject GetItem() {
        return item;
    }
}
