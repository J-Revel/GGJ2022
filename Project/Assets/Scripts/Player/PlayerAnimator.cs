using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private AnimatedSprite sprite;

    public enum State {Idle, Walk, Jump, Float, Fall, Wall}
    private bool isLiving = true;
    private bool permaDeath;

    private string GetStateId(bool isAlive, State state)
    {
        if(isAlive)
        {
            return  $"{state} Alive";
        }
        else
        {
            return "Dead";
        }
    }

    private State state;
    private float scaleAnimationSpeed = 3f;
    private float scaleAnimationTime = 0f;

    public State SpriteState
    {
        get => state;
        set
        {
            if(value != state)
            {
                this.SwitchState(value);
            }
        }
    }

    public void UpdatePermaRisk(bool isObserved, float timeRatio)
    {
        Debug.Log("Risk : " + timeRatio);
    }

    private void SwitchState(State state)
    {
        if (isLiving)
        {
            sprite.SelectAnim(GetStateId(isLiving, state));
        }
        this.state = state;
    }

    public void TriggerPermaDeath()
    {
        permaDeath = true;
    }


    public void Awake()
    {
        LivingStateManager.RegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnDestroy()
    {
        LivingStateManager.UnRegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnLifeStateChanges(bool isLiving)
    {
        this.isLiving = isLiving;
        if (this.isLiving)
        {
            this.UpdatePermaRisk(false, 0f);
            this.CleanRendererAnimation();
        }

        sprite.SelectAnim(GetStateId(this.isLiving, state));
    }

    private void CleanRendererAnimation()
    {
        this.transform.localScale = Vector3.one;
        this.scaleAnimationTime = 0f;
    }

    private void RendererAnimation()
    {
        scaleAnimationTime += Time.deltaTime * scaleAnimationSpeed;
        float scaleXAnimationValue = (1f + Mathf.Sin(scaleAnimationTime)) / 2f;
        float scaleYAnimationValue = (1f + Mathf.Sin(scaleAnimationTime + 0.5f)) / 2f;

        float scaleX = 0.9f + 0.15f * scaleXAnimationValue;
        float scaleY = 0.85f + 0.25f * scaleYAnimationValue;
        this.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    private void Update()
    {
        if (isLiving || permaDeath)
        {
            return;
        }

        RendererAnimation();
    }

    public void DisplayCantSwitchFeedback()
    {
        Debug.Log("I CANT SWITCH ONO");
    }
}
