using UnityEngine;

[DisallowMultipleComponent]
public class StaffCooldowns : MonoBehaviour
{
    private float beginnerRemaining;
    private float advancedRemaining;
    private float masterRemaining;

    private void Update()
    {
        beginnerRemaining = TickDown(beginnerRemaining);
        advancedRemaining = TickDown(advancedRemaining);
        masterRemaining = TickDown(masterRemaining);
    }

    public bool IsOnCooldown(StaffTier tier)
    {
        return GetRemaining(tier) > 0f;
    }

    public float GetRemaining(StaffTier tier)
    {
        switch (tier)
        {
            case StaffTier.Beginner:
                return beginnerRemaining;
            case StaffTier.Advanced:
                return advancedRemaining;
            case StaffTier.Master:
                return masterRemaining;
            default:
                return 0f;
        }
    }

    public void StartCooldown(StaffTier tier, float duration)
    {
        duration = Mathf.Max(0f, duration);

        switch (tier)
        {
            case StaffTier.Beginner:
                beginnerRemaining = duration;
                break;
            case StaffTier.Advanced:
                advancedRemaining = duration;
                break;
            case StaffTier.Master:
                masterRemaining = duration;
                break;
        }
    }

    private float TickDown(float remaining)
    {
        if (remaining <= 0f)
        {
            return 0f;
        }

        return Mathf.Max(0f, remaining - Time.deltaTime);
    }
}
