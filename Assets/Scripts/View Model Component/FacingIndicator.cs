using UnityEngine;
using System.Collections;

/// <summary>
/// I think controls the little dot to indicate which way to face
/// </summary>
public class FacingIndicator : MonoBehaviour 
{
	[SerializeField] Renderer[] directions;
	[SerializeField] Material normal;
	[SerializeField] Material selected;
	
	public void SetDirection (Directions dir)
	{
		int index = (int)dir;
		for (int i = 0; i < 4; ++i)
			directions[i].material = (i == index) ? selected : normal;
	}
}
