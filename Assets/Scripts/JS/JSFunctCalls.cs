using System.Runtime.InteropServices;
using UnityEngine;

public class JSFunctCalls : MonoBehaviour
{
  [DllImport("__Internal")] private static extern void SendPostMessage(string message);

  [DllImport("__Internal")] private static extern void RegisterVisibilityChangeListener(string gameObjectName);

  [DllImport("__Internal")] private static extern void RegisterResizeListener(string gameObjectName, string methodName);

  [DllImport("__Internal")] private static extern void RegisterTokenListener(string gameObjectName, string methodName);

  [SerializeField] private SlotBehaviour slotBehaviour;

  // Start, not Awake: OrientationChange's Awake/Start must run before the initial dimensions callback.
  void Start()
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    RegisterVisibilityChangeListener(gameObject.name);
#endif
    RegisterDimensionsListener();
  }

  // Called from CustomJsLib.jslib via SendMessage on visibilitychange/blur/focus.
  public void OnFocusChanged(string value)
  {
    if (slotBehaviour) slotBehaviour.HandleFocus(value == "1");
  }

  internal void SendCustomMessage(string message){
#if UNITY_WEBGL && !UNITY_EDITOR
    SendPostMessage(message);
#endif
  }

  // Self-contained resize bridge: the page drives OC.SwitchDisplay("width,height") on its own resize.
  internal void RegisterDimensionsListener(string gameObjectName = "OC", string methodName = "SwitchDisplay")
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    RegisterResizeListener(gameObjectName, methodName);
#else
    Debug.Log($"[JS] Resize listener not registered ('{gameObjectName}.{methodName}', editor mode)");
#endif
  }

  // Inbound auth: routes the host's "TokenReceived" message to gameObjectName.methodName(json).
  internal void RegisterAuthTokenListener(string gameObjectName, string methodName = "ReceiveAuthToken")
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    RegisterTokenListener(gameObjectName, methodName);
#else
    Debug.Log($"[JS] Token listener not registered ('{gameObjectName}.{methodName}', editor mode)");
#endif
  }
}
