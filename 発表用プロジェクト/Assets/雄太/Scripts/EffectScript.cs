using UnityEngine;
using System.Collections;

// 自動で消える
// 旧型スクリプト
public class EffectScript : MonoBehaviour {
    [SerializeField]
    private bool DestroyFlag = true;
    public float DestroyTimer = 2.0f;
    private Transform square;
    [SerializeField]
    private float RotationX;
    [SerializeField]
    private float PositionY = 1;
    [SerializeField]
    private float PositionZ = 1;
    [SerializeField]
    private float ColliderDestroyTimer;
    [SerializeField]
    private float colliderOntimer  = 0;
    [SerializeField]
    private Collider coll;


    void Awake()
    {
        square = GetComponentInParent<Transform>();
        // transform.position = new Vector3(square.position.x, PositionY , square.position.z);
        transform.localPosition = new Vector3(0, PositionY , PositionZ);
        transform.Rotate(new Vector3(RotationX, square.rotation.y, square.rotation.z));
        
    }

    void Start () 
    {
        
        
        // transform.position = square.transform.position;
        if(coll != null)
        {
            Invoke("ColliderOn" , colliderOntimer);
        }

        
        Invoke("ColliderOff" , ColliderDestroyTimer);
        if(DestroyFlag == true)
        {
            Destroy (gameObject, DestroyTimer);
        }
    }

    void ColliderOn()
    {
        coll.enabled= true;
    }

    void ColliderOff()
    {
        Destroy(coll);
    }
    
}