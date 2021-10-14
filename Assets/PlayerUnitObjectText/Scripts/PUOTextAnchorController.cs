using UnityEngine;


namespace PlayerUnitObjectText.PUOText
{

	public class PUOTextAnchorController : MonoBehaviour
	{
		public GameObject targetGameObject = null;
		public Camera mainCamera;
		public float ageInSeconds; 

		public bool puoTextShown { get { return puoTextController.puoTextShown; } }
		protected PUOTextController puoTextController;

		void Awake()
		{
			puoTextController = GetComponentInChildren<PUOTextController>();
		}

		void Update()
		{
			ageInSeconds += Time.deltaTime;

			if (targetGameObject != null)
			{
				UpdatePosition();
			}

			if (!puoTextController.puoTextShown)
			{
				gameObject.SetActive(false);
			}
		}

		public void ShowPUOText(GameObject targetGameObject, PUOTextType puoTextType, string puoText)
		{
			this.targetGameObject = targetGameObject;
			UpdatePosition();
			puoTextController.ShowPUOText(puoTextType, puoText);

			gameObject.SetActive(true);
		}

		public void ShowPUOText(GameObject targetGameObject, PUOTextType puoTextType, int puoTextNumber)
		{
			this.targetGameObject = targetGameObject;
			UpdatePosition();
			puoTextController.ShowPUOText(puoTextType, puoTextNumber);

			gameObject.SetActive(true);
		}

		public void ResetForReuse()
		{
			// Reset our age counter for damage pooling.
			ageInSeconds = 0f;
		}

		protected void UpdatePosition()
		{
			transform.position = mainCamera.WorldToScreenPoint(targetGameObject.transform.position);
		}
	}
}