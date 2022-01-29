using System;

public class LivingStateManager
{
    private event Action<bool> livingState;
    private static LivingStateManager instance;
    public static LivingStateManager Instance => instance ??= new LivingStateManager();

    public static void TriggerLifeChanges(bool isLiving)
    {
        Instance.livingState?.Invoke(isLiving);
    }

    public static void RegisterForLifeStateChanges(Action<bool> callback)
    {
        Instance.livingState += callback;
    }

    public static void UnRegisterForLifeStateChanges(Action<bool> callback)
    {
        Instance.livingState -= callback;
    }

    public static void CleanAllLivingEvents()
    {
        Instance.livingState = null;
    }
}


