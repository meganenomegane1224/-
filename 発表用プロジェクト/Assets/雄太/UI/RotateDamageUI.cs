using UnityEngine;
using System.Collections;
 
public class RotateDamageUI : MonoBehaviour {
 
	void Update() {
		transform.rotation = Camera.main.transform.rotation;
	}
}
 