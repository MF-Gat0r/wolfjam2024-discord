using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Name: Cmd Coldown
/// Purpose: Set twitch command cooldowns to prevent event spamming and overloading the api
/// Author(s): Katie Hellmann, GucioDevs Unity3D Quick Tips: Cooldown Timers, Gator Flack
/// </summary>

public class CmdCooldown : MonoBehaviour
{
    [SerializeField] private int maxBoostCount = 4; // The number of boosts needed before activation
    [SerializeField] private float boostCooldownTime = 3f; // Cooldown per !boost usage
    private int currentBoostCount = 0; // Tracks how many boosts have been used
    private bool canUseBoost = true; 

    public event Action OnBoostMeterFilled; // Event triggered when boost meter is full

    private void Start()
    {
        // Reset meter on start
        currentBoostCount = 0;
    }

    public void OnChatMessage(string pChatter, string pMessage)
    {
        if (pMessage.Equals("!boost", StringComparison.OrdinalIgnoreCase))
        {
            if (canUseBoost)
            {
                FillBoostMeter();
            }
            else
            {
                Debug.Log($"Boost is on cooldown! Please wait {boostCooldownTime} seconds.");
            }
        }
    }

    /// Adds to the boost meter and checks if the meter is full.
    private void FillBoostMeter()
    {
        currentBoostCount++;
        canUseBoost = false; // Temporarily disable boosting to prevent spam
        Debug.Log($"Boost used! ({currentBoostCount}/{maxBoostCount})");

        if (currentBoostCount >= maxBoostCount)
        {
            ActivateBoost();
            currentBoostCount = 0; // Reset meter
        }

        StartCoroutine(ResetBoostCooldown());
    }

    private void ActivateBoost()
    {
        Debug.Log("BOOST ACTIVATED!");
        OnBoostMeterFilled?.Invoke(); // Trigger for event 
    }
    /// Waits for cooldown before allowing another !boost command.

    private IEnumerator ResetBoostCooldown()
    {
        yield return new WaitForSeconds(boostCooldownTime);
        canUseBoost = true;
    }

    /// Allows the streamer to modify the required boosts dynamically.
    public void SetBoostRequirement(int newRequirement)
    {
        if (newRequirement > 0)
        {
            maxBoostCount = newRequirement;
            Debug.Log($"Boost requirement updated! Now takes {maxBoostCount} boosts to activate.");
        }
    }
}
