using UnityEngine;
using System.Collections;

/// <summary>
/// How a PlayerUnit has its active turn selected. Ie player input, AI, etc.
/// </summary>
public enum Drivers
{
	None,
	/// <summary>
	/// Human player selects the turn
	/// </summary>
	Human,
	/// <summary>
	/// AI inference for the turn
	/// </summary>
	Computer,
	/// <summary>
	/// Trainable AI where AI is trained in the editor mode with UnityML
	/// </summary>
	ReinforcementLearning, 
	/// <summary>
	/// Trainable AI where action is supplied through DeepMind stle black box Unity env
	/// </summary>
	BlackBoxRL,
	/// <summary>
	/// Trainable AI where action is supplied through the UnityML Python Gym API interface
	/// </summary>
	ReinforcementLearningGym,
}
