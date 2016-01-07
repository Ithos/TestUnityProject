using UnityEngine;
using System.Collections;

public class BallPhysics : MonoBehaviour {

    public float forceConstant = 1.0f;

    private Transform _myTransform;

	// Use this for initialization
	void Start () {
        _myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {

        Movement mov = collision.collider.GetComponent<Movement>();

        if (mov != null)
        {
            Rigidbody body = GetComponent<Rigidbody>();
            float force = new Vector3(mov.MovementVector.x, 0.0f, mov.MovementVector.z).magnitude * forceConstant;
            Vector3 vec = (_myTransform.position - mov.transform.position).normalized * force;
            body.AddForce(vec.x, vec.y, vec.z, ForceMode.Impulse);
        }
    }
}
