using UnityEngine;
using System.Collections;

public class Gizmo : MonoBehaviour {
	public float radius = 2.0f;
	public Color color = Color.red;
	public void OnDrawGizmos(){
		//Estos gizmos son iconos que se mostraran en el editor para marcar objetos en principio invisibles, no se mostraran en el juego final
		Gizmos.color = color;
		Gizmos.DrawSphere (transform.position, radius);
		//Pueden emplearse iconos personalizados creando una carpeta llamada Gizmos en la carpeta Assets e incluyendo alli los iconos a emplear
		//Despues estos iconos se cargan llamando a: ej->Gizmos.DrawIcon(transform.position, "myIcon.png"); 
	}

}
