using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


[System.Serializable]
public class FireResultSlots
{
    [SerializeField] internal FireResultSlot[] ResultSlots;
}
public class BonusController : MonoBehaviour
{

    [SerializeField] private SlotBehaviour slotBehaviour;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private AudioController audioManager;
    [Header("FireLink Slot")]
    [SerializeField] private FireResultSlots[] fireslot;
    [SerializeField]
    private GameObject[] Rowbanner;
    [SerializeField]
    private TMP_Text[] Rowbannertext;
    [SerializeField]
    private ImageAnimation[] RowbannerAnim;
    [SerializeField] private TMP_Text RemainingSlotTxt;
    [SerializeField] private ImageAnimation YellowPlarticle;

    private int NoOFBanneropen;
    private Coroutine BonusCoroutine;

    [Header("Movable Sun")]
    [SerializeField] private Image MovableSun;
    [SerializeField] private TMP_Text MovableSunText;
    [SerializeField] private Transform SunHitposition;
    [SerializeField] private ImageAnimation SunBlastAnim;
    [SerializeField] private double WinAmount;

    [Header("Freespin")]
    [SerializeField] private GameObject FreespinTextObj;
    [SerializeField] private GameObject FireLinkFeatureStartupPage;
    [SerializeField] private Button FireLinkFeatureStartButton;
    private Coroutine StartBonusRoutine;

    [Header("Slots")]
    [SerializeField] private GameObject NormalSlot;
    [SerializeField] private GameObject BonusSlot;
    // Title visibility now lives on UIManager (it owns both orientations' titles).

    private void Start()
    {
        if (FireLinkFeatureStartButton) FireLinkFeatureStartButton.onClick.RemoveAllListeners();
        if (FireLinkFeatureStartButton) FireLinkFeatureStartButton.onClick.AddListener(OnBonusStartClicked);

        foreach (var res in fireslot)
        {
            for (int i = 0; i < res.ResultSlots.Length; i++)
            {
                res.ResultSlots[i].ShowRandomeResult(slotBehaviour.myImages[Random.Range(0, slotBehaviour.myImages.Length - 3)], slotBehaviour.myImages[Random.Range(0, slotBehaviour.myImages.Length - 3)], slotBehaviour.myImages[Random.Range(0, slotBehaviour.myImages.Length - 3)]);
                int x = Random.Range(0, slotBehaviour.myImages.Length - 6);
                int y = Random.Range(0, slotBehaviour.myImages.Length - 6);
                int z = Random.Range(0, slotBehaviour.myImages.Length - 6);
                int q = Random.Range(0, slotBehaviour.myImages.Length - 6);
                int p = Random.Range(0, slotBehaviour.myImages.Length - 6);


                res.ResultSlots[i].SetRandomeImages(slotBehaviour.myImages[x], slotBehaviour.myImages[y], slotBehaviour.myImages[z], slotBehaviour.myImages[q], slotBehaviour.myImages[p]);
            }

        }

        //StartCoroutine(TestStart());

        //    StartCoroutine(testStop());
    }

    internal IEnumerator BonusStartupPage(int spinCount)
    {
        FireLinkFeatureStartupPage.SetActive(true);
        FireLinkFeatureStartButton.interactable = true;
        uiManager.OpenPopup(uiManager.BonusPopup);
        yield return new WaitForSeconds(1.5f);
        Debug.Log("dev_test" + "3" + slotBehaviour.WasAutoSpinOn);
        if (StartBonusRoutine != null)
        {
            Debug.Log("dev_test" + "4");

            StopCoroutine(StartBonusRoutine);
            StartBonusRoutine = null;
        }
        if (slotBehaviour.WasAutoSpinOn)
        {
            Debug.Log("dev_test" + "5");
            if (FireLinkFeatureStartButton.interactable)
            {
                FireLinkFeatureStartButton.interactable = false;
                StartBonusRoutine = StartCoroutine(StartBonus(spinCount));
            }
        }
        // Route through UIManager so the correct orientation's title is hidden and the
        // state survives an orientation switch mid-bonus.
        if (uiManager)
        {
            uiManager.TitleHiddenByBonus = true;
            uiManager.ApplyTitleState();
        }
    }
    private void OnBonusStartClicked()
    {
        FireLinkFeatureStartButton.interactable = false;
        if (StartBonusRoutine != null)
        {
            StopCoroutine(StartBonusRoutine);
            StartBonusRoutine = null;
        }
        StartBonusRoutine = StartCoroutine(StartBonus(socketManager.ResultData.features.bonus.spinCount));
    }
    internal IEnumerator StartBonus(int spinCount)
    {
        NoOFBanneropen = 0;
        WinAmount = 0;
        if (slotBehaviour.IsFreeSpin)
        {
            FreespinTextObj.SetActive(false);
        }

        StartCoroutine(uiManager.SlideChangeAnimation(() => changeSlide(true)));
        SetinitialReel();
        StartCoroutine(PlaySlideChangeTransitionAnimation(spinCount));
        // StartCoroutine(BonusTweenRoutine(spinCount));
        yield return null;
    }

