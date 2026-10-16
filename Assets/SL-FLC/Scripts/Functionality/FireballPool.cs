using DG.Tweening;
using UnityEngine;

public class FireballPool : GenericObjectPool<Fireball>
{
  internal RectTransform PoolParentRect => ParentTransform as RectTransform;

  // The base fallback does not activate overflow items the way GetFromPool does.
  internal override Fireball CreateNewPooledItem()
  {
    Fireball newItem = base.CreateNewPooledItem();
    newItem.gameObject.SetActive(true);
    return newItem;
  }

  // The base reset does not kill tweens, which would keep writing to returned items.
  internal void KillTweensAndReturnAll()
  {
    for (int i = 0; i < ItemsInUse.Count; i++)
      ItemsInUse[i].transform.DOKill();
    ReturnAllItemsToPool();
  }
}
