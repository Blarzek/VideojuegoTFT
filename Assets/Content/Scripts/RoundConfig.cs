using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoundConfig : MonoBehaviour {

    public int RoundTime { get; set; }
    public int PunchEnemies { get; set; }
    public int GunEnemies { get; set; }
    public bool Heal { get; set; }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public RoundConfig(int roundTime, int punchEnemies, int gunEnemies, bool heal) {
        RoundTime = roundTime;
        PunchEnemies = punchEnemies;
        GunEnemies = gunEnemies;
        Heal = heal;
    }
}
