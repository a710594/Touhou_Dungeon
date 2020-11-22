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

        ATKLabel.text = data.ATK.ToString();
        ATKLabel.transform.parent.gameObject.SetActive(data.ATK !=0);
        DEFLabel.text = data.DEF.ToString();
        DEFLabel.transform.parent.gameObject.SetActive(data.DEF != 0);
        MTKLabel.text = data.MTK.ToString();
        MTKLabel.transform.parent.gameObject.SetActive(data.MTK != 0);
        MEFLabel.text = data.MEF.ToString();
        MEFLabel.transform.parent.gameObject.SetActive(data.MEF != 0);
    }

    public void SetData(Equip equip)
    {
        ATKLabel.text = equip.ATK.ToString();
        ATKLabel.transform.parent.gameObject.SetActive(equip.ATK != 0);
        DEFLabel.text = equip.DEF.ToString();
        DEFLabel.transform.parent.gameObject.SetActive(equip.DEF != 0);
        MTKLabel.text = equip.MTK.ToString();
        MTKLabel.transform.parent.gameObject.SetActive(equip.MTK != 0);
        MEFLabel.text = equip.MEF.ToString();
        MEFLabel.transform.parent.gameObject.SetActive(equip.MEF != 0);
    }
}
