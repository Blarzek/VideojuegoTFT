using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerDestroyer : MonoBehaviour {

    public float duration; // Tiempo hasta que el objeto se destruya
    private float timer; // Temporizador

    // Use this for initialization
    void Start() {
        timer = duration;
    }

    // Update is called once per frame
    void Update() {
        // El objeto se destruye cuando el temporizador llega a cero
        if (timer > 0) {
            timer -= Time.deltaTime;
        } else {
            Destroy(gameObject);
        }
    }
}
