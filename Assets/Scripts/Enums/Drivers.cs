using UnityEngine;
using System.Collections;

/// <summary>
/// How a PlayerUnit has its active turn selected. Ie player input, AI, etc.
/// </summary>
public enum Drivers
{
	None,
	Human,
	Computer,
	ReinforcementLearning, //trainable AI with UnityML
	BlackBoxRL //blackbox RL mode, relies on DeepMind style black box Unity env
}
