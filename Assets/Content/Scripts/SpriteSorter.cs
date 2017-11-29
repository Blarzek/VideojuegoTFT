using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSorter : MonoBehaviour {

    public int layer;

	// Use this for initialization
	void Start () {
        if (GameManager.sharedInstance.actualMode == GameManager.GameMode.StoryMode || GameManager.sharedInstance.actualMode == GameManager.GameMode.HordeMode) {
            InvokeRepeating("SortSprite", 0, 0.5f);
        }
    }
	
	// Update is called once per frame
	void Update () {

    }

    private void SortSprite() {
        // Dibujar al personaje en la profundidad que corresponde
        GetComponent<SpriteRenderer>().sortingOrder = (Mathf.RoundToInt(transform.position.y * 100f) * -1) + layer;
    }
}
