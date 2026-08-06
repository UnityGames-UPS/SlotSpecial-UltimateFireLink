using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class OrientationChange : MonoBehaviour
{
  [SerializeField] private CanvasScalerSwitcher canvasSwitch;
  [SerializeField] private CanvasScaler CanvasScaler;
  [SerializeField] private float transitionDuration = 0.5f;

  private Tween matchTween;
  private Coroutine matchRoutine;
  private bool hasApplied;
  private bool appliedPortrait;

  // The host's first SwitchDisplay can arrive a frame or more after load; apply the
  // current screen immediately so the very first frame is already in the right
  // orientation instead of showing the scene-authored one.
  private void Start()
  {
    SwitchDisplay(Screen.width + "," + Screen.height);
  }

  /// <summary>
  /// Platform entry point ("width,height"), sent by the React host on load and on
  /// orientation change. Orientation alone decides which UI set is shown.
  /// </summary>
  void SwitchDisplay(string dimensions)
  {
    string[] parts = dimensions.Split(',');
    if (parts.Length != 2
        || !int.TryParse(parts[0], out int width)
        || !int.TryParse(parts[1], out int height)
        || width <= 0 || height <= 0)
    {
      Debug.LogWarning("Unity: SwitchDisplay got malformed dimensions: " + dimensions);
      return;
    }

    Debug.Log($"Unity: Received Dimensions - Width: {width}, Height: {height}");
    bool portrait = width < height;

    // The host may resend on every resize, so only an actual orientation flip rebuilds
    // the view. The match math still runs every time -- the aspect can change within a
    // single orientation (a windowed desktop going 16:9 -> 21:9).
    if (!hasApplied || portrait != appliedPortrait)
    {
      // Must run BEFORE the match math -- it sets referenceResolution, which the math reads.
      canvasSwitch.ApplyView(portrait);
      hasApplied = true;
      appliedPortrait = portrait;
    }

    if (matchRoutine != null) StopCoroutine(matchRoutine);
    matchRoutine = StartCoroutine(MatchCoroutine(width, height));
  }

  // Device class no longer picks the view -- it only flags the iPhone notch offsets.
  void DiviceCheck(string device)
  {
    Debug.Log("Unity: Received DeviceCheck:   " + device);
    canvasSwitch.SetApple(device == "IP");
  }

  IEnumerator MatchCoroutine(int width, int height)
  {
    // Let the new referenceResolution and layout settle for a frame.
    yield return null;

    Vector2 referenceAspect = CanvasScaler.referenceResolution;
    float widthScale = width / referenceAspect.x;
    float heightScale = height / referenceAspect.y;

    // No counter-rotation anymore, so this holds for both orientations.
    float targetScale = Mathf.Min(widthScale, heightScale);

    float targetMatch;
    if (Mathf.Abs(heightScale - widthScale) < 0.0001f)
    {
      targetMatch = 0.5f;
    }
    else
    {
      float logRatio = Mathf.Log(heightScale / widthScale);
      targetMatch = Mathf.Clamp01(Mathf.Log(targetScale / widthScale) / logRatio);
    }

    if (matchTween != null && matchTween.IsActive()) matchTween.Kill();
    matchTween = DOTween.To(() => CanvasScaler.matchWidthOrHeight, x => CanvasScaler.matchWidthOrHeight = x, targetMatch, transitionDuration).SetEase(Ease.InOutQuad);
    Debug.Log($"matchWidthOrHeight set to: {targetMatch}");
  }

  private void OnDisable()
  {
    if (matchTween != null && matchTween.IsActive()) matchTween.Kill();
    matchTween = null;
  }

#if UNITY_EDITOR
  private void Update()
  {
    // Change the Game view resolution preset, then press Space. Runs the exact
    // same path as the platform message.
    if (Input.GetKeyDown(KeyCode.Space))
    {
      SwitchDisplay(Screen.width + "," + Screen.height);
    }
  }
#endif
}
