using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PlayerUnitObjectText.PUOText
{
	/// <summary>
	/// Shows text over PlayerUnitObjects 
	/// </summary>
	public class PUOTextOverlayCanvasController : MonoBehaviour
	{
		public static PUOTextOverlayCanvasController instance;
		public Camera mainCamera; 
		public bool poolDamage; 

		public PUOTextAnchorController puoTextAnchorPrefab;
		
		protected Dictionary<PUOTextType, List<PUOTextAnchorController>> puoTextAnchorDictionary;

		void Awake()
		{
			if (instance == null)
				instance = this;
			else
			{
				Debug.LogWarning("Detected 2 PUOTextOverlayCanvasControllers. The static instance variable is still set to the first instance. Double click this warning and read the related comment.");
			}

			InitializeDictionary();
		}

		void Start()
		{
			if (mainCamera == null)
			{
				// Turn ourselves off since we're invalid.
				gameObject.SetActive(false);

				string errorMessage = "You must set PUOTextOverlayCanvasController.mainCamera to the main camera of your scene. Disabling PUOTextOverlayCanvas.";
				throw new System.InvalidOperationException(errorMessage);
			}
		}

		public void ShowPUOText(GameObject targetGameObject, PUOTextType puoTextType, string puoText)
		{
			PUOTextAnchorController puoTextAnchorInstance;
			puoTextAnchorInstance = EnsurePUOTextAnchor(targetGameObject, puoTextType);
			puoTextAnchorInstance.mainCamera = mainCamera;
			puoTextAnchorInstance.transform.SetParent(transform);
			puoTextAnchorInstance.ShowPUOText(targetGameObject, puoTextType, puoText);
		}

				public void ShowPUOText(GameObject targetGameObject, PUOTextType puoTextType, int puoTextNumber)
		{
			PUOTextAnchorController puoTextAnchorInstance;
			puoTextAnchorInstance = EnsurePUOTextAnchor(targetGameObject, puoTextType);
			puoTextAnchorInstance.mainCamera = mainCamera;
			puoTextAnchorInstance.transform.SetParent(transform);
			puoTextAnchorInstance.ShowPUOText(targetGameObject, puoTextType, puoTextNumber);
		}

		/// <summary>
		/// This function looks for a PUOTextAnchor of the right type. If one doesn't exist,
		/// it creates one.
		/// </summary>
		/// <param name="targetGameObject">Which game object is this text going to be attached to. (Used when poolDamage is enabled)</param>
		/// <param name="puoTextType">The type of text</param>
		/// <returns></returns>
		protected PUOTextAnchorController EnsurePUOTextAnchor(GameObject targetGameObject, PUOTextType puoTextType)
		{
			PUOTextAnchorController textAnchor;

			if (poolDamage)
			{
				// Look for a currently playing animation that is young enough so we can add our numbers to them.
				textAnchor = puoTextAnchorDictionary[puoTextType].Find(x => x.puoTextShown && x.targetGameObject == targetGameObject && x.ageInSeconds < 1f);
				if (textAnchor != null)
				{
					return textAnchor;
				}
			}

			// Look for a puoText that has stopped playing
			textAnchor = puoTextAnchorDictionary[puoTextType].Find(x => !x.puoTextShown);
			if(textAnchor != null)
			{
				textAnchor.ResetForReuse();
				return textAnchor;
			}

			// Otherwise we have to create a new one from our prefab.
			textAnchor = Instantiate(puoTextAnchorPrefab) as PUOTextAnchorController;
			textAnchor.name = string.Format("{0} {1}", puoTextAnchorPrefab.name, puoTextType);
			puoTextAnchorDictionary[puoTextType].Add(textAnchor);

			return textAnchor;
		}

		/// <summary>
		/// Sets up our puoTextAnchorDictionary for our PUOText object pool. 
		/// </summary>
 		protected void InitializeDictionary()
		{
			puoTextAnchorDictionary = new Dictionary<PUOTextType, List<PUOTextAnchorController>>();

			// Get a list of all the entries in PUOTextType
			Array puoTextTypes = Enum.GetValues(typeof(PUOTextType));

			// Initialize a list for each entry so we don't have to check if one's been created
			// every time we want to show puo text.
			foreach (object puoTextType in puoTextTypes)
			{
				if (puoTextAnchorDictionary.ContainsKey((PUOTextType)puoTextType))
				{
					// Turn ourselves off since we're invalid.
					gameObject.SetActive(false);

					string errorMessage = string.Format("The enumeration PUOTextType must have unique values. [PUOTextType.{0}] has the same value as another entry.", puoTextType);
					throw new InvalidOperationException(errorMessage);
				}

				puoTextAnchorDictionary[(PUOTextType)puoTextType] = new List<PUOTextAnchorController>();
			}
		}
	}
}
