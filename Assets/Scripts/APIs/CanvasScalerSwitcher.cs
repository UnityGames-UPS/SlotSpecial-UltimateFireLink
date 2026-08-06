using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasScalerSwitcher : MonoBehaviour
{
  [SerializeField]
  private CanvasScaler canvasOP;

  [SerializeField]
  private UIManager Uimanager;

  [SerializeField]
  private SlotBehaviour slotManager;

  [SerializeField]
  private FireballBackgroundController fireballController;

  [Header("UI Change For Apple")]

  [SerializeField]
  private GameObject TheBgObj;
  [Header("UI Change For Apple")]

  [SerializeField] internal GameObject MobileTopBar;
  [SerializeField] private GameObject MobileBottomBar;
  [SerializeField] private GameObject MobileMajorMiniMinor;
  [SerializeField] private GameObject BonusCountUI;

  [Header("BG Change")]
  [SerializeField] private GameObject LandscapeBG;
  [SerializeField] private GameObject PotrateBG;
  [SerializeField] private GameObject FreeSpinCount;
  [SerializeField] private GameObject BonusSpinCount;
  [SerializeField] private Transform MobileFreeSpinCountpos;
  [SerializeField] private Transform MobileSunHitPoint;
  [SerializeField] private GameObject SunHitPoint;

  [SerializeField] private GameObject MainSlot;
  [SerializeField] private GameObject BonusSlot;

  [Header("UI Elements")]
  [SerializeField] private Button m_ibutton;
  [SerializeField] private Button p_ibutton;
  [SerializeField] private Button m_MusicButton;
  [SerializeField] private Button m_MusicOFFButton;
  [SerializeField] private Button p_MusicButton;
  [SerializeField] private Button p_MusicOFFButton;
  [SerializeField] private Button m_SoundButton;
  [SerializeField] private Button m_SoundOFFButton;
  [SerializeField] private Button p_SoundButton;
  [SerializeField] private Button p_SoundOFFButton;
  [SerializeField] private Button p_backButton;
  [SerializeField] private Button m_backButton;
  [SerializeField] private Button m_plusButton;
  [SerializeField] private Button p_plusButton;
  [SerializeField] private Button p_minusButton;
  [SerializeField] private Button m_minusButton;


  [SerializeField] private TMP_Text m_creditText;
  [SerializeField] private TMP_Text p_creditText;
  [SerializeField] private TMP_Text p_winText;
  [SerializeField] private TMP_Text m_winText;
  [SerializeField] private TMP_Text p_TotalbetText;
  [SerializeField] private TMP_Text m_TotalbetText;
  [SerializeField] private TMP_Text[] m_MiniMajorMinor;
  [SerializeField] private TMP_Text[] p_MiniMajorMinor;

  // Snapshot of every transform the view switch mutates, captured once in Awake.
  // Restoring it before each apply is what makes switching repeatable in both
  // directions -- the old *firstTime guards existed only to stop the cumulative
  // offsets from stacking, and they blocked the second orientation from applying.
  private struct RectState
  {
    public Vector2 anchorMin, anchorMax, pivot, anchoredPosition, sizeDelta, offsetMin, offsetMax;
    public Vector3 localPosition, localScale;

    public static RectState Capture(RectTransform rt)
    {
      return new RectState
      {
        anchorMin = rt.anchorMin,
        anchorMax = rt.anchorMax,
        pivot = rt.pivot,
        anchoredPosition = rt.anchoredPosition,
        sizeDelta = rt.sizeDelta,
        offsetMin = rt.offsetMin,
        offsetMax = rt.offsetMax,
        localPosition = rt.localPosition,
        localScale = rt.localScale
      };
    }

    public void ApplyTo(RectTransform rt)
    {
      rt.anchorMin = anchorMin;
      rt.anchorMax = anchorMax;
      rt.pivot = pivot;
      rt.localScale = localScale;
      rt.anchoredPosition = anchoredPosition;
      rt.sizeDelta = sizeDelta;
      // offsets last: for stretch anchors they are the authoritative view of the rect
      rt.offsetMin = offsetMin;
      rt.offsetMax = offsetMax;
      rt.localPosition = localPosition;
    }
  }

  // Keyed on RectTransform and stored as anchoredPosition, NOT localPosition.
  // Every one of these objects is a RectTransform parented under BG, and
  // localPosition is derived from anchoredPosition + the anchor reference point,
  // which moves whenever BG's rect changes (referenceResolution, offsets, scale all
  // change on switch). Caching localPosition would restore a number computed against
  // the other orientation's parent size and land the object in the wrong place.
  private readonly Dictionary<RectTransform, Vector2> _anchoredPosCache = new Dictionary<RectTransform, Vector2>();
  private readonly Dictionary<Transform, Vector3> _localPosCache = new Dictionary<Transform, Vector3>();
  private RectTransform _bgRect;
  private RectState _bgState;

  private bool _isApple;
  private bool _hasApplied;
  private bool _appliedPortrait;

  void Awake()
  {
    _bgRect = TheBgObj ? TheBgObj.GetComponent<RectTransform>() : null;
    if (_bgRect) _bgState = RectState.Capture(_bgRect);

    CachePos(MainSlot);
    CachePos(BonusSlot);
    CachePos(MobileTopBar);
    CachePos(MobileBottomBar);
    CachePos(MobileMajorMiniMinor);
    CachePos(BonusCountUI);
    CachePos(FreeSpinCount);
    CachePos(BonusSpinCount);
    CachePos(SunHitPoint);
  }

  private void CachePos(GameObject go)
  {
    if (!go) return;

    RectTransform rt = go.transform as RectTransform;
    if (rt) _anchoredPosCache[rt] = rt.anchoredPosition;
    else _localPosCache[go.transform] = go.transform.localPosition;
  }

  // Device messages no longer choose the view -- they only tell us whether the
  // notch offsets apply. Orientation alone decides mobile vs PC.
  public void OnMobileDeviceDetected(string s)
  {
    Debug.Log("Called OnMobileDeviceDetected: " + s);
    SetApple(s == "I");
  }

  [ContextMenu("Toggle Apple")]
  private void ToggleApple() { SetApple(!_isApple); }

  public void SetApple(bool apple)
  {
    if (_hasApplied && _isApple == apple) return;
    _isApple = apple;
    // If a view is already live, re-run it so the notch offsets appear/disappear.
    // Safe because apply is restore-then-apply, and this deliberately bypasses the
    // orientation debounce (which lives in OrientationChange.SwitchDisplay).
    if (_hasApplied) ApplyView(_appliedPortrait);
  }

  /// <summary>The only entry point that changes the view. Unconditionally idempotent.</summary>
  public void ApplyView(bool portrait)
  {
    RestoreCachedTransforms();

    if (portrait) ApplyPortrait();
    else ApplyLandscape();

    _hasApplied = true;
    _appliedPortrait = portrait;
  }

  private void RestoreCachedTransforms()
  {
    if (_bgRect) _bgState.ApplyTo(_bgRect);

    foreach (KeyValuePair<RectTransform, Vector2> kv in _anchoredPosCache)
    {
      if (kv.Key) kv.Key.anchoredPosition = kv.Value;
    }

    foreach (KeyValuePair<Transform, Vector3> kv in _localPosCache)
    {
      if (kv.Key) kv.Key.localPosition = kv.Value;
    }
  }

  private void SetFireballOrientation(bool mobile)
  {
    if (fireballController == null)
    {
      Debug.LogWarning("[CanvasScalerSwitcher] fireballController is not assigned — fireballs will stay on the landscape paths.");
      return;
    }
    fireballController.SetOrientation(mobile);
  }

  // iPhone notch/Dynamic Island compensation. The MainSlot/BonusSlot +100 here is
  // deliberate and stacks with the +100 ApplyPortrait already applied, giving the
  // +200 total that iPhone portrait has always shipped with.
  private void ApplyAppleOffsets()
  {
    Shift(MainSlot, 100f);
    Shift(BonusSlot, 100f);
    Shift(MobileBottomBar, 100f);

    Shift(MobileTopBar, -100f);
    Shift(MobileMajorMiniMinor, -100f);
    Shift(BonusCountUI, -100f);
  }

  // Shift in the same unit the cache restores, so offsets and restore stay consistent.
  private static void Shift(GameObject go, float y)
  {
    if (!go) return;

    RectTransform rt = go.transform as RectTransform;
    if (rt) rt.anchoredPosition += new Vector2(0f, y);
    else go.transform.localPosition += new Vector3(0f, y, 0f);
  }

  private void ApplyLandscape()
  {
    PotrateBG.gameObject.SetActive(false);
    LandscapeBG.gameObject.SetActive(true);
    canvasOP.referenceResolution = new Vector2(1920f, 1080f);

    // Applied on top of the restored baseline, so it can never accumulate.
    if (_bgRect)
    {
      _bgRect.localScale = Vector3.one * 0.8f;
      Vector2 offsetMin = _bgRect.offsetMin; // left, bottom
      Vector2 offsetMax = _bgRect.offsetMax; // right, top

      offsetMin.y += -70f;   // move bottom edge UP by 70
      offsetMax.y -= 70f;   // move top edge DOWN by 70

      _bgRect.offsetMin = offsetMin;
      _bgRect.offsetMax = offsetMax;
    }

    Uimanager.IsPcToggle(true);

    Uimanager.Paytable_Button = p_ibutton;
    Uimanager.Sound_Button = p_SoundButton;
    Uimanager.Music_Button = p_MusicButton;
    Uimanager.GameExit_Button = p_backButton;
    Uimanager.SoundOFF_Button = p_SoundOFFButton;
    Uimanager.MusicOFF_Button = p_MusicOFFButton;

    slotManager.Balance_text = p_creditText;
    slotManager.TotalBet_text = p_TotalbetText;
    slotManager.TotalWin_text = p_winText;
    slotManager.TBetPlus_Button = p_plusButton;
    slotManager.TBetMinus_Button = p_minusButton;

    for (int i = 0; i < p_MiniMajorMinor.Length; i++)
    {
      slotManager.MiniMajorMinor[i] = null;
      slotManager.MiniMajorMinor[i] = p_MiniMajorMinor[i];

    }
    SetFireballOrientation(false);

    Uimanager.UpdateThebuttons();
    slotManager.assignbuttons();
    slotManager.RefreshBoundTexts();
    // Re-apply live gameplay state to the newly-bound widgets: they were never told
    // about a spin/bonus that started while the other orientation was active.
    // (Title state is already handled by IsPcToggle -> ApplyTitleState above.)
    slotManager.ReapplyButtonGrpState();
  }

  private void ApplyPortrait()
  {
    PotrateBG.gameObject.SetActive(true);
    LandscapeBG.gameObject.SetActive(false);
    canvasOP.referenceResolution = new Vector2(1080f, 2340f);
    Uimanager.IsPcToggle(false);

    Uimanager.Paytable_Button = m_ibutton;
    Uimanager.Sound_Button = m_SoundButton;
    Uimanager.Music_Button = m_MusicButton;
    Uimanager.GameExit_Button = m_backButton;
    Uimanager.SoundOFF_Button = m_SoundOFFButton;
    Uimanager.MusicOFF_Button = m_MusicOFFButton;


    slotManager.Balance_text = m_creditText;
    slotManager.TotalBet_text = m_TotalbetText;
    slotManager.TotalWin_text = m_winText;
    slotManager.TBetPlus_Button = m_plusButton;
    slotManager.TBetMinus_Button = m_minusButton;

    for (int i = 0; i < m_MiniMajorMinor.Length; i++)
    {
      slotManager.MiniMajorMinor[i] = null;
      slotManager.MiniMajorMinor[i] = m_MiniMajorMinor[i];
    }

    // Order matters: local offsets first, then snap-to-target, then rebind.
    // Snapping first would let the +100 drag these along if they are parented
    // under MainSlot.
    Shift(MainSlot, 100f);
    Shift(BonusSlot, 100f);

    if (_isApple) ApplyAppleOffsets();

    // referenceResolution changed above, so the canvas rect is stale this frame.
    // These three snap to live world positions, so flush the layout first or they
    // read targets computed against the previous orientation's scale factor.
    Canvas.ForceUpdateCanvases();

    FreeSpinCount.transform.position = MobileFreeSpinCountpos.position;
    BonusSpinCount.transform.position = MobileFreeSpinCountpos.position;
    SunHitPoint.transform.position = MobileSunHitPoint.position;

    SetFireballOrientation(true);

    Uimanager.UpdateThebuttons();
    slotManager.assignbuttons();
    slotManager.RefreshBoundTexts();
    // Re-apply live gameplay state to the newly-bound widgets: they were never told
    // about a spin/bonus that started while the other orientation was active.
    // (Title state is already handled by IsPcToggle -> ApplyTitleState above.)
    slotManager.ReapplyButtonGrpState();
  }
}
