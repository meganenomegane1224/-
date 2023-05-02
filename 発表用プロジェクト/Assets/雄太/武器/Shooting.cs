using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shooting : MonoBehaviour
{

    public GameObject bulletPrefab;
    public float shotSpeed;
    public int shotCount = 30;
    [SerializeField]
    private int MaxShotCount;
    private float shotInterval;
    [SerializeField]
    private int IntervalCount = 10;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] audioClip;
    [SerializeField]
    private int bulletRange;
    [SerializeField]
    private Animator animator;
    void Update()
    {
        if(EventSystem.current.IsPointerOverGameObject())
			{
                return;
            }

        if (Input.GetKey(KeyCode.Mouse0))
        {

            shotInterval += 1;

            if (shotInterval % IntervalCount == 0)
            {
                if(shotCount > 0)
                {
                    animator.SetTrigger("shot");
                    shotCount -= 1;
                    GameObject bullet = (GameObject)Instantiate(bulletPrefab, transform.position, Quaternion.Euler(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y, 0));
                    Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                    bulletRb.AddForce(transform.forward * shotSpeed);
                    audioSource.PlayOneShot(audioClip[0]);

                //�ˌ�����Ă���3�b��ɏe�e�̃I�u�W�F�N�g��j�󂷂�.

                    Destroy(bullet, bulletRange);
                }
                else
                {
                    audioSource.PlayOneShot(audioClip[1]);
                } 


            }


        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            shotCount = MaxShotCount;

        }

    }

    public void Shot()
    {
        
    }
}
