using System.Runtime.InteropServices;
using UnityEngine;

public class JSFunctCalls : MonoBehaviour
{
  [DllImport("__Internal")] private static extern void SendLogToReactNative(string message);

  [DllImport("__Internal")] private static extern void SendPostMessage(string message);

  [DllImport("__Internal")] private static extern void RegisterVisibilityChangeListener(string gameObjectName);

  [SerializeField] private SlotBehaviour slotBehaviour;

  void Start()
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    RegisterVisibilityChangeListener(gameObject.name);
#endif
  }

  // Called from CustomJsLib.jslib via SendMessage on visibilitychange/blur/focus.
  public void OnFocusChanged(string value)
  {
    if (slotBehaviour) slotBehaviour.HandleFocus(value == "1");
  }

  void OnEnable()
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    Application.logMessageReceived += HandleLog;
#endif
  }

  void OnDisable()
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    Application.logMessageReceived -= HandleLog;
#endif
  }

#if UNITY_WEBGL && !UNITY_EDITOR
  void HandleLog(string logString, string stackTrace, LogType type)
  {
    string formattedMessage = $"[{type}] {logString}";
    SendLogToReactNative(formattedMessage);
  }
#endif

  internal void SendCustomMessage(string message){
#if UNITY_WEBGL && !UNITY_EDITOR
    SendPostMessage(message);
#endif
  }
}
