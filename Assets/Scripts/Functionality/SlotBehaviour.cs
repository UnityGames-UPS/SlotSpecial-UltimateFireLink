using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotBehaviour : MonoBehaviour
{

  [Header("Sprites")]
  [SerializeField]
  internal Sprite[] myImages;  //images taken initially

  [Header("Slot Images")]

  [SerializeField]
  private List<SlotImage> images;     //class to store total images
  [SerializeField]
  private List<SlotImage> Tempimages;     //class to store the result matrix
  [SerializeField]
  private List<Slottext> TempText;

  [Header("Slots Elements")]
  [SerializeField]
  private LayoutElement[] Slot_Elements;
  [SerializeField] private GameObject MaskOverRideParrent;

  [Header("Slots Transforms")]
  [SerializeField]
  private Transform[] Slot_Transform;

  [Header("Line Button Objects")]
  [SerializeField]
  private List<GameObject> StaticLine_Objects;

  [Header("Line Button Texts")]
  [SerializeField]
  private List<TMP_Text> StaticLine_Texts;

  private Dictionary<int, string> y_string = new Dictionary<int, string>();

  [Header("Buttons")]
  [SerializeField]
  private Button SlotStart_Button;
  [SerializeField]
  private Button Mobile_SlotStart_Button;
  [SerializeField] private Button P_AutoSpinStop_Button;
  [SerializeField]
  internal Button TBetPlus_Button;
  [SerializeField]
  internal Button TBetMinus_Button;
  [SerializeField] private Button Turbo_On_Button;        // landscape/PC, hidden by default
  [SerializeField] private Button Turbo_Off_Button;       // landscape/PC, shown by default
  [SerializeField] private Button Mobile_Turbo_On_Button; // portrait, hidden by default
  [SerializeField] private Button Mobile_Turbo_Off_Button;// portrait, shown by default
  [SerializeField] private Button StopSpin_Button;
  [SerializeField] private Button Mobile_StopSpin_Button;
  [SerializeField] private Button M_AutoSpinStop_Button;

  [Header("Animated Sprites")]
  [SerializeField]
  internal Sprite[] Sun_Sprite;
  [SerializeField]
  private Sprite[] FreeSpin_Sprite;
  [SerializeField]
  private Sprite[] Nine_Sprite;
  [SerializeField]
  private Sprite[] A_Sprite;
  [SerializeField]
  private Sprite[] Q_Sprite;
  [SerializeField]
  private Sprite[] k_Sprite;
  [SerializeField]
  private Sprite[] J_Sprite;
  [SerializeField]
  private Sprite[] Ten_Sprite;
  [SerializeField]
  private Sprite[] FortuneCake_Sprite;
  [SerializeField]
  private Sprite[] Dimpsum_Sprite;
  [SerializeField]
  private Sprite[] Laltern_Sprite;
  [SerializeField]
  private Sprite[] kattle_Sprite;


  [Header("Jackpot Labels")]
  [SerializeField]
  internal Sprite[] JackpotSprites;   // mini/minor/major/mega label images, ordered to match InitFeature.jackpotMultipliers
  [SerializeField]
  private float ScatterSymbolScale = 1.8f;   // scale applied to a scatter symbol when its value is shown

  [Header("Miscellaneous UI")]
  [SerializeField]
  internal TMP_Text Balance_text;
  [SerializeField]
  internal TMP_Text TotalBet_text;
  [SerializeField]
  internal TMP_Text LineBet_text;
  [SerializeField]
  internal TMP_Text TotalWin_text;
  [SerializeField] internal TMP_Text[] MiniMajorMinor;

  [Header("Audio Management")]
  [SerializeField]
  private AudioController audioController;

  [SerializeField]
  private UIManager uiManager;

  [Header("BonusGame Popup")]
  [SerializeField]
  private BonusController _bonusManager;

  [Header("Free Spins Board")]
  [SerializeField]
  private GameObject FSBoard_Object;
  [SerializeField]
  private TMP_Text FSnum_text;

  int tweenHeight = 0;  //calculate the height at which tweening is done

  [SerializeField]
  private GameObject Image_Prefab;    //icons prefab

  private List<Tweener> alltweens = new List<Tweener>();

  private Tweener WinTween = null;

  [SerializeField]
  private List<ImageAnimation> TempList;  //stores the sprites whose animation is running at present 

  [SerializeField]
  private SocketIOManager SocketManager;

  private Dictionary<Transform, (Transform parent, int siblingIndex)> originalData = new Dictionary<Transform, (Transform, int)>();
  private Coroutine AutoSpinRoutine = null;
  private Coroutine FreeSpinRoutine = null;
  private Coroutine tweenroutine;
  private Tween BalanceTween;
  internal bool IsAutoSpin = false;
  internal bool IsFreeSpin = false;
  private bool InsideFreeSpin = false;
  private bool IsSpinning = false;
  internal bool CheckPopups = false;
  internal int BetCounter = 0;
  private double currentBalance = 0;
  private double currentTotalBet = 0;
  protected int Lines = 50;
  [SerializeField]
  private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing
  private int numberOfSlots = 5;          //number of columns
  private bool StopSpinToggle;
  private float SpinDelay = 0.2f;
  [SerializeField] private float AutoSpinHoldDuration = 1.5f; // seconds to hold spin button before auto-spin starts
  internal bool IsTurboOn;
  internal bool WasAutoSpinOn;
  private Coroutine _holdRoutine;
  private bool _holdConsumed; // true once a hold has started auto-spin, so release won't also single-spin
  private int priviousButtonIndex;
  private Coroutine LogoAnim;

  private void Start()
  {
    IsAutoSpin = false;

    assignbuttons();
    ApplyTurboState(false);   // default: turbo off — show Off buttons, hide On buttons (no click audio)
    if (FSBoard_Object) FSBoard_Object.SetActive(false);
    OffsetSymbolTexts();

    tweenHeight = (15 * IconSizeFactor) - 280;
  }

  // Nudge every symbol's value-text child down by 7px. Done once here (not per spin) so the
  // offset doesn't accumulate.
  private void OffsetSymbolTexts()
  {
    foreach (var temp in Tempimages)
    {
      foreach (var sym in temp.slotImages)
      {
        if (sym && sym.transform.childCount > 0)
        {
          Transform t = sym.transform.GetChild(0);
          Vector3 p = t.localPosition;
          p.y -= 7f;
          t.localPosition = p;
        }
      }
    }
  }

  internal void assignbuttons()
  {
    // Spin buttons are driven by AutoSpinManager pointer events (tap = single spin, hold = auto-spin),
    // not onClick — clear any leftover onClick listeners so a release can't double-fire a spin.
    if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
    if (Mobile_SlotStart_Button) Mobile_SlotStart_Button.onClick.RemoveAllListeners();

    if (TBetPlus_Button) TBetPlus_Button.onClick.RemoveAllListeners();
    if (TBetPlus_Button) TBetPlus_Button.onClick.AddListener(delegate { ChangeBet(true); });

    if (TBetMinus_Button) TBetMinus_Button.onClick.RemoveAllListeners();
    if (TBetMinus_Button) TBetMinus_Button.onClick.AddListener(delegate { ChangeBet(false); });

    if (StopSpin_Button) StopSpin_Button.onClick.RemoveAllListeners();
    if (StopSpin_Button) StopSpin_Button.onClick.AddListener(() => { audioController.PlayButtonAudio(); StopSpinToggle = true; StopSpin_Button.gameObject.SetActive(false); });

    if (Mobile_StopSpin_Button) Mobile_StopSpin_Button.onClick.RemoveAllListeners();
    if (Mobile_StopSpin_Button) Mobile_StopSpin_Button.onClick.AddListener(() => { audioController.PlayButtonAudio(); StopSpinToggle = true; Mobile_StopSpin_Button.gameObject.SetActive(false); });


    if (Turbo_Off_Button) { Turbo_Off_Button.onClick.RemoveAllListeners(); Turbo_Off_Button.onClick.AddListener(() => SetTurbo(true)); }
    if (Turbo_On_Button) { Turbo_On_Button.onClick.RemoveAllListeners(); Turbo_On_Button.onClick.AddListener(() => SetTurbo(false)); }
    if (Mobile_Turbo_Off_Button) { Mobile_Turbo_Off_Button.onClick.RemoveAllListeners(); Mobile_Turbo_Off_Button.onClick.AddListener(() => SetTurbo(true)); }
    if (Mobile_Turbo_On_Button) { Mobile_Turbo_On_Button.onClick.RemoveAllListeners(); Mobile_Turbo_On_Button.onClick.AddListener(() => SetTurbo(false)); }

    if (P_AutoSpinStop_Button) P_AutoSpinStop_Button.onClick.RemoveAllListeners();
    if (P_AutoSpinStop_Button) P_AutoSpinStop_Button.onClick.AddListener(() => { StopAutoSpin(); WasAutoSpinOn = false; });

    if (M_AutoSpinStop_Button) M_AutoSpinStop_Button.onClick.RemoveAllListeners();
    if (M_AutoSpinStop_Button) M_AutoSpinStop_Button.onClick.AddListener(() => { StopAutoSpin(); WasAutoSpinOn = false; });


  }
  void SetTurbo(bool on)
  {
    audioController.PlayButtonAudio();
    ApplyTurboState(on);
  }

  // Toggles turbo state and swaps On/Off button GameObjects for both orientations.
  void ApplyTurboState(bool on)
  {
    IsTurboOn = on;
    // On buttons visible when turbo is ON; Off buttons visible when turbo is OFF.
    if (Turbo_On_Button) Turbo_On_Button.gameObject.SetActive(on);
    if (Turbo_Off_Button) Turbo_Off_Button.gameObject.SetActive(!on);
    if (Mobile_Turbo_On_Button) Mobile_Turbo_On_Button.gameObject.SetActive(on);
    if (Mobile_Turbo_Off_Button) Mobile_Turbo_Off_Button.gameObject.SetActive(!on);
  }


  #region Hold Button To Start Auto Spin
  // The Spin button drives both single-spin (tap) and auto-spin (hold). AutoSpinManager (on each
  // Spin button, PC + mobile) forwards the button's pointer events here; SlotBehaviour owns the hold
  // timer so both orientations share one flow. A quick tap runs one spin; holding for
  // AutoSpinHoldDuration starts auto-spin.

  internal void OnSpinPointerDown(Button spinButton)
  {
    // Pointer events ignore Button.interactable, so guard manually: no reaction while a spin or
    // auto-spin is running, or while the button is disabled.
    if (spinButton == null || !spinButton.interactable || IsSpinning || IsAutoSpin) return;

    _holdConsumed = false;
    if (_holdRoutine != null) StopCoroutine(_holdRoutine);
    _holdRoutine = StartCoroutine(SpinHoldRoutine(spinButton));
  }

  internal void OnSpinPointerUp(Button spinButton)
  {
    CancelHold();
    // If the hold already started auto-spin, skip so the release doesn't also fire a spin.
    if (_holdConsumed) return;
    // Pointer events ignore Button.interactable, so re-entry guard manually: never launch a spin on
    // top of a running spin / auto-spin (that starts a second reel tween and corrupts the reels).
    // StartSlots() disables the spin button synchronously, so !interactable also covers the case
    // where IsSpinning hasn't flipped true yet.
    if (spinButton == null || !spinButton.interactable || IsSpinning || IsAutoSpin) return;
    StartSlots();
  }

  // Dragging off the button cancels the pending hold so a release elsewhere can't start auto-spin.
  internal void OnSpinPointerExit()
  {
    CancelHold();
  }

  private IEnumerator SpinHoldRoutine(Button spinButton)
  {
    yield return new WaitForSeconds(AutoSpinHoldDuration);
    _holdRoutine = null;

    if (spinButton == null || !spinButton.interactable || IsSpinning || IsAutoSpin) yield break;

    _holdConsumed = true; // release will skip the single spin
    AutoSpin();
  }

  private void CancelHold()
  {
    if (_holdRoutine != null)
    {
      StopCoroutine(_holdRoutine);
      _holdRoutine = null;
    }
  }
  #endregion

  #region Autospin
  private void AutoSpin()
  {
    if (!IsAutoSpin)
    {

      IsAutoSpin = true;
      WasAutoSpinOn = true;
      if (P_AutoSpinStop_Button) P_AutoSpinStop_Button.gameObject.SetActive(true);
      if (M_AutoSpinStop_Button) M_AutoSpinStop_Button.gameObject.SetActive(true);

      if (AutoSpinRoutine != null)
      {
        StopCoroutine(AutoSpinRoutine);
        AutoSpinRoutine = null;
      }
      AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());

    }
  }

  private void StopAutoSpin()
  {
    audioController.PlayButtonAudio();
    if (IsAutoSpin)
    {
      IsAutoSpin = false;

      if (P_AutoSpinStop_Button) P_AutoSpinStop_Button.gameObject.SetActive(false);
      if (M_AutoSpinStop_Button) M_AutoSpinStop_Button.gameObject.SetActive(false);
      StartCoroutine(StopAutoSpinCoroutine());
    }
  }

  private IEnumerator AutoSpinCoroutine()
  {
    while (IsAutoSpin)
    {
      StartSlots(IsAutoSpin);
      yield return tweenroutine;
      yield return new WaitForSeconds(SpinDelay);
    }
    WasAutoSpinOn = false;
  }

  private IEnumerator StopAutoSpinCoroutine()
  {
    yield return new WaitUntil(() => !IsSpinning);
    ToggleButtonGrp(true);
    // WasAutoSpinOn = false;
    if (AutoSpinRoutine != null || tweenroutine != null)
    {
      StopCoroutine(AutoSpinRoutine);
      StopCoroutine(tweenroutine);
      tweenroutine = null;
      AutoSpinRoutine = null;
      StopCoroutine(StopAutoSpinCoroutine());
    }
  }
  #endregion

  #region FreeSpin
  internal void FreeSpin(int spins)
  {
    if (!IsFreeSpin)
    {
      if (FSBoard_Object) FSBoard_Object.SetActive(true);
      if (SocketManager.ResultData.features.freeSpin.count >= 0) FSnum_text.text = uiManager.FreeSpinOptionButton[priviousButtonIndex].SpinNumber.ToString();
      IsFreeSpin = true;
      ToggleButtonGrp(false);

      if (FreeSpinRoutine != null)
      {
        StopCoroutine(FreeSpinRoutine);
        FreeSpinRoutine = null;
      }
      FreeSpinRoutine = StartCoroutine(FreeSpinCoroutine(spins));
    }
  }

  private IEnumerator FreeSpinCoroutine(int spinchances)
  {
    int i = 0;
    while (SocketManager.ResultData.features.freeSpin.count > 0)
    {
      uiManager.FreeSpins--;

      StartSlots();
      yield return tweenroutine;
      yield return new WaitForSeconds(SpinDelay);
      if (SocketManager.ResultData.features.freeSpin.count >= 0) FSnum_text.text = SocketManager.ResultData.features.freeSpin.count.ToString();
      i++;
    }


    if (uiManager.FreespinWinAmount > 0)
    {
      uiManager.SetBonusWin(uiManager.FreespinWinAmount);
      yield return new WaitForSeconds(2f);
    }


    yield return (uiManager.SlideChangeAnimation(() =>
    {
      uiManager.CloseBonusWin();
      if (uiManager.FreespinBorder) uiManager.FreespinBorder.SetActive(false);
      if (uiManager.NormalBorder) uiManager.NormalBorder.SetActive(true);
      if (FSBoard_Object) FSBoard_Object.SetActive(false);

    }));
    uiManager.ClosePopup(uiManager.BonusPopup);
    IsFreeSpin = false;

    if (WasAutoSpinOn)
    {
      AutoSpin();
    }
    else
    {
      ToggleButtonGrp(true);
    }
  }
  #endregion

  /// <summary>
  /// Snaps the balance to a value pushed by the backend outside of any spin flow
  /// (see SocketIOManager.OnBalanceSync). Not tweened -- an external correction is not
  /// a win animation -- and re-runs the low-balance gate, since the push can move the
  /// player across the "can spin" threshold in either direction.
  /// </summary>
  internal void UpdateBalanceDisplay(double newBalance)
  {
    BalanceTween?.Kill();
    currentBalance = newBalance;
    if (Balance_text) Balance_text.text = newBalance.ToString("F3");
    CompareBalance();
  }

  private void CompareBalance()
  {
    if (SocketManager.PlayerData.balance < currentTotalBet)
    {
      uiManager.LowBalPopup();
    }
  }

  private void ChangeBet(bool IncDec)
  {
    if (audioController) audioController.PlayButtonAudio();
    if (IncDec)
    {
      BetCounter++;
      if (BetCounter >= SocketManager.InitialData.bets.Count)
      {
        BetCounter = 0; // Loop back to the first bet
      }
    }
    else
    {
      BetCounter--;
      if (BetCounter < 0)
      {
        BetCounter = SocketManager.InitialData.bets.Count - 1; // Loop to the last bet
      }
    }
    if (LineBet_text) LineBet_text.text = SocketManager.InitialData.bets[BetCounter].ToString();
    if (TotalBet_text) TotalBet_text.text = (SocketManager.InitialData.bets[BetCounter] * Lines).ToString();
    for (int i = 0; i < MiniMajorMinor.Length; i++)
    {
      MiniMajorMinor[i].text = (SocketManager.InitialData.bets[BetCounter] * SocketManager.InitFeature.jackpotMultipliers[i]).ToString();
    }
    currentTotalBet = SocketManager.InitialData.bets[BetCounter] * Lines;
    //CompareBalance();
  }

  #region InitialFunctions
  internal void shuffleInitialMatrix()
  {
    for (int i = 0; i < Tempimages.Count; i++)
    {
      for (int j = 0; j < 5; j++)
      {
        int randomIndex = UnityEngine.Random.Range(0, 7);
        Tempimages[i].slotImages[j].sprite = myImages[randomIndex];
      }
    }
  }

  /// <summary>
  /// Repopulates the currently-bound texts from live state. Must be called after an
  /// orientation switch rebinds Balance_text/TotalBet_text/TotalWin_text to the other
  /// orientation's widgets, which still hold whatever the scene authored. This is not
  /// cosmetic: BalanceDeduction() parses TotalBet_text/Balance_text back out as the
  /// source of truth, so a stale widget corrupts the next spin's arithmetic.
  /// Unlike SetInitialUI() this does NOT reset BetCounter.
  /// </summary>
  internal void RefreshBoundTexts()
  {
    if (SocketManager == null || SocketManager.InitialData == null || SocketManager.InitialData.bets == null || SocketManager.InitialData.bets.Count == 0)
      return;

    BetCounter = Mathf.Clamp(BetCounter, 0, SocketManager.InitialData.bets.Count - 1);

    if (LineBet_text) LineBet_text.text = SocketManager.InitialData.bets[BetCounter].ToString();
    if (TotalBet_text) TotalBet_text.text = (SocketManager.InitialData.bets[BetCounter] * Lines).ToString();

    if (TotalWin_text)
    {
      double win = 0;
      if (SocketManager.ResultData != null && SocketManager.ResultData.payload != null)
        win = SocketManager.ResultData.payload.winAmount;
      TotalWin_text.text = win.ToString("F3");
    }

    if (Balance_text && SocketManager.PlayerData != null)
      Balance_text.text = SocketManager.PlayerData.balance.ToString("F3");

    if (SocketManager.InitFeature != null && SocketManager.InitFeature.jackpotMultipliers != null)
    {
      for (int i = 0; i < MiniMajorMinor.Length; i++)
      {
        if (MiniMajorMinor[i] && i < SocketManager.InitFeature.jackpotMultipliers.Count)
          MiniMajorMinor[i].text = (SocketManager.InitialData.bets[BetCounter] * SocketManager.InitFeature.jackpotMultipliers[i]).ToString();
      }
    }
  }

  internal void SetInitialUI()
  {
    BetCounter = 0;
    if (LineBet_text) LineBet_text.text = SocketManager.InitialData.bets[BetCounter].ToString();
    if (TotalBet_text) TotalBet_text.text = (SocketManager.InitialData.bets[BetCounter] * Lines).ToString();
    if (TotalWin_text) TotalWin_text.text = "0.000";
    if (Balance_text) Balance_text.text = SocketManager.PlayerData.balance.ToString("F3");
    for (int i = 0; i < MiniMajorMinor.Length; i++)
    {
      MiniMajorMinor[i].text = (SocketManager.InitialData.bets[BetCounter] * SocketManager.InitFeature.jackpotMultipliers[i]).ToString();
    }

    currentBalance = SocketManager.PlayerData.balance;
    currentTotalBet = SocketManager.InitialData.bets[BetCounter] * Lines;
    //_bonusManager.PopulateWheel(SocketManager.bonusdata);                                                               //        change here for bonusController
    CompareBalance();
    uiManager.InitialiseUIData(SocketManager.UIData.paylines);
  }
  #endregion

  // Native focus path. Audio only -- it drives the same mute method the JS path uses,
  // but never the socket timeout: OnApplicationFocus is unreliable inside a WebView and
  // a spurious signal there would close a live session.
  private void OnApplicationFocus(bool focus)
  {
    if (audioController) audioController.SetMuteAll(!focus);
  }

  // Driven from JS via JSFunctCalls.OnFocusChanged on WebGL, where
  // OnApplicationFocus alone does not cover tab visibility changes. This is the only
  // signal trusted to arm/disarm the background socket timeout.
  internal void HandleFocus(bool focus)
  {
    if (audioController) audioController.SetMuteAll(!focus);
    if (SocketManager) SocketManager.HandleFocusChange(focus);
  }

  //function to populate animation sprites accordingly
  private void PopulateAnimationSprites(ImageAnimation animScript, int val)
  {

    // if (animScript == null) return;
    animScript.textureArray.Clear();
    animScript.textureArray.TrimExcess();
    switch (val)
    {
      case 0:
        for (int i = 0; i < A_Sprite.Length; i++)
        {
          animScript.textureArray.Add(A_Sprite[i]);
        }
        animScript.AnimationSpeed = 28f;
        break;
      case 1:
        for (int i = 0; i < k_Sprite.Length; i++)
        {
          animScript.textureArray.Add(k_Sprite[i]);
        }
        animScript.AnimationSpeed = 28f;
        break;
      case 2:
        for (int i = 0; i < Q_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Q_Sprite[i]);
        }
        animScript.AnimationSpeed = 28f;
        break;

      case 3:
        for (int i = 0; i < J_Sprite.Length; i++)
        {
          animScript.textureArray.Add(J_Sprite[i]);
        }
        animScript.AnimationSpeed = 28f;
        break;
      case 4:
        for (int i = 0; i < Ten_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Ten_Sprite[i]);
        }
        animScript.AnimationSpeed = 28f;
        break;
      case 5:
        for (int i = 0; i < Nine_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Nine_Sprite[i]);
        }
        animScript.AnimationSpeed = 28f;
        break;
      case 6:
        for (int i = 0; i < Dimpsum_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Dimpsum_Sprite[i]);
        }
        animScript.AnimationSpeed = 28f;
        break;
      case 7:
        for (int i = 0; i < FortuneCake_Sprite.Length; i++)
        {
          animScript.textureArray.Add(FortuneCake_Sprite[i]);
        }
        animScript.AnimationSpeed = 28f;
        break;
      case 8:
        for (int i = 0; i < kattle_Sprite.Length; i++)
        {
          animScript.textureArray.Add(kattle_Sprite[i]);
        }
        animScript.AnimationSpeed = 28f;
        break;
      case 9:
        for (int i = 0; i < kattle_Sprite.Length; i++)
        {
          animScript.textureArray.Add(kattle_Sprite[i]);
        }
        animScript.AnimationSpeed = 28f;
        break;
      case 10:
        for (int i = 0; i < Laltern_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Laltern_Sprite[i]);
        }
        animScript.AnimationSpeed = 28f;
        break;
      case 11:
        for (int i = 0; i < Sun_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Sun_Sprite[i]);
        }
        animScript.AnimationSpeed = 50f;
        break;
      case 12:
        for (int i = 0; i < FreeSpin_Sprite.Length; i++)
        {
          animScript.textureArray.Add(FreeSpin_Sprite[i]);
        }
        animScript.AnimationSpeed = 75f;
        break;

    }
  }

  #region SlotSpin
  //starts the spin process
  internal void StartSlots(bool autoSpin = false)
  {

    if (audioController) audioController.PlaySpinButtonAudio();


    if (!autoSpin)
    {
      if (AutoSpinRoutine != null)
      {
        StopCoroutine(AutoSpinRoutine);
        StopCoroutine(tweenroutine);
        tweenroutine = null;
        AutoSpinRoutine = null;
      }
    }

    if (SlotAnimRoutine != null)
    {
      StopCoroutine(SlotAnimRoutine);
      SlotAnimRoutine = null;
    }
    WinningsAnim(false);
    if (SlotStart_Button) SlotStart_Button.interactable = false;
    if (Mobile_SlotStart_Button) Mobile_SlotStart_Button.interactable = false;
    if (TempList.Count > 0)
    {
      StopGameAnimation();
    }

    tweenroutine = StartCoroutine(TweenRoutine());
  }

  //manage the Routine for spinning of the slots
  private IEnumerator TweenRoutine()
  {
    if (LogoAnim != null)
    {
      StopCoroutine(LogoAnim);
      LogoAnim = null;
    }
    if (currentBalance < currentTotalBet && !IsFreeSpin)
    {
      CompareBalance();
      StopAutoSpin();
      yield return new WaitForSeconds(1);
      ToggleButtonGrp(true);
      yield break;
    }

    if (TotalWin_text) TotalWin_text.text = "0.000";
    // if (audioController) audioController.PlayWLAudio("spin");

    IsSpinning = true;
    RestoreObjectsToOriginalParents();
    ToggleButtonGrp(false);

    TempList.Clear();
    TempList.TrimExcess();

    if (!IsTurboOn && !IsFreeSpin && !IsAutoSpin)
    {
      StopSpin_Button.gameObject.SetActive(true);
      Mobile_StopSpin_Button.gameObject.SetActive(true);
    }
    for (int i = 0; i < numberOfSlots; i++)
    {
      InitializeTweening(Slot_Transform[i]);
      yield return new WaitForSeconds(0.1f);
    }
    if (!IsFreeSpin)
    {
      BalanceDeduction();
    }


    SocketManager.AccumulateResult("SPIN");

    yield return new WaitUntil(() => SocketManager.isResultdone);

    ResetSlotText();
    for (int j = 0; j < SocketManager.ResultData.matrix.Count; j++)
    {
      List<int> resultnum = SocketManager.ConvertListStringToListInt(SocketManager.ResultData.matrix[j]);
      for (int i = 0; i < 5; i++)
      {
        if (j < 4)
        {
          if (Tempimages[j].slotImages[i]) Tempimages[j].slotImages[i].sprite = myImages[resultnum[i]];
          PopulateAnimationSprites(Tempimages[j].slotImages[i].gameObject.GetComponent<ImageAnimation>(), resultnum[i]);
        }
      }
    }

    if (IsTurboOn)
    {


      StopSpinToggle = true;
    }
    else
    {
      for (int i = 0; i < 5; i++)
      {
        yield return new WaitForSeconds(0.1f);
        if (StopSpinToggle)
        {
          break;
        }
      }
      StopSpin_Button.gameObject.SetActive(false);
      Mobile_StopSpin_Button.gameObject.SetActive(false);
    }

    for (int i = 0; i < numberOfSlots; i++)
    {
      yield return StopTweening(5, Slot_Transform[i], i, StopSpinToggle);
    }
    StopSpinToggle = false;

    yield return alltweens[^1].WaitForCompletion();
    KillAllTweens();
    if (SocketManager.ResultData.features.bonus.scatterValues.Count > 0)
    {
      for (int i = 0; i < SocketManager.ResultData.features.bonus.scatterValues.Count; i++)
      {
        MoveObjectsToNewParent(MaskOverRideParrent, Tempimages[SocketManager.ResultData.features.bonus.scatterValues[i].index[0]].slotImages[SocketManager.ResultData.features.bonus.scatterValues[i].index[1]].gameObject);
      }
    }
    if (SocketManager.ResultData.payload.winAmount > 0)
    {
      SpinDelay = 1.2f;
    }
    else
    {
      SpinDelay = 0.2f;
    }

    if (TotalWin_text) TotalWin_text.text = SocketManager.ResultData.payload.winAmount.ToString("F3");
    BalanceTween?.Kill();
    if (Balance_text) Balance_text.text = SocketManager.PlayerData.balance.ToString("F3");

    currentBalance = SocketManager.PlayerData.balance;

    List<int> winLine = new();
    foreach (var item in SocketManager.ResultData.payload.wins)
    {
      if (audioController) audioController.PlayWLAudio("win");
      winLine.Add(item.line);
    }

    if (IsAutoSpin)
    {
      yield return CheckPayoutLineBackend(winLine);
    }
    else
    {

      LogoAnim = StartCoroutine(CheckPayoutLineBackend(winLine));
    }

    CheckPopups = true;

    if (IsFreeSpin)
    {
      uiManager.FreespinWinAmount += SocketManager.ResultData.payload.winAmount;
      uiManager.FreespinWinTxt.text = uiManager.FreespinWinAmount.ToString("F3");
    }

    if (SocketManager.ResultData.features.bonus.isTriggered)
    {
      yield return new WaitForSeconds(1f);
      CheckBonusGame();
    }
    else
    {
      if (!IsFreeSpin)
      {
        CheckWinPopups();
      }
      else
      {
        CheckPopups = false;
      }
    }

    yield return new WaitUntil(() => !CheckPopups);

    if (SocketManager.ResultData.features.freeSpin.isFreeSpin)
    {

      if (IsFreeSpin)
      {
        IsFreeSpin = false;
        if (FreeSpinRoutine != null)
        {
          StopCoroutine(FreeSpinRoutine);
          FreeSpinRoutine = null;
        }
        yield return new WaitForSeconds(2f);
        InsideFreeSpin = true;
        yield return uiManager.ShowFreeSpinStartScreen((int)SocketManager.ResultData.features.freeSpin.count, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer1, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer2, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer3);
      }
      yield return new WaitForSeconds(0.6f);
      if (IsAutoSpin)
      {
        WasAutoSpinOn = true;
      }
      if (!InsideFreeSpin) uiManager.FreeSpinProcess((int)SocketManager.ResultData.features.freeSpin.count);
      InsideFreeSpin = false;
      if (IsAutoSpin)
      {

        StopAutoSpin();
        yield return new WaitForSeconds(0.1f);
      }
    }

    if (!IsAutoSpin && !IsFreeSpin)
    {
      ToggleButtonGrp(true);
      IsSpinning = false;
    }
    else
    {
      IsSpinning = false;
    }

  }

  private void BalanceDeduction()
  {
    double bet = 0;
    double balance = 0;
    try
    {
      bet = double.Parse(TotalBet_text.text);
    }
    catch (Exception e)
    {
      Debug.Log("Error while conversion " + e.Message);
    }

    try
    {
      balance = double.Parse(Balance_text.text);
    }
    catch (Exception e)
    {
      Debug.Log("Error while conversion " + e.Message);
    }
    double initAmount = balance;

    balance = balance - bet;

    BalanceTween = DOTween.To(() => initAmount, (val) => initAmount = val, balance, 0.8f).OnUpdate(() =>
    {
      if (Balance_text) Balance_text.text = initAmount.ToString("F3");
    });
  }

  internal void CheckWinPopups()
  {
    //  Debug.Log("dev_test" + SocketManager.ResultData.payload.winAmount+ "   0   " + currentTotalBet);
    if (SocketManager.ResultData.payload.winAmount >= currentTotalBet * 10)
    {
      uiManager.PopulateWin(1, SocketManager.ResultData.payload.winAmount);
      //Debug.Log("dev_test" + "0");
    }
    //else if (SocketManager.ResultData.WinAmout >= currentTotalBet * 15 &&SocketManager.ResultData.payload.winAmount < currentTotalBet * 20)
    //{
    //    uiManager.PopulateWin(2,SocketManager.ResultData.payload.winAmount);
    //    Debug.Log("dev_test" + "1");
    //}
    //else if (SocketManager.ResultData.WinAmout >= currentTotalBet * 20)
    //{
    //    uiManager.PopulateWin(3,SocketManager.ResultData.payload.winAmount);
    //    Debug.Log("dev_test" + "2");
    //}
    else
    {
      CheckPopups = false;

    }
  }

  internal void CheckBonusGame()
  {
    if (audioController) audioController.PlayWLAudio("bonusStart");
    StartCoroutine(_bonusManager.BonusStartupPage(SocketManager.ResultData.features.bonus.spinCount));                                                         // startBonus


  }
  private void MoveObjectsToNewParent(GameObject newParent, GameObject obj)
  {
    if (obj != null)
    {
      Transform objTransform = obj.transform;
      originalData[objTransform] = (objTransform.parent, objTransform.GetSiblingIndex());
      //  Vector3 pos = obj.transform.position;
      objTransform.SetParent(newParent.transform, true);

    }

  }
  private void RestoreObjectsToOriginalParents()
  {
    foreach (var entry in originalData)
    {
      Transform objTransform = entry.Key;
      Transform originalParent = entry.Value.parent;
      int originalIndex = entry.Value.siblingIndex;

      if (objTransform != null && originalParent != null)
      {
        objTransform.SetParent(originalParent, true);
        objTransform.SetSiblingIndex(originalIndex);
      }
    }

    originalData.Clear();
  }
  // Returns the jackpot tier (mini/minor/major/mega) for a scatter `value`, or -1 if it isn't one.
  // The backend sends scatter values already multiplied by the per-line bet, so a jackpot arrives
  // as jackpotMultipliers[i] * bets[BetCounter] (matching the MiniMajorMinor panel), not the raw
  // multiplier. Compared with a tolerance since these are doubles.
  internal int GetJackpotTier(double value)
  {
    var jm = SocketManager.InitFeature?.jackpotMultipliers;
    var bets = SocketManager.InitialData?.bets;
    if (jm == null || bets == null || BetCounter < 0 || BetCounter >= bets.Count) return -1;
    double bet = bets[BetCounter];
    for (int i = 0; i < jm.Count; i++)
    {
      if (Math.Abs(value - jm[i] * bet) < 0.0001) return i;
    }
    return -1;
  }

  // Jackpot label sprite for a scatter value, or null when it's a regular (non-jackpot) value.
  internal Sprite GetJackpotSprite(double value)
  {
    int tier = GetJackpotTier(value);
    if (tier >= 0 && JackpotSprites != null && tier < JackpotSprites.Length) return JackpotSprites[tier];
    return null;
  }

  // localScale for a jackpot label image: mini (tier 0) is slightly smaller, the rest use 0.5.
  internal float GetJackpotImageScale(double value)
  {
    return GetJackpotTier(value) == 0 ? 0.45f : 0.5f;
  }

  private void ResetSlotText()
  {
    foreach (var temp in Tempimages)
    {
      foreach (var txt in temp.slotImages)
      {

        txt.transform.localScale = Vector3.one;
        txt.transform.GetChild(0).gameObject.SetActive(false);
        txt.transform.GetChild(0).localScale = Vector3.one;
        if (txt.transform.childCount > 1) txt.transform.GetChild(1).gameObject.SetActive(false);
      }
    }

    bool bonusTriggered = SocketManager.ResultData.features.bonus.isTriggered;

    if (SocketManager.ResultData.features.bonus.scatterValues.Count > 0)
    {
      for (int i = 0; i < SocketManager.ResultData.features.bonus.scatterValues.Count; i++)
      {
        double scatterValue = SocketManager.ResultData.features.bonus.scatterValues[i].value;
        var slotSymbol = Tempimages[SocketManager.ResultData.features.bonus.scatterValues[i].index[0]].slotImages[SocketManager.ResultData.features.bonus.scatterValues[i].index[1]];
        slotSymbol.transform.localScale *= ScatterSymbolScale;

        int jackpotTier = GetJackpotTier(scatterValue);
        if (jackpotTier >= 0)
        {
          // Jackpot tier: show the label image (second child) instead of the numeric text.
          if (slotSymbol.transform.childCount > 1)
          {
            Image jackpotImg = slotSymbol.transform.GetChild(1).GetComponent<Image>();
            if (jackpotImg && JackpotSprites != null && jackpotTier < JackpotSprites.Length) jackpotImg.sprite = JackpotSprites[jackpotTier];
            slotSymbol.transform.GetChild(1).localScale = Vector3.one * GetJackpotImageScale(scatterValue);
            slotSymbol.transform.GetChild(1).gameObject.SetActive(true);
          }
        }
        else
        {
          // Regular value as text. In base game with no bonus triggered, inflate it as a teaser.
          slotSymbol.gameObject.transform.GetChild(0).gameObject.SetActive(true);
          string displayText;
          if (bonusTriggered)
          {
            displayText = scatterValue.ToString();
          }
          else
          {
            // Teaser value for base game: scatterValue rounded up to the nearest multiple of 5
            // (minimum 5), then scaled by a random integer factor so it stays an exact multiple
            // of 5 while looking bigger.
            double baseMultipleOfFive = Math.Max(5, Math.Ceiling(scatterValue / 5.0) * 5.0);
            int bigFactor = UnityEngine.Random.Range(3, 8);
            displayText = (baseMultipleOfFive * bigFactor).ToString("F1");
          }
          slotSymbol.gameObject.GetComponentInChildren<TMP_Text>().text = displayText;
          slotSymbol.transform.GetChild(0).localScale *= 0.75f;
        }


        ImageAnimation FixedSlotAnim = slotSymbol.gameObject.GetComponent<ImageAnimation>();
        FixedSlotAnim.textureArray.Clear();
        FixedSlotAnim.textureArray.TrimExcess();
        for (int j = 0; j < Sun_Sprite.Length; j++)
        {
          FixedSlotAnim.textureArray.Add(Sun_Sprite[j]);
        }
        FixedSlotAnim.AnimationSpeed = 40f;
        TempList.Add(FixedSlotAnim);
        FixedSlotAnim.StopAnimation();
        FixedSlotAnim.StartAnimation();

        //Tempimages[SocketManager.ResultData.features.bonus.scatterValues[i].index[0]].slotImages[SocketManager.ResultData.features.bonus.scatterValues[i].index[1]].material = MaskOverRide;
      }
    }
  }
  IEnumerator CheckPayoutLineBackend(List<int> LineId)
  {
    Debug.Log("Dev_Test: " + Newtonsoft.Json.JsonConvert.SerializeObject(LineId));
    if (LineId.Count > 0)
    {

      List<KeyValuePair<int, int>> coords = new();
      for (int j = 0; j < LineId.Count; j++)
      {
        for (int k = 0; k < SocketManager.ResultData.payload.wins[j].positions.Count; k++)
        {
          int rowIndex = SocketManager.InitialData.lines[LineId[j]][k];
          int columnIndex = k;
          coords.Add(new KeyValuePair<int, int>(rowIndex, columnIndex));
        }
        if (!IsAutoSpin && !IsFreeSpin)
        {
          foreach (var coord in coords)
          {
            int rowIndex = coord.Key;
            int columnIndex = coord.Value;
            StartGameAnimation(Tempimages[rowIndex].slotImages[columnIndex].gameObject);

          }
          yield return new WaitForSeconds(1.5f);
          StopGameAnimation(false);
          coords.Clear();
        }
      }
      if (IsAutoSpin || IsFreeSpin)
      {
        foreach (var coord in coords)
        {
          int rowIndex = coord.Key;
          int columnIndex = coord.Value;
          StartGameAnimation(Tempimages[rowIndex].slotImages[columnIndex].gameObject);

        }
      }

      yield return new WaitForSeconds(1);
      StopGameAnimation(false);


      WinningsAnim(true);
    }
    else
    {

      //if (audioController) audioController.PlayWLAudio("lose");
      if (audioController) audioController.StopWLAaudio();
    }
  }

  private Coroutine SlotAnimRoutine = null;
  private void WinningsAnim(bool IsStart)
  {
    if (IsStart)
    {
      //  WinTween = TotalWin_text.gameObject.GetComponent<RectTransform>().DOScale(new Vector2(1.5f, 1.5f), 1f).SetLoops(-1, LoopType.Yoyo).SetDelay(0);
    }
    else
    {
      // WinTween.Kill();
      // TotalWin_text.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
    }
  }

  #endregion



  internal IEnumerator FreeSpinOptionSelected(int buttonIndex)                                                                  //                 freespinOptionCall
  {
    if (audioController) audioController.PlayButtonAudio();
    if (buttonIndex < 5)
    {
      SocketManager.AcumulateFreeSpin(buttonIndex);
      priviousButtonIndex = buttonIndex;
    }
    else
    {
      int x = UnityEngine.Random.Range(0, 5);
      if (buttonIndex != 8) SocketManager.AcumulateFreeSpin(x);
      priviousButtonIndex = x;
    }
    yield return (SocketManager.isResultdone);
    uiManager.StartFirstFreeSpin(uiManager.FreeSpinOptionButton[priviousButtonIndex].SpinNumber, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer1, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer2, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer3);
  }



  internal void CallCloseSocket()
  {
    StartCoroutine(SocketManager.CloseSocket());
  }


  // Remembered so an orientation switch can re-apply it: TBetPlus/TBetMinus_Button are
  // rebound to the other orientation's widgets, which never received the last toggle
  // and would otherwise stay interactable mid-spin or mid-bonus.
  private bool buttonGrpEnabled = true;

  internal void ReapplyButtonGrpState()
  {
    ToggleButtonGrp(buttonGrpEnabled);
  }

  void ToggleButtonGrp(bool toggle)
  {
    buttonGrpEnabled = toggle;

    if (SlotStart_Button) SlotStart_Button.interactable = toggle;
    if (Mobile_SlotStart_Button) Mobile_SlotStart_Button.interactable = toggle;
    //if (P_AutoSpinStop_Button) P_AutoSpinStop_Button.interactable = toggle;
    //if (Mobile_StopSpin_Button) Mobile_StopSpin_Button.interactable = toggle;
    // if (StopSpin_Button) StopSpin_Button.interactable = toggle;
    //if (M_AutoSpinStop_Button) M_AutoSpinStop_Button.interactable = toggle;

    if (TBetMinus_Button) TBetMinus_Button.interactable = toggle;
    if (TBetPlus_Button) TBetPlus_Button.interactable = toggle;
  }

  //start the icons animation
  private void StartGameAnimation(GameObject animObjects)
  {
    ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();

    temp.StartAnimation();
    TempList.Add(temp);
  }

  //stop the icons animation
  private void StopGameAnimation(bool WithSun = true)
  {
    if (WithSun)
    {
      for (int i = 0; i < TempList.Count; i++)
      {
        TempList[i].StopAnimation();
      }
      TempList.Clear();
      TempList.TrimExcess();
    }
    else
    {
      for (int i = 0; i < TempList.Count; i++)
      {
        Transform obj = TempList[i].transform;
        // A displayed scatter shows either its value text (GetChild(0)) or its jackpot label
        // image (GetChild(1)); keep those looping. Only stop symbols with neither shown.
        bool scatterShown = obj.GetChild(0).gameObject.activeInHierarchy
          || (obj.childCount > 1 && obj.GetChild(1).gameObject.activeInHierarchy);
        if (!scatterShown)
        {
          TempList[i].StopAnimation();
        }
      }
    }

  }


  #region TweeningCode
  private void InitializeTweening(Transform slotTransform)
  {
    slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
    Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
    tweener.Play();
    alltweens.Add(tweener);
  }



  private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index, bool isStop)
  {
    alltweens[index].Kill();
    int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
    slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, -100);
    alltweens[index] = slotTransform.DOLocalMoveY(-tweenpos + 250f, 0.3f).SetEase(Ease.OutQuad);
    audioController.PlayWLAudio("spinStop");
    if (!isStop)
    {
      yield return new WaitForSeconds(0.2f);
    }
    else
    {
      yield return null;
    }


  }


  private void KillAllTweens()
  {
    for (int i = 0; i < numberOfSlots; i++)
    {
      alltweens[i].Kill();
    }
    alltweens.Clear();

  }
  #endregion

}

[Serializable]
public class SlotImage
{
  public List<Image> slotImages = new List<Image>(10);
}

[Serializable]
public class Slottext
{
  public List<TMP_Text> slotImages = new List<TMP_Text>(10);
}

