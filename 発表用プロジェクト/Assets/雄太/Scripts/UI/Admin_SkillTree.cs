using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Admin_SkillTree : MonoBehaviour
{
    public SkillsTreeUnite[] skillsTreeUnites;
    [System.Serializable]
    public struct SkillsTreeUnite
    {
        public Skills[] skills;
        public bool isEnd;
        public int nextBranchNumber;
    }

    [System.Serializable]
    public struct Skills
    {
        public SkillTreeButton button;
        public SkillType skillType;
        public float addStatus;
        public bool skillOn;
        public int needPoint;
    }

    [SerializeField]
    private Admin_UI admin_UI;
    private Admin admin;
    private Admin_Date admin_Date;

    public enum SkillType
    {
        RaiseAttack,
        RaiseHP,
        RaiseDefence,
        RaiseDamage,
        SPSkill1,
        None
    }

    [SerializeField]
    public static float AllRaiseAttack;
    [SerializeField]
    public static float AllRaiseHP;
    [SerializeField]
    public static float AllRaiseDefence;
    [SerializeField]
    public static float AllRaiseDamage;
    private int selectedTreeUniteNumber;
    private int selectedSkillNumber;
    [SerializeField]
    private Text skillPointText;
    [SerializeField]
    private GameObject[] AlartPanel;
    [SerializeField]
    private TextMeshProUGUI explanationPanel;
    [SerializeField]
    private TextMeshProUGUI explanationPoint;
    [SerializeField]
    private GameObject explanationParent;

    
    // Start is called before the first frame update
    public void StartByAdmin_UI()
    {
        AllRaiseAttack = 0;
        AllRaiseDamage = 0;
        AllRaiseDefence = 0;
        AllRaiseHP = 0;
        admin_UI = GetComponentInParent<Admin_UI>();
        admin = admin_UI.admin;
        admin_Date = admin_UI.admin_Date;

        for (int i = 0; i < skillsTreeUnites.Length; i++)
        {
            var n = PlayerPrefs.GetInt(i.ToString());
            for (int t = 0; t < skillsTreeUnites[i].skills.Length; t++)
            {
                if(t <= n)
                {
                    var g = skillsTreeUnites[i].skills[t];
                    skillsTreeUnites[i].skills[t].skillOn = true;
                    Debug.Log("エラーが呼ばれたときの番号は" + i + "であり、その番号は" + t) ;
                    g.button.StartSet();

                    if(g.skillType == SkillType.RaiseAttack)
                    {
                        AllRaiseAttack += g.addStatus;
                        Debug.Log(AllRaiseAttack);
                    }
                    else if(g.skillType == SkillType.RaiseHP)
                    {
                        AllRaiseHP += g.addStatus;
                        
                    }
                    else if(g.skillType == SkillType.RaiseDefence)
                    {
                        AllRaiseDefence += g.addStatus;
                    }
                    else if(g.skillType == SkillType.RaiseDamage)
                    {
                        AllRaiseDamage += g.addStatus;
                    }
                    else if(g.skillType == SkillType.SPSkill1)
                    {
                        Debug.Log("なんか特別なやつもらえるらしいけどなにも実装してないでwwwww");
                    }
                    admin.RaiseHPBySkillTree();
                }
                if(t == skillsTreeUnites[i].skills.Length-1)
                {
                    skillsTreeUnites[i].isEnd = true;
                }
            }
        }

        skillPointText.text = PlayerPrefs.GetInt("skillPoint").ToString();
    }

    // Update is called once per frame
    
    public void SkillGet()
    {
        var g = skillsTreeUnites[selectedTreeUniteNumber].skills[selectedSkillNumber];
        if(g.skillOn == true)
        {
            Alarm(0);
            Debug.Log("すでに取得していますよ");
            return;
        }
        if(GetSkillAvtiveSelf(selectedTreeUniteNumber , selectedSkillNumber -1) == true)
        {
            if(admin.skillPoints >= g.needPoint)
            {
                g.skillOn = true;
                skillsTreeUnites[selectedTreeUniteNumber].skills[selectedSkillNumber].skillOn= true;
                g.button.SkillOn();
                admin.skillPoints -= g.needPoint;
                skillPointText.text = admin.skillPoints.ToString();
                Debug.Log("取得");


                // 各ステータスの上昇値を設定
                if(g.skillType == SkillType.RaiseAttack)
                {
                    AllRaiseAttack += g.addStatus;
                    Debug.Log(AllRaiseAttack);
                }
                else if(g.skillType == SkillType.RaiseHP)
                {
                    AllRaiseHP += g.addStatus;
                    admin.RaiseHPBySkillTree();
                }
                else if(g.skillType == SkillType.RaiseDefence)
                {
                    AllRaiseDefence += g.addStatus;
                }
                else if(g.skillType == SkillType.RaiseDamage)
                {
                    AllRaiseDamage += g.addStatus;
                }
                else if(g.skillType == SkillType.SPSkill1)
                {
                    Debug.Log("なんか特別なやつもらえるらしいけどなにも実装してないでwwwww");
                }


                if(selectedSkillNumber == skillsTreeUnites[selectedTreeUniteNumber].skills.Length-1)
                {
                    skillsTreeUnites[selectedTreeUniteNumber].isEnd = true;
                    if(JudgeNextLevel(skillsTreeUnites[selectedTreeUniteNumber].nextBranchNumber) == true)
                    {
                        skillsTreeUnites[skillsTreeUnites[selectedTreeUniteNumber].nextBranchNumber].skills[0].skillOn = true;
                        Debug.Log("次の幹へ行けます");
                    }
                    else
                    {
                        Debug.Log("次の幹へはまだ無理だね");
                    }
                }
            }
            else
            {
                Alarm(1);
                Debug.Log("ポイントが足りませんよ");
            }
            
        }
        else
        {
            Alarm(2);
            Debug.Log("まだ取れませんよ");
        }

        admin_UI.ChangeSkillTree = true;
    }
    private bool JudgeNextLevel(int nextTreeNumber)
    {
        if(nextTreeNumber < 0)return false;

        List<int> array = new List<int>();
        for (int i = 0; i < skillsTreeUnites.Length; i++)
        {
            if (skillsTreeUnites[i].nextBranchNumber == nextTreeNumber)
            {
                array.Add(i);
            }
        }
        foreach (var item in array)
        {
            if(skillsTreeUnites[item].isEnd == false)return false;
        }
        return true;
    }

    public bool GetSkillAvtiveSelf(int BranchNumber , int skillNumber)
    {
        if( skillsTreeUnites[BranchNumber].skills[skillNumber].skillOn == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SelectSkill(int treeUniteNumber , int skillNumber)
    {
        selectedTreeUniteNumber = treeUniteNumber;
        selectedSkillNumber = skillNumber;
        var n = skillsTreeUnites[treeUniteNumber].skills[skillNumber];
        Explanation(n.skillType , n.addStatus , n.needPoint);
    }

    private void Alarm(int n)
    {
        Instantiate(AlartPanel[n] ,this.transform);
    }

    private void Explanation(SkillType skillType , float raiseStatus , int point)
    {
        if(skillType == SkillType.RaiseAttack)
        {
            explanationPanel.text = "有効にすると攻撃ステータスを"+raiseStatus+"上昇させる";
            explanationPoint.text = "スキルポイントを"+point+"使用する";
        }
        else if(skillType == SkillType.RaiseDamage)
        {
            explanationPanel.text = "有効にすると敵に与えるダメージが実数値で"+raiseStatus+"上昇する";
            explanationPoint.text = "スキルポイントを"+point+"使用する";
        }
        else if(skillType == SkillType.RaiseDefence)
        {
            explanationPanel.text = "有効にすると防御ステータスを"+raiseStatus+"上昇させる";
            explanationPoint.text = "スキルポイントを"+point+"使用する";
        }
        else if(skillType == SkillType.RaiseHP)
        {
            explanationPanel.text = "有効にするとHPステータスを"+raiseStatus+"上昇させる";
            explanationPoint.text = "スキルポイントを"+point+"使用する";
        }
        else if(skillType == SkillType.SPSkill1)
        {
            explanationPanel.text = "有効にすると特殊スキルを取得";
            explanationPoint.text = "スキルポイントを"+point+"使用する";
        }
    }
}