    IEnumerator ChangeBannnerText(int ScatterCount)
    {
        int rowOpen = CheckNoOfRow();
        int index = 0;
        for (int i = 0; i < socketManager.InitFeature.bonusTrigger.Count; i++)
        {
            if ((int)socketManager.InitFeature.bonusTrigger[i].rows == rowOpen)
            {
                index = i;
            }
        }

        for (int i = Rowbannertext.Length - 1; i >= 0; i--)
        {
            if (socketManager.InitFeature.bonusTrigger[i + 1].count[0] - ScatterCount > 0)
            {
                Rowbannertext[Rowbannertext.Length - i - 1].text = (socketManager.InitFeature.bonusTrigger[i + 1].count[0] - ScatterCount).ToString();
            }
            else
            {
                Rowbannertext[Rowbannertext.Length - i - 1].text = "0";
            }
        }
        for (int i = Rowbannertext.Length - 1; i >= 0; i--)
        {

            if (8 - CheckNoOfRow() <= i)
            {
                Rowbannertext[i].text = "0";
                RowbannerAnim[i].gameObject.SetActive(true);

                RowbannerAnim[i].StopAnimation();

                RowbannerAnim[i].StartAnimation();
                if (Rowbanner[i].gameObject.activeInHierarchy)
                {
                    if (audioManager) audioManager.PlayWLAudio("fireBlast");
                }
                yield return new WaitForSeconds(1f);
                audioManager.StopWLAaudio();
                Rowbanner[i].SetActive(false);
                RowbannerAnim[i].gameObject.SetActive(false);
            }
            if (8 - ScatterCount + 4 * (3 - i) <= 0)
            {

                //Rowbannertext[i].text = "0";
                //RowbannerAnim[i].gameObject.SetActive(true);

                //RowbannerAnim[i].StopAnimation();

                //RowbannerAnim[i].StartAnimation();
                //if (Rowbanner[i].gameObject.activeInHierarchy)
                //{
                //    if (audioManager) audioManager.PlayWLAudio("fireBlast");
                //}
                //yield return new WaitForSeconds(1f);
                //audioManager.StopWLAaudio();
                //Rowbanner[i].SetActive(false);
                //RowbannerAnim[i].gameObject.SetActive(false);

            }
            Transform obj = Rowbanner[i].transform.GetChild(1).GetChild(1);

            obj.GetComponent<ImageAnimation>().StopAnimation();
            obj.GetComponent<ImageAnimation>().StartAnimation();
        }

    }


    private void changeSlide(bool isBonusTile)
    {
        FireLinkFeatureStartupPage.SetActive(false);
        if (isBonusTile)
        {
            NormalSlot.SetActive(false);
            BonusSlot.SetActive(true);
        }
        else
        {
            NormalSlot.SetActive(true);
            BonusSlot.SetActive(false);

        }
    }
    private void SetinitialReel()
    {

        foreach (var res in fireslot)
        {
            foreach (var slot in res.ResultSlots)
            {
                if (slot.isFixed)
                {
                    slot.isFixed = false;
                    slot.ShowImage.transform.localScale /= slot.FixedSymbolScale;
                    slot.NumText.gameObject.SetActive(false);
                    if (slot.JackpotImage) slot.JackpotImage.gameObject.SetActive(false);
                    StartCoroutine(slot.ApplyFilter(true));
                    slot.ShowRandomeResult(slotBehaviour.myImages[Random.Range(0, slotBehaviour.myImages.Length - 3)], slotBehaviour.myImages[Random.Range(0, slotBehaviour.myImages.Length - 3)], slotBehaviour.myImages[Random.Range(0, slotBehaviour.myImages.Length - 3)]);
                }

            }
        }
        foreach (var obj in Rowbanner)
        {
            obj.SetActive(true);
        }
        for (int i = 0; i < socketManager.ResultData.features.bonus.scatterValues.Count; i++)
        {
            fireslot[socketManager.ResultData.features.bonus.scatterValues[i].index[0] + 4].ResultSlots[socketManager.ResultData.features.bonus.scatterValues[i].index[1]].isSlotFixed(true, socketManager.ResultData.features.bonus.scatterValues[i].value, slotBehaviour.Sun_Sprite, slotBehaviour.GetJackpotSprite(socketManager.ResultData.features.bonus.scatterValues[i].value), slotBehaviour.GetJackpotImageScale(socketManager.ResultData.features.bonus.scatterValues[i].value));
            StartCoroutine(fireslot[socketManager.ResultData.features.bonus.scatterValues[i].index[0] + 4].ResultSlots[socketManager.ResultData.features.bonus.scatterValues[i].index[1]].ApplyFilter(false));
        }

        SunBlastAnim.gameObject.SetActive(false);
        slotBehaviour.TotalWin_text.text = WinAmount.ToString();
    }
    private int CheckNoOfRowk()
    {
        // int x= socketManager.ResultData.features.bonus.bonus.matrix.Count;
        int x = 0;
        x = socketManager.ResultData.features.bonus.matrix.Count;
        return x;
    }

