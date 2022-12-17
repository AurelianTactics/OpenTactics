using UnityEngine;
using System.Collections;

public enum Drivers
{
	None,
	Human,
	Computer,
	ReinforcementLearning, //trainable AI with UnityML
	BlackBoxRL //blackbox RL mode, relies on DeepMind style black box Unity env
}
