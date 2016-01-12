using UnityEngine;
using System.Collections;

public class RedGoal : MonoBehaviour {

    public GameManager gameManager;
    public string ballCenterTag;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ballCenterTag))
        {
            gameManager.bluePlayerGoal();
        }
    }
}
