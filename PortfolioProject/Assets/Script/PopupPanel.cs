using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanel : MonoBehaviour
{
    [SerializeField] Text title ,yesBtnText, noBtnText, confirmBtnText;
    [SerializeField] Button yesBtn, noBtn , confirmBtn;



    public void resetPopup()
    {
        yesBtn.gameObject.SetActive(false);
        noBtn.gameObject.SetActive(false);
        confirmBtn.gameObject.SetActive(false);
    }

    public void Open(string titleText, string yes, string no )
    {
        resetPopup();
        title.text = titleText;
        yesBtnText.text = yes;
        noBtnText.text = no;

        yesBtn.gameObject.SetActive(true);
        noBtn.gameObject.SetActive(true);
        gameObject.SetActive(true);

    }
    public void Open(string titleText, string confirm)
    {
        resetPopup();
        title.text = titleText;
        confirmBtnText.text = confirm;

        confirmBtn.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }


    //0 = yesBtn, 1 = confirmBtn
    public void PopupBtnSet(int tmp,Action func)
    {
        if (tmp==0)
        {
            yesBtn.onClick.AddListener(()=>yesBtn.onClick.RemoveAllListeners());
            yesBtn.onClick.AddListener(() => func());
        }
        else if (tmp ==1)
        {
            confirmBtn.onClick.AddListener(() => confirmBtn.onClick.RemoveAllListeners());
            confirmBtn.onClick.AddListener(() => func());
        }
        
    }

}
