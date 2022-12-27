using UnityEngine;
using System.Collections;

/// <summary>
/// PlayerUnit relationship status with another PlayerUnit. Ie allied, hostile, etc.
/// </summary>
public enum Alliances
{
    /// <summary>
    /// no alliance
    /// </summary>
    None = 0,

    /// <summary>
    /// units that are not on your side but not hostile to you. may or may not be hostile to other units or can turn hostile
    /// </summary>
    Neutral = 1 << 0,

    /// <summary>
    /// units on your side and under your control
    /// </summary>
    Hero = 1 << 1,

    /// <summary>
    /// units hostile to you
    /// </summary>
    Enemy = 1 << 2,

    /// <summary>
    /// units on your side but not under your control
    /// </summary>
    Allied = 1 << 3
}