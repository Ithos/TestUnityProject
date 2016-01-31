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

    public float maxX = 100.0f;
    public float maxY = 100.0f;
    public float maxZ = 500.0f;

    private GameObject _blue;
    private GameObject _red;
    private GameObject _ball;

    private int _blueScore = 0;
    private int _redScore = 0;

    public int BlueScore
    {
        get { return _blueScore; }
    }

    public int RedScore
    {
        get { return _redScore; }
    }


	// Use this for initialization
	void Start () {

        generatePlayers(0.0f, 0.0f);
        generateBall(neutralBallSpawnPoint);
        
        
	}
	
	// Update is called once per frame
	void Update () {
       checkBallsInGame();
	}

    private void resetGameAfterGoal( bool blue)
    {
        float blueEnergy = _blue.GetComponent<PlayerMovement>().Energy;
        float redEnergy = _red.GetComponent<AIEnemyMovement>().Energy;
        Destroy(_blue);
        Destroy(_red);
        Destroy(_ball);

        generatePlayers(blueEnergy, redEnergy);
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

    private void generatePlayers(float blueEnergy, float redEnergy)
    {
        if (bluePlayer != null && blueSpawnPoint != null)
        {
            _blue = Instantiate(bluePlayer, blueSpawnPoint.position, Quaternion.AngleAxis(180.0f, new Vector3(0.0f, 1.0f, 0.0f))) as GameObject;
            _blue.name = "Blue_Player";
            _blue.GetComponent<PlayerMovement>().Energy = blueEnergy;
        }
        else
        {
            Debug.LogError("Blue player instantiation error.");
        }

        if (redPlayer != null && redSpawnPoint != null)
        {
            _red = Instantiate(redPlayer, redSpawnPoint.position, Quaternion.identity) as GameObject;
            _red.name = "Red_Player";
            AIEnemyMovement ai = _red.GetComponent<AIEnemyMovement>();
            if (ai != null)
            {
                ai.Energy = redEnergy;
            }
            else
            {
                _red.GetComponent<PlayerMovement>().Energy = redEnergy;
            }
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

    private void checkBallsInGame()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag(_ball.tag);
        if (array.Length > 2)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                Destroy(array[i]);
            }

            generateBall(neutralBallSpawnPoint);
        }
    }

    private void checkMaxCoord()
    {
        Transform trans = _ball.transform;

        if (Mathf.Abs(trans.position.x) > maxX || Mathf.Abs(trans.position.y) > maxY || Mathf.Abs(trans.position.z) > maxZ)
        {
            Destroy(_ball);
            generateBall(neutralBallSpawnPoint);
        }
    }
}
