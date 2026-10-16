using System.Collections;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class FireballPath
{
  public Transform PointA;
  public Transform PointB;
  public float ImageRotationZ;
}

public class FireballBackgroundController : MonoBehaviour
{
  [SerializeField] private FireballPool fireballPool;

  [Header("Landscape / PC Paths")]
  [SerializeField] private FireballPath LeftPath;
  [SerializeField] private FireballPath RightPath;

  [Header("Portrait / Mobile Paths")]
  [SerializeField] private FireballPath Mobile_LeftPath;
  [SerializeField] private FireballPath Mobile_RightPath;

  [Header("Timing")]
  [SerializeField] private float TravelDuration = 2.5f;
  [SerializeField] private float SpawnInterval = 2f;

  [Header("Randomness")]
  [SerializeField] private float PointA_RandomX = 100f;
  [SerializeField] private float PointB_RandomY = 100f;

  [Header("Pool Parent Width (for masking)")]
  [SerializeField] private float Mobile_PoolWidth = 1350f;
  [SerializeField] private float PoolWidth = 2340f;

  private bool isMobile = false;

  private void Start()
  {
    ApplyPoolWidth();
    StartCoroutine(FireballLoop());
  }

  internal void SetOrientation(bool mobile)
  {
    isMobile = mobile;
    ApplyPoolWidth();
  }

  private void ApplyPoolWidth()
  {
    if (fireballPool == null) return;

    RectTransform poolParent = fireballPool.PoolParentRect;
    if (poolParent == null) return;

    poolParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, isMobile ? Mobile_PoolWidth : PoolWidth);
  }

  private IEnumerator FireballLoop()
  {
    while (true)
    {
      LaunchFireball(isMobile ? Mobile_LeftPath : LeftPath);
      LaunchFireball(isMobile ? Mobile_RightPath : RightPath);
      yield return new WaitForSeconds(SpawnInterval);
    }
  }

  private void LaunchFireball(FireballPath path)
  {
    if (fireballPool == null || path == null || path.PointA == null || path.PointB == null)
      return;

    Fireball fireball = fireballPool.GetFromPool();

    // Tween in local space: the canvas scale factor changes on orientation switch, so a
    // world-space target goes stale mid-flight. The random offsets are in the same local
    // units for the same reason.
    Transform parent = fireball.transform.parent;
    if (parent == null)
    {
      Debug.LogError("[FireballBackgroundController] Pooled fireball has no parent — assign ParentTransform on the FireballPool.");
      fireballPool.ReturnToPool(fireball);
      return;
    }

    Vector3 localStart = parent.InverseTransformPoint(path.PointA.position);
    localStart.x += Random.Range(-PointA_RandomX, PointA_RandomX);

    Vector3 localTarget = parent.InverseTransformPoint(path.PointB.position);
    localTarget.y += Random.Range(-PointB_RandomY, PointB_RandomY);

    fireball.transform.localPosition = localStart;
    fireball.SetRotation(path.ImageRotationZ);
    StartCoroutine(TravelFireball(fireball, localTarget));
  }

  private IEnumerator TravelFireball(Fireball fireball, Vector3 localTarget)
  {
    Tween moveTween = fireball.transform.DOLocalMove(localTarget, TravelDuration).SetEase(Ease.Linear);
    yield return moveTween.WaitForCompletion();
    fireball.transform.DOKill();
    fireballPool.ReturnToPool(fireball);
  }

  private void OnDisable()
  {
    StopAllCoroutines();
    if (fireballPool != null)
      fireballPool.KillTweensAndReturnAll();
  }
}
