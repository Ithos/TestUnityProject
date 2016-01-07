using UnityEngine;
using System.Collections;

public class CharacterBallCollision : MonoBehaviour {

    public float forceConstant = 1.0f;
    public string targetTag = "";

    private Transform _myTransform;

	// Use this for initialization
	void Start () {
        _myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.collider.tag == targetTag)
        {
            Rigidbody body = collision.collider.GetComponent<Rigidbody>();
            Movement mov = GetComponent<Movement>();

            float force = new Vector3(mov.MovementVector.x, 0.0f, mov.MovementVector.z).magnitude * forceConstant;
            Vector3 vec = (body.transform.position - _myTransform.position ).normalized * force;
            body.AddForce(vec.x, vec.y, vec.z, ForceMode.Impulse);
        }
    }
}
