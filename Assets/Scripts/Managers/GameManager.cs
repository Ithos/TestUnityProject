using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public GameObject bluePlayer;
    public GameObject redPlayer;
    public GameObject ball;

    public Transform blueSpawnPoint;
    public Transform redSpawnPoint;

    public Transform neutralBallSpawnPoint;
    public Transform blueBallSpawnPoint;
    public Transform redBallSpawnPoint;

    private GameObject _blue;
    private GameObject _red;
    private GameObject _ball;

    private int _blueScore = 0;
    private int _redScore = 0;


	// Use this for initialization
	void Start () {

        generatePlayers();
        generateBall(neutralBallSpawnPoint);
        
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void resetGameAfterGoal( bool blue)
    {
        Destroy(_blue);
        Destroy(_red);
        Destroy(_ball);

        generatePlayers();
        Transform tmpTransf = blue ? redBallSpawnPoint : blueBallSpawnPoint;
        generateBall(tmpTransf);
    }

    public void bluePlayerGoal()
    {
        ++_blueScore;
        resetGameAfterGoal( true );
    }

    public void redPlayerGoal()
    {
        ++_redScore;
        resetGameAfterGoal(false);
    }

    private void generatePlayers()
    {
        if (bluePlayer != null && blueSpawnPoint != null)
        {
            _blue = Instantiate(bluePlayer, blueSpawnPoint.position, Quaternion.AngleAxis(180.0f, new Vector3(0.0f, 1.0f, 0.0f))) as GameObject;
            _blue.name = "Blue_Player";
        }
        else
        {
            Debug.LogError("Blue player instantiation error.");
        }

        if (redPlayer != null && redSpawnPoint != null)
        {
            _red = Instantiate(redPlayer, redSpawnPoint.position, Quaternion.identity) as GameObject;
            _red.name = "Red_Player";
        }
        else
        {
            Debug.LogError("Red player instantiation error.");
        }

    }

    private void generateBall(Transform spawnPoint)
    {
        if (ball != null && neutralBallSpawnPoint != null)
        {
            _ball = Instantiate(ball, spawnPoint.position, Quaternion.identity) as GameObject;
            _ball.name = "Ball";
        }
        else
        {
            Debug.LogError("Ball instantiation error.");
        }
    }
}
