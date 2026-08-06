using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FreeSpinSelectButton : MonoBehaviour
{
    [SerializeField] private SlotBehaviour slotManager;
    [SerializeField] private int ButtonIndexx;
    [SerializeField] private Button Optionbutton;
    [SerializeField] private TMP_Text Index;
    [SerializeField] private TMP_Text Multiplier;
    [SerializeField] internal Image SelectEffectImg;
    [SerializeField] internal Vector3 originalScale = new Vector3(1,1,1);
    internal Tween ButtonAnim;
    internal int SpinNumber;
    internal int multiplyer1;
     internal int multiplyer2;
     internal int multiplyer3;

    private void Start()
    {
        if (Optionbutton) Optionbutton.onClick.RemoveAllListeners();
        if (Optionbutton) Optionbutton.onClick.AddListener(OnButtonClicked);
       
    }

    internal void SetData(int index,int m1,int m2,int m3)
    {
        SelectEffectImg.gameObject.SetActive(false);
        if (ButtonAnim != null) ButtonAnim.Kill();
        transform.localScale = originalScale;

        Index.text = index.ToString();
        SpinNumber = index;
        multiplyer1 = m1;
        multiplyer2 = m2;
        multiplyer3 = m3;
        string temps = m1.ToString()+"x" +" "+ m2.ToString()+"x" +"\n" + m3.ToString()+"x";
        Multiplier.text = temps.ToString();
    }

    internal void OnButtonClicked()
    {
        transform.localScale = originalScale;
        SelectEffectImg.gameObject.SetActive(true);

        originalScale = transform.localScale;
        ButtonAnim =transform.DOScale(originalScale * 1.1f, 0.8f).SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Yoyo).From(originalScale * 0.9f);
       
        StartCoroutine(slotManager.FreeSpinOptionSelected(ButtonIndexx));
    }
    internal void SetIntractable(bool isTrue)
    {
        Optionbutton.interactable =isTrue;
    }
}
