using UnityEngine;
using System.Collections;

public class DeathPlaneTrigger : MonoBehaviour {

    public GameObject bluePlayer;
    public GameObject redPlayer;
    public GameObject ball;

    public Transform blueSpawnPoint;
    public Transform redSpawnPoint;
    public Transform ballSpawnPoint;

    public string bluePlayerTag = "";
    public string redPlayerTag = "";
    public string ballTag = "";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(bluePlayerTag))
        {
            resetBluePlayer(other.gameObject);
        }
        else if (other.CompareTag(redPlayerTag))
        {
            resetRedPlayer(other.gameObject);
        }
        else if (other.CompareTag(ballTag))
        {
            resetBall(other.gameObject);
        }
    }

    private void resetBluePlayer(GameObject bluePlayerObject)
    {
        float blueEnergy = bluePlayerObject.GetComponent<PlayerMovement>().Energy;
        Destroy(bluePlayerObject);

        if (bluePlayer != null && blueSpawnPoint != null)
        {
            GameObject _blue = Instantiate(bluePlayer, blueSpawnPoint.position, Quaternion.AngleAxis(180.0f, new Vector3(0.0f, 1.0f, 0.0f))) as GameObject;
            _blue.name = "Blue_Player";
            _blue.GetComponent<PlayerMovement>().Energy = blueEnergy;
        }
        else
        {
            Debug.LogError("Blue player instantiation error.");
        }
    }

    private void resetRedPlayer(GameObject redPlayerObject)
    {
        float redEnergy = redPlayerObject.GetComponent<PlayerMovement>().Energy;
        Destroy(redPlayerObject);

        if (redPlayer != null && redSpawnPoint != null)
        {
            GameObject _red = Instantiate(redPlayer, redSpawnPoint.position, Quaternion.AngleAxis(180.0f, new Vector3(0.0f, 1.0f, 0.0f))) as GameObject;
            _red.name = "Red_Player";
            _red.GetComponent<PlayerMovement>().Energy = redEnergy;
        }
        else
        {
            Debug.LogError("Red player instantiation error.");
        }
    }

    private void resetBall(GameObject ballObject)
    {
        Destroy(ballObject);

        if (ball != null && ballSpawnPoint != null)
        {
            GameObject _ball = Instantiate(ball, ballSpawnPoint.position, Quaternion.identity) as GameObject;
            _ball.name = "Ball";
        }
        else
        {
            Debug.LogError("Ball instantiation error.");
        }
    }
}
