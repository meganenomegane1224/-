using UnityEngine;
 
public class RotateDamageUI : MonoBehaviour {
	private Camera main;
 private void Start() {
	main = Camera.main;
 }
	void Update() {
		transform.rotation = main.transform.rotation;
	}
}
 