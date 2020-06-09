using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipComment : MonoBehaviour
{
    public Text ATKLabel;
    public Text DEFLabel;
    public Text MTKLabel;
    public Text MEFLabel;

    public void SetData(int id)
    {
        EquipData.RootObject data = EquipData.GetData(id);

        ATKLabel.text = "攻擊：" + data.ATK.ToString();
        ATKLabel.gameObject.SetActive(data.ATK !=0);
        DEFLabel.text = "防禦：" + data.DEF.ToString();
        DEFLabel.gameObject.SetActive(data.DEF != 0);
        MTKLabel.text = "魔攻：" + data.MTK.ToString();
        MTKLabel.gameObject.SetActive(data.MTK != 0);
        MEFLabel.text = "魔防：" + data.MEF.ToString();
        MEFLabel.gameObject.SetActive(data.MEF != 0);
    }
}
