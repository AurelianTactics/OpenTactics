using UnityEngine;
using System.Collections;

public enum Alliances
{
	None = 0, //no alliance
	Neutral = 1 << 0, //units that are not on your side but not hostile to you. may or may not be hostile to other units or can turn hostile
	Hero = 1 << 1, //units on your side and under your control
	Enemy = 1 << 2, //units hostile to you
    Allied = 1 << 3 //units on your side but not under your control
}