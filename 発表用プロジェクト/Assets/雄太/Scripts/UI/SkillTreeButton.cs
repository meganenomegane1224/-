using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeButton : MonoBehaviour
{
    [SerializeField]
    private int treeUniteNumber;
    [SerializeField]
    private int skillNumber;
    [SerializeField]
    public bool value = false;
    [SerializeField]
    private Image image;




    private static readonly Color ON_BG_COLOR = new Color(0.2f, 0.84f, 0.3f);
    
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnClick()
    {
        var admin_SkillTree = GetComponentInParent<Admin_SkillTree>();
        admin_SkillTree.SelectSkill(treeUniteNumber , skillNumber);

    }

    public void SkillOn()
    {
        value = true;
        image.color = ON_BG_COLOR;
    }
    
    public void StartSet()
    {
        value = true;
        image.color = ON_BG_COLOR;
    }
}
