using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Menu UI")]
    [SerializeField]
    private Button Menu_Button;
    [SerializeField]
    private GameObject Menu_Object;
    [SerializeField]
    private RectTransform Menu_RT;

    [Header("Settings UI")]
    [SerializeField]
    private Button Settings_Button;
    [SerializeField]
    private GameObject Settings_Object;
    [SerializeField]
    private RectTransform Settings_RT;
    [SerializeField]
    private Button Terms_Button;
    [SerializeField]
    private Button Privacy_Button;

    [SerializeField]
    internal Button Exit_Button;
    [SerializeField]
    private GameObject Exit_Object;
    [SerializeField]
    private RectTransform Exit_RT;

    [SerializeField]
    internal Button Paytable_Button;
    [SerializeField]
    private GameObject Paytable_Object;
    [SerializeField]
    private RectTransform Paytable_RT;

    [Header("Popus UI")]
    [SerializeField]
    private GameObject MainPopup_Object;

    [Header("About Popup")]
    [SerializeField]
    private GameObject AboutPopup_Object;
    [SerializeField]
    private Button AboutExit_Button;
    [SerializeField]
    private Image AboutLogo_Image;
    [SerializeField]
    private Button Support_Button;

    [Header("Paytable Popup")]
    [SerializeField]
    private Button Paytable_Privious_Button;
    [SerializeField]
    private Button paytable_Next_Button;
    [SerializeField]
    private GameObject[] PaytablePages;
    [SerializeField]
    private int CurrentPaytablePage;
    [SerializeField]
    private GameObject PaytablePopup_Object;
    [SerializeField]
    private Button PaytableExit_Button;
    [SerializeField]
    private Button GoToHomePage_Button;
    [SerializeField]
    private TMP_Text[] SymbolsText;
    [SerializeField]
    private TMP_Text FreeSpin_Text;
    [SerializeField]
    private TMP_Text FireLink_Text;
    [SerializeField]
    private TMP_Text Jackpot_Text;
    [SerializeField]
    private TMP_Text Bonus_Text;
    [SerializeField]
    private TMP_Text Wild_Text;

    [Header("Settings Popup")]
    [SerializeField]
    private GameObject SettingsPopup_Object;
    [SerializeField]
    private Button SettingsExit_Button;
    [SerializeField]
    internal Button Sound_Button;
    [SerializeField]
    internal Button Music_Button;

    [SerializeField]
    internal Button SoundOFF_Button;
    [SerializeField]
    internal Button MusicOFF_Button;

    [Header("Bonus Win Popup")]
    [SerializeField] private Image BonusWinBG;
    [SerializeField] private GameObject BonusWinObj;
    [SerializeField] private GameObject BonusWinTexts;
    [SerializeField] internal GameObject BonusPopup;
    [SerializeField] private TMP_Text BonusWinAmount;

    [Header("Win Popup")]
    [SerializeField]
    private Sprite BigWin_Sprite;
    [SerializeField]
    private Sprite HugeWin_Sprite;
    [SerializeField]
    private Sprite MegaWin_Sprite;
    [SerializeField]
    private Sprite Jackpot_Sprite;
    [SerializeField]
    private Image Win_Image;
    [SerializeField]
    private GameObject WinPopup_Object;
    [SerializeField]
    private TMP_Text Win_Text;
    [SerializeField] private Button SkipWinAnimation;

    [Header("FreeSpins Popup")]
    [SerializeField] internal FreeSpinSelectButton[] FreeSpinOptionButton;
    [SerializeField]
    private GameObject FreeSpinPopup_Object;
    [SerializeField]
    internal GameObject FreespinBorder;
    [SerializeField]
    internal GameObject NormalBorder;
    [SerializeField]
    private TMP_Text Free_Text;
    [SerializeField]
    private GameObject FreeSpinStartupPanel;
    [SerializeField] private TMP_Text NoOFFreeSpintxt;
    [SerializeField] private TMP_Text Multiplyers;
    [SerializeField] private int NoOFFreeSpins;
    [SerializeField] private Button StartFreeSpinButton;
    [SerializeField] internal TMP_Text FreespinWinTxt;
    [SerializeField] internal double FreespinWinAmount = 0;

    [Header("Splash Screen")]
    [SerializeField]
    private GameObject Loading_Object;
    [SerializeField]
    private Image Loading_Image;
    [SerializeField]
    private TMP_Text Loading_Text;
    [SerializeField]
    private TMP_Text LoadPercent_Text;
    [SerializeField]
    private Button QuitSplash_button;

    [Header("Disconnection Popup")]
    [SerializeField]
    private Button CloseDisconnect_Button;
    [SerializeField]
    private GameObject DisconnectPopup_Object;

    [Header("AnotherDevice Popup")]
    [SerializeField]
    private Button CloseAD_Button;
    [SerializeField]
    private GameObject ADPopup_Object;

    [Header("Reconnection Popup")]
    [SerializeField]
    private TMP_Text reconnect_Text;
    [SerializeField]
    private GameObject ReconnectPopup_Object;

    [Header("LowBalance Popup")]
    [SerializeField]
    private Button LBExit_Button;
    [SerializeField]
    private GameObject LBPopup_Object;

    [Header("Slide Change anim Popup")]
    [SerializeField] private GameObject SlideChangeObj;
    [SerializeField] private GameObject SlideChangePopup;
    [SerializeField] private Transform upPos, downPos, centerPos;
    [SerializeField] private ImageAnimation[] Lalterns;

    [Header("Quit Popup")]
    [SerializeField]
    private GameObject QuitPopup_Object;
    [SerializeField]
    private Button YesQuit_Button;
    [SerializeField]
    private Button NoQuit_Button;
    [SerializeField]
    private Button CrossQuit_Button;

    [Header("Mobile and PC UI")]
    [SerializeField]
    internal GameObject Mobile_UltimateFireLinkText;
    [SerializeField]
    internal GameObject PC_UltimateFireLinkText;
    [SerializeField]
    internal GameObject Mobile_MajorMiniMinor;
    [SerializeField]
    internal GameObject PC_MajorMiniMinor;
    [SerializeField]
    internal GameObject Mobile_ButonPanel;
    [SerializeField]
    internal GameObject PC_Buttonpanel;
    [SerializeField]
    internal GameObject Mobile_TopBar;

    [Space]
    [SerializeField]
    private AudioController audioController;
    [SerializeField]
    private Button m_AwakeGameButton;

    [SerializeField]
    internal Button GameExit_Button;

    [SerializeField]
    private SlotBehaviour slotManager;

    [SerializeField]
    private SocketIOManager socketManager;
    [SerializeField] internal GameObject RaycastBlocker;
    private bool isMusicMuted = false;
    private bool isSoundMuted = false;
    private bool isExit = false;
    private Tween WinPopupTextTween;
    private Tween ClosePopupTween;
    internal int FreeSpins;

    private void Start()
    {

        UpdateThebuttons();
    }
    internal void UpdateThebuttons()
    {
        if (Menu_Button) Menu_Button.onClick.RemoveAllListeners();
        if (Menu_Button) Menu_Button.onClick.AddListener(OpenMenu);

        if (Exit_Button) Exit_Button.onClick.RemoveAllListeners();
        if (Exit_Button) Exit_Button.onClick.AddListener(CloseMenu);

        //if (About_Button) About_Button.onClick.RemoveAllListeners();
        //if (About_Button) About_Button.onClick.AddListener(delegate { OpenPopup(AboutPopup_Object); });

        if (AboutExit_Button) AboutExit_Button.onClick.RemoveAllListeners();
        if (AboutExit_Button) AboutExit_Button.onClick.AddListener(delegate { ClosePopup(AboutPopup_Object); });

        if (Paytable_Button) Paytable_Button.onClick.RemoveAllListeners();
        if (Paytable_Button) Paytable_Button.onClick.AddListener(delegate { OpenPopup(PaytablePopup_Object); audioController.PlayButtonAudio(); });

        if (PaytableExit_Button) PaytableExit_Button.onClick.RemoveAllListeners();
        if (PaytableExit_Button) PaytableExit_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); audioController.PlayButtonAudio(); });

        if (GoToHomePage_Button) GoToHomePage_Button.onClick.RemoveAllListeners();
        if (GoToHomePage_Button) GoToHomePage_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); audioController.PlayButtonAudio(); });

        if (paytable_Next_Button) paytable_Next_Button.onClick.RemoveAllListeners();
        if (paytable_Next_Button) paytable_Next_Button.onClick.AddListener(OnPaytablePriviousButtonClicked);

        if (Paytable_Privious_Button) Paytable_Privious_Button.onClick.RemoveAllListeners();
        if (Paytable_Privious_Button) Paytable_Privious_Button.onClick.AddListener(OnPaytableNextButtonClicked);


        if (Settings_Button) Settings_Button.onClick.RemoveAllListeners();
        if (Settings_Button) Settings_Button.onClick.AddListener(delegate { OpenPopup(SettingsPopup_Object); });

        if (SettingsExit_Button) SettingsExit_Button.onClick.RemoveAllListeners();
        if (SettingsExit_Button) SettingsExit_Button.onClick.AddListener(delegate { ClosePopup(SettingsPopup_Object); });



        if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
        if (GameExit_Button) GameExit_Button.onClick.AddListener(delegate
        {
            OpenPopup(QuitPopup_Object);
            Debug.Log("Quit event: pressed Big_X button");
            audioController.PlayButtonAudio();
        });

        if (NoQuit_Button) NoQuit_Button.onClick.RemoveAllListeners();
        if (NoQuit_Button) NoQuit_Button.onClick.AddListener(delegate
        {
            if (!isExit)
            {
                ClosePopup(QuitPopup_Object);
                Debug.Log("quit event: pressed NO Button ");
                audioController.PlayButtonAudio();
            }
        });

        if (CrossQuit_Button) CrossQuit_Button.onClick.RemoveAllListeners();
        if (CrossQuit_Button) CrossQuit_Button.onClick.AddListener(delegate
        {
            if (!isExit)
            {
                ClosePopup(QuitPopup_Object);
                Debug.Log("quit event: pressed Small_X Button ");

            }
        });

        if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
        if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); });

        if (YesQuit_Button) YesQuit_Button.onClick.RemoveAllListeners();
        if (YesQuit_Button) YesQuit_Button.onClick.AddListener(delegate
        {
            CallOnExitFunction();
            Debug.Log("quit event: pressed YES Button ");

        });

        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(delegate { CallOnExitFunction(); socketManager.ReactNativeCallOnFailedToConnect(); }); //BackendChanges

        if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
        if (CloseAD_Button) CloseAD_Button.onClick.AddListener(CallOnExitFunction);

        if (QuitSplash_button) QuitSplash_button.onClick.RemoveAllListeners();
        if (QuitSplash_button) QuitSplash_button.onClick.AddListener(delegate { OpenPopup(QuitPopup_Object); });

        if (Sound_Button) Sound_Button.onClick.RemoveAllListeners();
        if (Sound_Button) Sound_Button.onClick.AddListener(ToggleSound);

        if (Music_Button) Music_Button.onClick.RemoveAllListeners();
        if (Music_Button) Music_Button.onClick.AddListener(ToggleMusic);

        if (SoundOFF_Button) SoundOFF_Button.onClick.RemoveAllListeners();
        if (SoundOFF_Button) SoundOFF_Button.onClick.AddListener(ToggleSound);

        if (MusicOFF_Button) MusicOFF_Button.onClick.RemoveAllListeners();
        if (MusicOFF_Button) MusicOFF_Button.onClick.AddListener(ToggleMusic);


        if (SkipWinAnimation) SkipWinAnimation.onClick.RemoveAllListeners();
        if (SkipWinAnimation) SkipWinAnimation.onClick.AddListener(SkipWin);

        if (StartFreeSpinButton) StartFreeSpinButton.onClick.RemoveAllListeners();
        if (StartFreeSpinButton) StartFreeSpinButton.onClick.AddListener(OnClickStartFreeSpinBtn);

        ApplyAudioState();
    }

    // Mute state lives only on the AudioSources, so it must be re-applied to the
    // orientation's button set every time CanvasScalerSwitcher rebinds them.
    internal void ApplyAudioState()
    {
        if (audioController)
        {
            audioController.ToggleMute(isMusicMuted, "bg");
            audioController.ToggleMute(isSoundMuted, "button");
            audioController.ToggleMute(isSoundMuted, "wl");
        }

        // Only swap when both halves of the pair are bound. The settings popup binds a
        // single always-visible toggle button and leaves the *OFF_Button ref empty --
        // hiding it there would leave nothing to click to toggle back.
        if (Music_Button && MusicOFF_Button)
        {
            Music_Button.gameObject.SetActive(!isMusicMuted);
            MusicOFF_Button.gameObject.SetActive(isMusicMuted);
        }

        if (Sound_Button && SoundOFF_Button)
        {
            Sound_Button.gameObject.SetActive(!isSoundMuted);
            SoundOFF_Button.gameObject.SetActive(isSoundMuted);
        }
    }

    private bool isPcView;

    /// <summary>Set by BonusController: the title is hidden for the duration of the bonus.</summary>
    internal bool TitleHiddenByBonus;

    public void IsPcToggle(bool isPC)
    {
        isPcView = isPC;

        if (isPC)
        {
            Mobile_MajorMiniMinor.SetActive(false);
            Mobile_TopBar.SetActive(false);
            Mobile_ButonPanel.SetActive(false);

            PC_Buttonpanel.SetActive(true);
            PC_MajorMiniMinor.SetActive(true);
        }
        else
        {
            Mobile_MajorMiniMinor.SetActive(true);
            Mobile_TopBar.SetActive(true);
            Mobile_ButonPanel.SetActive(true);

            PC_Buttonpanel.SetActive(false);
            PC_MajorMiniMinor.SetActive(false);
        }

        ApplyTitleState();
    }

    /// <summary>
    /// Title visibility is a function of BOTH orientation and bonus state. Driving it
    /// from IsPcToggle alone re-showed the title when the player rotated mid-bonus,
    /// because the switch has no idea the bonus had hidden it.
    /// </summary>
    internal void ApplyTitleState()
    {
        bool show = !TitleHiddenByBonus;
        if (Mobile_UltimateFireLinkText) Mobile_UltimateFireLinkText.SetActive(!isPcView && show);
        if (PC_UltimateFireLinkText) PC_UltimateFireLinkText.SetActive(isPcView && show);
    }

    internal void LowBalPopup()
    {
        OpenPopup(LBPopup_Object);
    }

    internal void DisconnectionPopup()
    {

        if (!isExit)
        {
            OpenPopup(DisconnectPopup_Object);
        }

    }

    internal void PopulateWin(int value, double amount)
    {
        switch (value)
        {
            case 1:
                if (Win_Image) Win_Image.sprite = BigWin_Sprite;
                break;
            case 2:
                if (Win_Image) Win_Image.sprite = HugeWin_Sprite;
                break;
            case 3:
                if (Win_Image) Win_Image.sprite = MegaWin_Sprite;
                break;
            case 4:
                if (Win_Image) Win_Image.sprite = Jackpot_Sprite;
                break;
        }
        audioController.PlayWLAudio("megaWin");
        StartPopupAnim(amount);
    }

    internal void SetBonusWin(double WinAmount)
    {
        OpenPopup(BonusPopup);
        BonusWinObj.gameObject.SetActive(true);
        BonusWinBG.color = new Color(0, 0, 0, 0);
        BonusWinAmount.text = WinAmount.ToString("f3");
        BonusWinBG.DOColor(new Color(1, 1, 1, 1), 0.3f).OnComplete(() =>
        {

            BonusWinTexts.transform.localScale = new Vector3(0, 1f, 1f);
            BonusWinTexts.SetActive(true);
            BonusWinTexts.transform.DOScaleX(0.8f, 0.4f).SetEase(Ease.OutBack);

        });


    }
    internal void CloseBonusWin()
    {
        BonusWinObj.gameObject.SetActive(false);
        BonusWinTexts.SetActive(false);

    }
    private void StartFreeSpins(int spins)
    {
        if (MainPopup_Object) MainPopup_Object.SetActive(false);
        if (FreeSpinPopup_Object) FreeSpinPopup_Object.SetActive(false);

        slotManager.FreeSpin(spins);
    }

    #region Paytable

    private void OnPaytablePriviousButtonClicked()
    {
        audioController.PlayButtonAudio();
        if (CurrentPaytablePage > 0)
        {
            CurrentPaytablePage--;
        }
        else
        {
            CurrentPaytablePage = PaytablePages.Length - 1;
        }
        foreach (var obj in PaytablePages)
        {
            obj.SetActive(false);
        }
        PaytablePages[CurrentPaytablePage].SetActive(true);
    }

    private void OnPaytableNextButtonClicked()
    {
        audioController.PlayButtonAudio();
        if (CurrentPaytablePage < PaytablePages.Length - 1)
        {
            CurrentPaytablePage++;
        }
        else
        {
            CurrentPaytablePage = 0;
        }
        foreach (var obj in PaytablePages)
        {
            obj.SetActive(false);
        }
        PaytablePages[CurrentPaytablePage].SetActive(true);
    }


    #endregion

    internal void FreeSpinProcess(int spins)
    {
        FreespinWinTxt.text = "0.000";
        FreespinWinAmount = 0;
        PopulateFreeSpinOption();
        int ExtraSpins = spins - FreeSpins;
        FreeSpins = spins;

        if (FreeSpinPopup_Object) FreeSpinPopup_Object.SetActive(true);
        if (Free_Text) Free_Text.text = ExtraSpins.ToString() + " Free spins awarded.";
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        if (FreespinBorder) FreespinBorder.SetActive(true);
        if (NormalBorder) NormalBorder.SetActive(false);

        FreeSpinOptionButton[5].SelectEffectImg.gameObject.SetActive(false);
        if (FreeSpinOptionButton[5].ButtonAnim != null) FreeSpinOptionButton[5].ButtonAnim.Kill();
        transform.localScale = FreeSpinOptionButton[5].originalScale;

        if (slotManager.WasAutoSpinOn)
        {

            FreeSpinOptionButton[5].OnButtonClicked();

        }

    }

    internal IEnumerator ShowFreeSpinStartScreen(int spins, int m1, int m2, int m3)
    {
        NoOFFreeSpintxt.text = spins.ToString();
        Multiplyers.text = m1.ToString() + "x      " + m2.ToString() + "x    " + m3.ToString() + "x";
        OpenPopup(BonusPopup);
        FreeSpinStartupPanel.SetActive(true);
        NoOFFreeSpins = spins;

        if (slotManager.WasAutoSpinOn)
        {

            if (FreeSpinStartupPanel.activeInHierarchy)
            {
                yield return new WaitForSeconds(1.5f);

                ClosePopup(BonusPopup);
                FreeSpinStartupPanel.SetActive(false);
                StartFreeSpins(spins);
            }
        }
    }

    private void OnClickStartFreeSpinBtn()
    {
        if (FreeSpinStartupPanel.activeInHierarchy)
        {
            ClosePopup(BonusPopup);
            FreeSpinStartupPanel.SetActive(false);
            StartFreeSpins(NoOFFreeSpins);
        }
    }
    internal void StartFirstFreeSpin(int spins, int m1, int m2, int m3)
    {
        foreach (var btn in FreeSpinOptionButton)
        {
            btn.SetIntractable(false);
        }
        DOVirtual.DelayedCall(2f, () =>
        {
            StartCoroutine(ShowFreeSpinStartScreen(spins, m1, m2, m3));
        });
    }
    internal void PopulateFreeSpinOption()
    {
        for (int i = 0; i < FreeSpinOptionButton.Length - 1; i++)
        {
            FreeSpinOptionButton[i].SetIntractable(true);
            FreeSpinOptionButton[i].SetData(socketManager.InitFeature.freespinOptions[i].count, socketManager.InitFeature.freespinOptions[i].multiplier[0], socketManager.InitFeature.freespinOptions[i].multiplier[1], socketManager.InitFeature.freespinOptions[i].multiplier[2]);
        }
        FreeSpinOptionButton[5].SetIntractable(true);
        FreeSpinOptionButton[5].SelectEffectImg.gameObject.SetActive(false);
        if (FreeSpinOptionButton[5].ButtonAnim != null) FreeSpinOptionButton[5].ButtonAnim.Kill();
    }
    void SkipWin()
    {
        Debug.Log("Skip win called");
        if (ClosePopupTween != null)
        {
            ClosePopupTween.Kill();
            ClosePopupTween = null;
        }
        if (WinPopupTextTween != null)
        {
            WinPopupTextTween.Kill();
            WinPopupTextTween = null;
        }
        ClosePopup(WinPopup_Object);
        slotManager.CheckPopups = false;
    }

    private void StartPopupAnim(double amount)
    {
        double initAmount = 0;
        if (WinPopup_Object) WinPopup_Object.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        WinPopupTextTween = DOTween.To(() => initAmount, (val) => initAmount = val, amount, 5f).OnUpdate(() =>
        {
            if (Win_Text) Win_Text.text = initAmount.ToString("F3");
        });

        ClosePopupTween = DOVirtual.DelayedCall(3f, () =>
        {
            ClosePopup(WinPopup_Object);
            slotManager.CheckPopups = false;
        });
    }

    internal void ADfunction()
    {
        OpenPopup(ADPopup_Object);
    }

    internal void InitialiseUIData(Paylines symbolsText)
    {

        PopulateSymbolsPayout(symbolsText);
    }

    private void PopulateSymbolsPayout(Paylines paylines)
    {
        for (int i = 0; i < SymbolsText.Length; i++)
        {
            if (i == 0)
            {
                string text = null;
                if (paylines.symbols[0].multiplier[0] != 0)
                {
                    text += "5x - " + paylines.symbols[0].multiplier[0] + "x";
                }
                if (paylines.symbols[0].multiplier[1] != 0)
                {
                    text += "\n4x - " + paylines.symbols[0].multiplier[1] + "x";
                }
                if (paylines.symbols[0].multiplier[2] != 0)
                {
                    text += "\n3x - " + paylines.symbols[0].multiplier[2] + "x";
                }
                if (SymbolsText[0]) SymbolsText[0].text = text;
            }
            else if (i == 1)
            {
                string text = null;
                if (paylines.symbols[4].multiplier[0] != 0)
                {
                    text += "5x - " + paylines.symbols[4].multiplier[0] + "x";
                }
                if (paylines.symbols[4].multiplier[1] != 0)
                {
                    text += "\n4x - " + paylines.symbols[4].multiplier[1] + "x";
                }
                if (paylines.symbols[4].multiplier[2] != 0)
                {
                    text += "\n3x - " + paylines.symbols[4].multiplier[2] + "x";
                }
                if (SymbolsText[1]) SymbolsText[1].text = text;
            }
            else if (i == 5)
            {
                string text = null;
                if (paylines.symbols[10].multiplier[0] != 0)
                {
                    text += "5x - " + paylines.symbols[10].multiplier[0] + "x";
                }
                if (paylines.symbols[10].multiplier[1] != 0)
                {
                    text += "\n4x - " + paylines.symbols[10].multiplier[1] + "x";
                }
                if (paylines.symbols[10].multiplier[2] != 0)
                {
                    text += "\n3x - " + paylines.symbols[10].multiplier[2] + "x";
                }
                if (SymbolsText[5]) SymbolsText[5].text = text;
            }
            else
            {
                string text = null;
                if (paylines.symbols[i + 4].multiplier[0] != 0)
                {
                    text += "5x - " + paylines.symbols[i + 4].multiplier[0] + "x";
                }
                if (paylines.symbols[i + 4].multiplier[1] != 0)
                {
                    text += "\n4x - " + paylines.symbols[i + 4].multiplier[1] + "x";
                }
                if (paylines.symbols[i + 4].multiplier[2] != 0)
                {
                    text += "\n3x - " + paylines.symbols[i + 4].multiplier[2] + "x";
                }
                if (SymbolsText[i]) SymbolsText[i].text = text;
            }
        }

        FreeSpin_Text.text = "3 scattered                     on reels 2, 3, and 4 pays 2x the total bet and awards selection of one of the following: \n\n " +
            $"{socketManager.InitFeature.freespinOptions[0].count} free games with {socketManager.InitFeature.freespinOptions[0].multiplier[0]}X, {socketManager.InitFeature.freespinOptions[0].multiplier[1]}X, or {socketManager.InitFeature.freespinOptions[0].multiplier[2]}X multiplier for all line wins with \n\n" +
            $"{socketManager.InitFeature.freespinOptions[1].count} free games with {socketManager.InitFeature.freespinOptions[1].multiplier[0]}X, {socketManager.InitFeature.freespinOptions[1].multiplier[1]}X, or {socketManager.InitFeature.freespinOptions[1].multiplier[2]}X multiplier for all line wins with \n\n" +
            $"{socketManager.InitFeature.freespinOptions[2].count} free games with {socketManager.InitFeature.freespinOptions[2].multiplier[0]}X, {socketManager.InitFeature.freespinOptions[2].multiplier[1]}X, or {socketManager.InitFeature.freespinOptions[2].multiplier[2]}X multiplier for all line wins with \n\n" +
            $"{socketManager.InitFeature.freespinOptions[3].count} free games with {socketManager.InitFeature.freespinOptions[3].multiplier[0]}X, {socketManager.InitFeature.freespinOptions[3].multiplier[1]}X, or {socketManager.InitFeature.freespinOptions[3].multiplier[2]}X multiplier for all line wins with \n\n" +
            $"{socketManager.InitFeature.freespinOptions[4].count} free games with {socketManager.InitFeature.freespinOptions[4].multiplier[0]}X, {socketManager.InitFeature.freespinOptions[4].multiplier[1]}X, or {socketManager.InitFeature.freespinOptions[4].multiplier[2]}X multiplier for all line wins with \n\n" +
            "Mystery choice of any of the above number of free games with any of the above sets of multipliers \n\n" +
            $"to free games with {socketManager.InitFeature.freespinOptions[0].multiplier[0]}X, {socketManager.InitFeature.freespinOptions[0].multiplier[1]}X, {socketManager.InitFeature.freespinOptions[0].multiplier[2]}X, {socketManager.InitFeature.freespinOptions[3].multiplier[0]}X, {socketManager.InitFeature.freespinOptions[3].multiplier[1]}X, {socketManager.InitFeature.freespinOptions[3].multiplier[2]}X, or {socketManager.InitFeature.freespinOptions[4].multiplier[2]}X multiplier for all line wins with \n\n" +
            "One or more in a winning combination multiplies the pay for that winning combination by a random wild multiplier from the ranges corresponding to the free games scenario selected.An alternate set of reels is used during the Free Games Bonus.Winning combinations for these reels are identical to the base game except 3 scattered               awards 2x the total bet and additional free games equal to the number of free games initially awarded.The multiplier set remains the same.The Free Games Bonus ends when no free games remain.\n\n\n" +
            "and the Fire Link Feature are not available during free games";

        FireLink_Text.text = $"{socketManager.InitFeature.bonusTrigger[0].count[0]} or more scattered         triggers the Fire Link Feature. The feature starts with 4 unlock rows when triggered with 4 to 7             symbols, or {socketManager.InitFeature.bonusTrigger[1].rows} unlocked rows when triggered with {socketManager.InitFeature.bonusTrigger[1].count[0]} to {socketManager.InitFeature.bonusTrigger[1].count[1]} All unlocked rows when triggered with symbols. 3 spins are awarded.            symbols lock in place for the duration of the feature and remaining symbol positions spin independently.\n\n" +
            "If a new appears, the symbol locks in place for the duration of the feature.The number of spins remaining resets to 3 with each newly locked that appears on an unlocked row.Collecting the indicated number of additional in unlocked rows unlocks that additional row.A maximum of 8 rows is available.\n\n" +
            "symbols award credit values " +
            "Credit values range from 50 to 5000 times the bet per line.Credit values displayed on the        symbols have already been multiplied by the bet per line.Only       symbols contribute to wins.The chance of winning a jackpot increases with higher bets.Jackpots will be paid as displayed at the end of the feature.When no spins remain or when all 40 positions are locked, the award displayed on each in an unlocked row is awarded and the feature ends.An alternate set of reels is used during the Fire Link Feature.\n\n" +
            "is not available during the Fire Link Feature.appears only on two reels per feature.";
    }

    private void CallOnExitFunction()
    {
        isExit = true;
        audioController.PlayButtonAudio();
        slotManager.CallCloseSocket();
    }

    private void OpenMenu()
    {
        audioController.PlayButtonAudio();
        if (Menu_Object) Menu_Object.SetActive(false);
        if (Exit_Object) Exit_Object.SetActive(true);
        //if (About_Object) About_Object.SetActive(true);
        if (Paytable_Object) Paytable_Object.SetActive(true);
        if (Settings_Object) Settings_Object.SetActive(true);

        //DOTween.To(() => About_RT.anchoredPosition, (val) => About_RT.anchoredPosition = val, new Vector2(About_RT.anchoredPosition.x, About_RT.anchoredPosition.y + 150), 0.1f).OnUpdate(() =>
        //{
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(About_RT);
        //});

        DOTween.To(() => Paytable_RT.anchoredPosition, (val) => Paytable_RT.anchoredPosition = val, new Vector2(Paytable_RT.anchoredPosition.x, Paytable_RT.anchoredPosition.y + 125), 0.1f).OnUpdate(() =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Paytable_RT);
        });

        DOTween.To(() => Settings_RT.anchoredPosition, (val) => Settings_RT.anchoredPosition = val, new Vector2(Settings_RT.anchoredPosition.x, Settings_RT.anchoredPosition.y + 250), 0.1f).OnUpdate(() =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Settings_RT);
        });
    }

    private void CloseMenu()
    {

        if (audioController) audioController.PlayButtonAudio();
        //DOTween.To(() => About_RT.anchoredPosition, (val) => About_RT.anchoredPosition = val, new Vector2(About_RT.anchoredPosition.x, About_RT.anchoredPosition.y - 150), 0.1f).OnUpdate(() =>
        //{
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(About_RT);
        //});

        DOTween.To(() => Paytable_RT.anchoredPosition, (val) => Paytable_RT.anchoredPosition = val, new Vector2(Paytable_RT.anchoredPosition.x, Paytable_RT.anchoredPosition.y - 125), 0.1f).OnUpdate(() =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Paytable_RT);
        });

        DOTween.To(() => Settings_RT.anchoredPosition, (val) => Settings_RT.anchoredPosition = val, new Vector2(Settings_RT.anchoredPosition.x, Settings_RT.anchoredPosition.y - 250), 0.1f).OnUpdate(() =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Settings_RT);
        });

        DOVirtual.DelayedCall(0.1f, () =>
         {
             if (Menu_Object) Menu_Object.SetActive(true);
             if (Exit_Object) Exit_Object.SetActive(false);
             //if (About_Object) About_Object.SetActive(false);
             if (Paytable_Object) Paytable_Object.SetActive(false);
             if (Settings_Object) Settings_Object.SetActive(false);
         });
    }

    internal void OpenPopup(GameObject Popup)
    {
        //if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }
    internal void CheckAndClosePopups()
    {

        if (ReconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(ReconnectPopup_Object);
        }
        if (DisconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(DisconnectPopup_Object);
        }
    }
    internal void ReconnectionPopup()
    {
        OpenPopup(ReconnectPopup_Object);
    }


    internal void ClosePopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();

        if (Popup) Popup.SetActive(false);
        if (!DisconnectPopup_Object.activeSelf)
        {
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
        }
    }


    private void ToggleMusic()
    {
        if (audioController) audioController.PlayButtonAudio();
        if (audioController) audioController.ReleaseFocusMute();
        isMusicMuted = !isMusicMuted;
        ApplyAudioState();
    }

    private void UrlButtons(string url)
    {
        Application.OpenURL(url);
    }

    private void ToggleSound()
    {
        if (audioController) audioController.PlayButtonAudio();
        if (audioController) audioController.ReleaseFocusMute();
        isSoundMuted = !isSoundMuted;
        ApplyAudioState();
    }

    internal IEnumerator SlideChangeAnimation(Action changeScreen)
    {
        OpenPopup(SlideChangePopup);

        SlideChangeObj.transform.position = downPos.position;
        Tween moveTween = SlideChangeObj.transform.DOMove(centerPos.position, 1.5f).SetEase(Ease.Linear);
        yield return moveTween.WaitForCompletion();


        changeScreen?.Invoke();


        moveTween = SlideChangeObj.transform.DOMove(upPos.position, 1.5f).SetEase(Ease.Linear);
        yield return moveTween.WaitForCompletion();

        ClosePopup(SlideChangePopup);
        yield return new WaitForSeconds(0.3f);
    }
}
