using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioController : MonoBehaviour
{
  [SerializeField] private AudioSource bg_adudio;
  [SerializeField] internal AudioSource audioPlayer_wl;
  [SerializeField] internal AudioSource audioPlayer_button;
  [SerializeField] internal AudioSource audioSpin_button;
  [SerializeField] private AudioClip[] clips;
  [SerializeField] private AudioClip[] Bonusclips;
  [SerializeField] private AudioSource bg_audioBonus;
  [SerializeField] private AudioSource audioPlayer_Bonus;

  // Focus muting works purely through the .mute flag. Engine Pause()/UnPause() is
  // deliberately not used: it is a second, independent thing controlling audibility
  // on top of .mute, and the two desync (a source paused on blur stays silent after
  // the user turns sound back on, until the next focus cycle happens to resume it).
  private readonly Dictionary<AudioSource, bool> preFocusMuteState = new Dictionary<AudioSource, bool>();
  private bool isForceMuted = false;

  private void Start()
  {
    if (bg_adudio) bg_adudio.Play();
    audioPlayer_button.clip = clips[clips.Length - 1];
    audioSpin_button.clip = clips[clips.Length - 2];
  }

  private IEnumerable<AudioSource> AllSources()
  {
    yield return bg_adudio;
    yield return bg_audioBonus;
    yield return audioPlayer_wl;
    yield return audioPlayer_Bonus;
    yield return audioPlayer_button;
    yield return audioSpin_button;
  }

  /// <summary>
  /// Focus-driven mute. Called from BOTH the JS visibility path
  /// (JSFunctCalls.OnFocusChanged -> SlotBehaviour.HandleFocus) and Unity's native
  /// OnApplicationFocus, either or both of which may fire for one blur/focus event --
  /// hence the isForceMuted guard, without which the second call would capture the
  /// already-forced `true` as the user's setting and leave audio stuck muted.
  /// </summary>
  internal void SetMuteAll(bool forceMute)
  {
    if (forceMute == isForceMuted) return;
    isForceMuted = forceMute;

    foreach (AudioSource source in AllSources())
    {
      if (!source) continue;
      if (forceMute)
      {
        preFocusMuteState[source] = source.mute;
        source.mute = true;
      }
      else
      {
        source.mute = preFocusMuteState.TryGetValue(source, out bool prevMuted) ? prevMuted : source.mute;
      }
    }
  }

  /// <summary>
  /// Called from the sound/music buttons before they apply their new state. An explicit
  /// tap proves the game really has focus, so a forced mute left stuck by an unpaired
  /// blur signal (routine in WebView embeds) must not swallow the press.
  /// </summary>
  internal void ReleaseFocusMute()
  {
    SetMuteAll(false);
  }

  // Single write point for the user's own mute choice. While a focus mute is in
  // effect the choice is recorded but not applied, so regaining focus restores the
  // latest user setting instead of clobbering it.
  private void SetSourceMute(AudioSource source, bool mute)
  {
    if (!source) return;
    if (isForceMuted)
    {
      preFocusMuteState[source] = mute;
      return;
    }
    source.mute = mute;
  }

  internal void SwitchBGSound(bool isbonus)
  {
    if (isbonus)
    {
      if (bg_audioBonus) bg_audioBonus.enabled = true;
      if (bg_adudio) bg_adudio.enabled = false;
    }
    else
    {
      if (bg_audioBonus) bg_audioBonus.enabled = false;
      if (bg_adudio) bg_adudio.enabled = true;
    }
  }

  internal void PlayWLAudio(string type)
  {
    audioPlayer_wl.loop = false;
    int index = 0;
    switch (type)
    {
      case "spin":
        index = 250;
        //audioPlayer_wl.loop = true;
        break;
      case "win":
        index = 0;
        break;
      case "lose":
        index = 2;
        break;
      case "spinStop":
        index = 10;
        break;
      case "megaWin":
        index = 7;
        break;
      case "bonusStart":
        index = 1;
        break;
      case "fireBlast":
        index = 4;
        break;
      case "fireWhoose":
        index = 5;
        break;
      case "bonusSpin":
        index = 11;
        break;
      case "cwin":
        index = 12;
        break;
    }
    StopWLAaudio();
    audioPlayer_wl.clip = clips[index];
    audioPlayer_wl.Play();

  }

  internal void PlayBonusAudio(string type)
  {
    audioPlayer_wl.loop = false;
    int index = 0;
    switch (type)
    {
      case "win":
        index = 0;
        break;
      case "lose":
        index = 1;
        break;
      case "cycleSpin":
        index = 2;
        break;
    }
    StopBonusAaudio();
    audioPlayer_Bonus.clip = Bonusclips[index];
    audioPlayer_Bonus.Play();

  }

  internal void PlayButtonAudio()
  {
    audioPlayer_button.clip = clips[3];
    audioPlayer_button.Play();
  }

  internal void PlaySpinButtonAudio()
  {
    audioSpin_button.Play();
  }

  internal void StopWLAaudio()
  {
    audioPlayer_wl.Stop();
    audioPlayer_wl.loop = false;
  }

  internal void StopBonusAaudio()
  {
    audioPlayer_Bonus.Stop();
    audioPlayer_Bonus.loop = false;
  }

  internal void StopBgAudio()
  {
    bg_adudio.Stop();
  }

  internal void ToggleMute(bool toggle, string type = "all")
  {
    switch (type)
    {
      case "bg":
        SetSourceMute(bg_adudio, toggle);
        SetSourceMute(bg_audioBonus, toggle);
        break;
      case "button":
        SetSourceMute(audioPlayer_button, toggle);
        SetSourceMute(audioSpin_button, toggle);
        break;
      case "wl":
        SetSourceMute(audioPlayer_wl, toggle);
        SetSourceMute(audioPlayer_Bonus, toggle);
        break;
      case "all":
        ToggleMute(toggle, "bg");
        ToggleMute(toggle, "button");
        ToggleMute(toggle, "wl");
        break;
    }
  }

}
