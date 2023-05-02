using UnityEngine;

public  class CharaDataSave:MonoBehaviour
{
    public const string CharacterDataKey = "CHARACTER_DATA";

    private void Update(){
                                                                                                
        var exp = Admin.MainEXP;
        var level = Admin.CharaLevel;

        PlayerPrefs.SetFloat(CharacterDataKey,exp);
        PlayerPrefs.SetInt(CharacterDataKey,level);

        PlayerPrefs.Save();
    }
}