    private int CheckNoOfRow()
    {
        // int x= socketManager.ResultData.features.bonus.bonus.matrix.Count;
        int x = 0;
        for (int i = 0; i < socketManager.InitFeature.bonusTrigger.Count; i++)
        {
            // Debug.Log("------scatterValues.Count " + socketManager.ResultData.features.bonus.scatterValues.Count + "------InitFeature.bonusTrigger[i].count[1] " + socketManager.InitFeature.bonusTrigger[i].count[0] + "--------" + socketManager.InitFeature.bonusTrigger[i].count[1]);

            if (socketManager.ResultData.features.bonus.scatterValues.Count >= socketManager.InitFeature.bonusTrigger[i].count[0] && socketManager.ResultData.features.bonus.scatterValues.Count <= socketManager.InitFeature.bonusTrigger[i].count[1])
            {
                x = (int)socketManager.InitFeature.bonusTrigger[i].rows;
                break;
            }
            else
            {
                x = 4;
            }
        }
        return x;
    }
    IEnumerator PlaySlideChangeTransitionAnimation(int SpinCount)
    {
        yield return (ChangeBannnerText(socketManager.ResultData.features.bonus.scatterCount));

        while (socketManager.ResultData.features.bonus.spinCount > 0)
        {
            yield return (BonusTweenRoutine(SpinCount));
            SpinCount--;
        }

        RemainingSlotTxt.text = "0";

        yield return (MovableSunAnim());
        StartCoroutine(uiManager.SlideChangeAnimation(() => changeSlide(false)));
        if (slotBehaviour.IsFreeSpin)
        {
            FreespinTextObj.SetActive(true);
        }
        if (uiManager)
        {
            uiManager.TitleHiddenByBonus = false;
            uiManager.ApplyTitleState();
        }
        yield return new WaitForSeconds(2f);
        //  slotBehaviour.CheckWinPopups();
        slotBehaviour.CheckPopups = false;
    }


    IEnumerator BonusTweenRoutine(int spinCount)
    {
        ChangeRemainingSlotText();
        Debug.Log("------scatterValues.Count " + socketManager.ResultData.features.bonus.scatterValues.Count);
        yield return (ChangeBannnerText(socketManager.ResultData.features.bonus.scatterCount));
        if (audioManager) audioManager.PlayWLAudio("bonusSpin");
        yield return (InitiliseTweening());

        socketManager.AccumulateResult("SPIN");

        yield return new WaitUntil(() => socketManager.isResultdone);

        yield return (StopTweening());



        // yield return new WaitForSeconds(2f);

    }

