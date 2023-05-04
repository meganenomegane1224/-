using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 
//　スキルのタイプ
public enum SkillType {
	Attack1,
	Attack2,
	Defense1,
	Defense2,
	Speed1,
	Speed2,
	Combo,
	Master
};
 
public class SkillSystem : MonoBehaviour {
    //　スキルを覚える為のスキルポイント
    [SerializeField] private int skillPoint;
    //　スキルを覚えているかどうかのフラグ
    [SerializeField] private bool[] skills;
    //　スキル毎のパラメータ
    [SerializeField] private SkillParameter[] skillParams;
    //　スキルポイントを表示するテキストUI
    public Text skillText;
 
	void Awake () {
		//　スキル数分の配列を確保
		skills = new bool[skillParams.Length];
		SetText ();
	}
	//　スキルを覚える
	public void LearnSkill(SkillType type, int point) {
		skills [(int)type] = true;
		SetSkillPoint (point);
		SetText ();
		CheckOnOff ();
	}
	//　スキルを覚えているかどうかのチェック
	public bool IsSkill(SkillType type) {
		return skills [(int)type];
	}
	//　スキルポイントを減らす
	public void SetSkillPoint(int point) {
		skillPoint -= point;
	}
	//　スキルポイントを取得
	public int GetSkillPoint() {
		return skillPoint;
	}
	//　スキルを覚えられるかチェック
	public bool CanLearnSkill(SkillType type, int spendPoint = 0) {
		//　持っているスキルポイントが足りない
		if (skillPoint < spendPoint) {
			return false;
		}
		//　攻撃UP2は攻撃UP1を覚えていなければダメ
		if (type == SkillType.Attack2) {
			return skills [(int)SkillType.Attack1];
		//　防御UP2は防御UP1を覚えていなければダメ
		} else if (type == SkillType.Defense2) {
			return skills [(int)SkillType.Defense1];
		//　速さUP2は速さUP1を覚えていなければダメ
		} else if (type == SkillType.Speed2) {
			return skills [(int)SkillType.Speed1];
		//　コンボは攻撃UP2と防御２を覚えていなければダメ
		} else if (type == SkillType.Combo) {
			return skills [(int)SkillType.Attack2] && skills [(int)SkillType.Defense2];
		//　マスタースキルは全てのスキルを覚えていなければダメ
		} else if (type == SkillType.Master) {
			return skills [(int)SkillType.Attack2] && skills [(int)SkillType.Defense2] && skills [(int)SkillType.Speed2] && skills [(int)SkillType.Combo];
		}
		return true;
	}
	//　スキル毎にボタンのオン・オフをする処理を実行させる
	void CheckOnOff() {
		foreach (var skillParam in skillParams) {
			skillParam.CheckButtonOnOff ();
		}
	}
 
	void SetText() {
		skillText.text = "スキルポイント：" + skillPoint;
	}
}