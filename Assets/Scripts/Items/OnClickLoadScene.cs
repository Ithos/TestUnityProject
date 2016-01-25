using UnityEngine;
using System.Collections;

public class OnClickLoadScene : MonoBehaviour {

    public string sceneName = "";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnMouseUp()
    {
        Application.LoadLevel(sceneName);
    }
}
