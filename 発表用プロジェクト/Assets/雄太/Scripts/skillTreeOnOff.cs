using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
 
public class skillTreeOnOff : MonoBehaviour {
 
    [SerializeField]
	public GameObject skillSystem;
    //　最初にフォーカスするゲームオブジェクト
    [SerializeField]
    public GameObject firstSelect;
 
	// Update is called once per frame
	void Start(){
		skillSystem.SetActive(false);
	}

	void Update () {
		if (Input.GetKeyDown ("t")) {
			skillSystem.SetActive (!skillSystem.activeSelf);
			EventSystem.current.SetSelectedGameObject (firstSelect);
		}
	}
}