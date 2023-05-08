using UnityEngine;

public class ProcessWeaponAnim : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WeaponAnim()
    {
        animator.SetTrigger("1Attack");
    }
}
