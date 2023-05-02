using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchChara : MonoBehaviour
{
    private Admin_Enemy adminEnemy;
    private Transform CharaTransform;
    private bool TalkEventFlag;
    // Start is called before the first frame update
    void Start()
    {
        adminEnemy = GetComponentInParent<Admin_Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // void OnTriggerStay(Collider col)
    // {
    //     if(col.tag == "Player")
    //     {
    //         CharaTransform = col.transform;
    //         // adminEnemy.encounterBool = true;
    //         if(adminEnemy.encounterBool == false) 
    //         {
    //             adminEnemy.EncounterCharactor();
    //         }
    //     }
    // }
    // void OnTriggerExit(Collider col)
    // {
    //     if(col.tag == "Player")
    //     {
    //         adminEnemy.LostCharactor();
    //         CharaTransform = null;
    //     }
    // }
    
    public Transform GetCharacterTransform()
    {
        return CharaTransform;
    }
}
