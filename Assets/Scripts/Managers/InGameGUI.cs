using UnityEngine;
using System.Collections;

public class InGameGUI : MonoBehaviour {
    public enum WindowID
    {
        ScoreBoard
    }

    public float windowHeigth = 30.0f;
    public float windowWidth = 70.0f;

    public float topMargin = 10.0f;

    public float labelXMargin = 1.0f;
    public float labelYMargin = 1.0f;

    public float barDisplacement = 5.0f;
    public float secondNumberDisplacement = 10.0f;
    
    public GameManager gameManager;

    private WindowID _windowId = WindowID.ScoreBoard;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUI.Window((int)_windowId,
            new Rect((Screen.width - windowWidth) / 2, topMargin, windowWidth, windowHeigth),
            scoreWindow, "Score");
    }

    private void scoreWindow(int id)
    {
        GUI.contentColor = Color.blue;
        GUI.Label(new Rect(labelXMargin, labelYMargin, windowWidth - 2 * labelXMargin, windowHeigth - 2 * labelYMargin),
            gameManager.BlueScore.ToString());
       
        GUI.contentColor = Color.red;
        GUI.Label(new Rect(labelXMargin + secondNumberDisplacement, labelYMargin, windowWidth - 2 * labelXMargin,
            windowHeigth - 2 * labelYMargin),
            gameManager.RedScore.ToString());

        GUI.contentColor = Color.grey;
        GUI.Label(new Rect(labelXMargin + barDisplacement, labelYMargin, windowWidth - 2 * labelXMargin, windowHeigth - 2 * labelYMargin),
            " | ");
    }
}
