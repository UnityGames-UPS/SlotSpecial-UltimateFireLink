using UnityEngine;

public class Fireball : MonoBehaviour
{
  [SerializeField] private Transform ImageChild;

  internal void SetRotation(float zAngle)
  {
    if (ImageChild != null)
      ImageChild.localRotation = Quaternion.Euler(0, 0, zAngle);
  }
}
