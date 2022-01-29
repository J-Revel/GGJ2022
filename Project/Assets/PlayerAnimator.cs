using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private AnimatedSprite sprite;

    public enum State {Idle, Walk, Jump, Float, Fall, Wall}
    private bool isLiving = true;

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

    private void SwitchState(State state)
    {
        this.state = state;
        if(isLiving) sprite.SelectAnim(GetStateId(isLiving,state));
    }

    // Start is called before the first frame update
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
        sprite.SelectAnim(GetStateId(this.isLiving, state));
    }
}
