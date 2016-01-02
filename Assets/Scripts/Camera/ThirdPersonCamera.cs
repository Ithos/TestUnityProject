using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
	public Transform target;
    public Transform lookAtTarget;
	public float walkDistance;
	public float runDistance;
	public float heigth;
	public string playerTag = "Player";

    public bool fixedCamera = false;

	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;

    public string rotateCameraButton = "Rotate_Camera_Button";
    public string rotateCameraHorizontal = "Rotate_Camera_Horizontal_Buttons";
    public string rotateCameraVertical = "Rotate_Camera_Vertical_Buttons";

    public string changeLookTarget = "Change_Camera_Target";


	public float rotationDamping = 3.0f;
	public float heightDamping = 2.0f;

	private Transform _myTransform;
    private Transform _currentLookAtTarget;

	private float _x;
	private float _y;

	private bool _mouseButtonDown = false;

	private bool _rotateCameraKeyPressed = false;

	void Awake(){
		_myTransform = transform;
        _x = _myTransform.rotation.x;
        _y = _myTransform.rotation.y;
	}

    void OnValidate()
    {
        if (fixedCamera)
        {
            rotateCameraButton = "";
            rotateCameraHorizontal = "";
            rotateCameraVertical = "";
        }
        
    }

	// Use this for initialization
	void Start () {
		if (target == null)
			Debug.LogWarning ("No target found.");
		else {
			CameraSetup (false);
		}

	}
	
	// Update is called once per frame
	void Update () {
        if (!fixedCamera)
        {
            if (Input.GetButtonDown(rotateCameraButton))
            {//1 Representa el boton derecho
                _mouseButtonDown = true;
            }

            if (Input.GetButtonUp(rotateCameraButton))
            {
                _mouseButtonDown = false;
            }

            if (Input.GetButtonDown(rotateCameraHorizontal) || Input.GetButtonDown(rotateCameraVertical))
            {
                _rotateCameraKeyPressed = true;
            }

            if (Input.GetButtonUp(rotateCameraHorizontal) || Input.GetButtonUp(rotateCameraVertical))
            {
                _rotateCameraKeyPressed = false;
            }
        }

        if (Input.GetButtonUp(changeLookTarget))
        {
            swapCameraTarget();
        }
	}

	void LateUpdate(){

		if (target == null) {
			GameObject tmp = GameObject.FindGameObjectWithTag(playerTag);
			if(tmp == null)
				return;
			target = tmp.transform;
		}

		if (_rotateCameraKeyPressed) {
            _x += Input.GetAxis(rotateCameraHorizontal) * xSpeed * 0.02f;
            _y -= Input.GetAxis(rotateCameraVertical) * ySpeed * 0.02f;
			
			RotateCamera();
		}else if (_mouseButtonDown) { 
			_x += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
			_y -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;

			RotateCamera();

		} else {
			//Calculamos los angulos de rotacion actuales
			float wantedRotationAngle = target.eulerAngles.y;
			float wantedHeight = target.position.y + heigth;

			float currentRotationAngle = _myTransform.eulerAngles.y;
			float currenHeight = _myTransform.position.y;

			//Interpolamos el angulo deseado
			currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

			//Interpolamos la altura deseada
			currenHeight = Mathf.Lerp(currenHeight, wantedHeight, heightDamping * Time.deltaTime);

			//Convertimos el angulo en rotacion
			Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

			// Damos la posicion de la camara en el plano x-z a los metros que debe estar por detras del objetivo
			_myTransform.position = target.position;
			_myTransform.position -= currentRotation * Vector3.forward * walkDistance;

			//Damos la altura de la camara
			_myTransform.position = new Vector3(_myTransform.position.x, currenHeight,  _myTransform.position.z);

			_x = _myTransform.rotation.x;
			_y = _myTransform.rotation.y;
		}

		// Miramos al objetivo
		_myTransform.LookAt (_currentLookAtTarget.position);
	}

	void CameraSetup(bool justY){
		if (justY) {
			if(_myTransform.position.y > target.position.y + heigth)
				_myTransform.position = new Vector3 (_myTransform.position.x, target.position.y + heigth, _myTransform.position.z);
			else if(_myTransform.position.y < target.position.y - heigth)
				_myTransform.position = new Vector3 (_myTransform.position.x, target.position.y - heigth, _myTransform.position.z);
		}
		else
			_myTransform.position = new Vector3 (target.position.x, target.position.y + heigth, target.position.z - walkDistance);

		//_myTransform.LookAt (target.position);
        _currentLookAtTarget = target;
	}

	private void RotateCamera(){
		//y = ClampAngle(y, yMinLimit, yMaxLimit );
		
		Quaternion rotation = Quaternion.Euler (_y, _x, 0);
		Vector3 position = rotation * (target.up * heigth - target.forward * walkDistance) + target.position;
		
		_myTransform.rotation = rotation;
		_myTransform.position = position;
	}

    private void swapCameraTarget()
    {
        if (_currentLookAtTarget != lookAtTarget)
        {
            _currentLookAtTarget = lookAtTarget;
        }
        else
        {
            _currentLookAtTarget = target;
        }
    }
}