    private void ChangeRemainingSlotText()
    {
        if (socketManager.ResultData.features.bonus.spinCount >= int.Parse(RemainingSlotTxt.text))
        {

            YellowPlarticle.StopAnimation();
            YellowPlarticle.StartAnimation();
        }
        if (socketManager.ResultData.features.bonus.spinCount >= 0) RemainingSlotTxt.text = socketManager.ResultData.features.bonus.spinCount.ToString();


    }
    IEnumerator InitiliseTweening()
    {
        foreach (var res in fireslot)
        {
            foreach (var slot in res.ResultSlots)
            {
                slot.InitializeTweening(slot.Slot.transform);
                if (slot.isFixed == false)
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
    }
    IEnumerator StopTweening()
    {
        for (int i = 0; i < socketManager.ResultData.features.bonus.scatterValues.Count; i++)
        {

            if (fireslot[socketManager.ResultData.features.bonus.scatterValues[i].index[0] + 8 - CheckNoOfRowk()].ResultSlots[socketManager.ResultData.features.bonus.scatterValues[i].index[1]].isFixed == false)
            {
                Debug.Log("pppppScatter :x   :" + socketManager.ResultData.features.bonus.scatterValues[i].index[0] + "  y:" + socketManager.ResultData.features.bonus.scatterValues[i].index[1] + "      " + (socketManager.ResultData.features.bonus.scatterValues[i].index[0] + 8 - CheckNoOfRowk()) + "   check no of row" + CheckNoOfRowk());
                fireslot[socketManager.ResultData.features.bonus.scatterValues[i].index[0] + 8 - CheckNoOfRowk()].ResultSlots[socketManager.ResultData.features.bonus.scatterValues[i].index[1]].isSlotFixed(false, socketManager.ResultData.features.bonus.scatterValues[i].value, slotBehaviour.Sun_Sprite, slotBehaviour.GetJackpotSprite(socketManager.ResultData.features.bonus.scatterValues[i].value), slotBehaviour.GetJackpotImageScale(socketManager.ResultData.features.bonus.scatterValues[i].value));
                StartCoroutine(fireslot[socketManager.ResultData.features.bonus.scatterValues[i].index[0] + 8 - CheckNoOfRowk()].ResultSlots[socketManager.ResultData.features.bonus.scatterValues[i].index[1]].ApplyFilter(false));
            }
        }


        foreach (var res in fireslot)
        {
            for (int k = 0; k < res.ResultSlots.Length; k++)
            {
                res.ResultSlots[k].StopTweening();
                if (res.ResultSlots[k].isFixed == false)
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }
            yield return new WaitForSeconds(0.05f);
        }

        for (int i = 0; i < socketManager.ResultData.features.bonus.scatterValues.Count; i++)
        {
            if (fireslot[socketManager.ResultData.features.bonus.scatterValues[i].index[0] + 8 - CheckNoOfRowk()].ResultSlots[socketManager.ResultData.features.bonus.scatterValues[i].index[1]].isFixed == false)
            {
                fireslot[socketManager.ResultData.features.bonus.scatterValues[i].index[0] + 8 - CheckNoOfRowk()].ResultSlots[socketManager.ResultData.features.bonus.scatterValues[i].index[1]].isFixed = true;

            }
        }


        if (audioManager) audioManager.StopWLAaudio();
    }
    IEnumerator MovableSunAnim()
    {
        for (int i = 0; i < fireslot.Length; i++)
        {
            for (int j = 0; j < fireslot[i].ResultSlots.Length; j++)
            {
                if (fireslot[i].ResultSlots[j].isFixed == true)
                {
                    fireslot[i].ResultSlots[j].playFrameBurnAnimation();

                    MovableSun.transform.position = fireslot[i].ResultSlots[j].transform.position;
                    MovableSunText.text = fireslot[i].ResultSlots[j].NumText.text;
                    //MovableSun.color = new Color(0, 0, 0, 0);
                    MovableSun.gameObject.SetActive(true);
                    // MovableSun.DOColor(new Color(1,1,1,1), 0.8f);
                    if (slotBehaviour.IsTurboOn) yield return new WaitForSeconds(0.5f);
                    else yield return new WaitForSeconds(0.8f);

                    SunBlastAnim.StopAnimation();
                    SunBlastAnim.gameObject.SetActive(false);

                    MovableSun.GetComponent<ImageAnimation>().StartAnimation();
                    audioManager.StopWLAaudio();
                    if (audioManager) audioManager.PlayWLAudio("fireWhoose");
                    float speed = 10;
                    if (slotBehaviour.IsTurboOn) speed = 20;


                    Tween moveTween = MovableSun.transform.DOMove(SunHitposition.position, speed).SetSpeedBased(true).SetEase(Ease.Linear);
                    yield return moveTween.WaitForCompletion();

                    UpdateCurrentWin(double.Parse(MovableSunText.text));
                    MovableSun.gameObject.SetActive(false);


                    MovableSun.GetComponent<ImageAnimation>().StopAnimation();
                    audioManager.StopWLAaudio();
                }
            }
        }
        if (slotBehaviour.Balance_text) slotBehaviour.Balance_text.text = socketManager.PlayerData.balance.ToString("F3");
    }

    private void UpdateCurrentWin(double winAmount)
    {
        SunBlastAnim.gameObject.SetActive(true);
        SunBlastAnim.StartAnimation();

        WinAmount += winAmount;
        slotBehaviour.TotalWin_text.text = WinAmount.ToString();
    }
}
