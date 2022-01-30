using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ValueEvent : UnityEvent<float> { };

public class SoundPlayer : MonoBehaviour
{
    [SerializeField]
    public UnityEvent JumpSoundEvent;

    [SerializeField]
    public UnityEvent WallJumpSoundEvent;

    [SerializeField]
    public UnityEvent SwitchSoundEvent;

    [SerializeField]
    public UnityEvent CannotSwitchSoundEvent;

    [SerializeField]
    public UnityEvent GroundSoundEvent;

    [SerializeField]
    public ValueEvent WalkEvent;

    [SerializeField]
    public UnityEvent PermaDieSoundEvent;
}
