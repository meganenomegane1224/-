using Random = UnityEngine.Random;
using UnityEngine;

public class MobItemDropper : MonoBehaviour
{
   [SerializeField] [Range(0,1)] private float dropRate = 0.1f;
   [SerializeField] private Item itemPrehub;
   [SerializeField] private int number = 1;

   public void DropIfNeeded()
   {

    if (Random.Range(0,1f) >= dropRate) return;

    for(var i = 0; i <= number; i++)
    {
        var item = Instantiate(itemPrehub, transform.position,Quaternion.identity);
        item.Initialize();
    }
   }
  
}
