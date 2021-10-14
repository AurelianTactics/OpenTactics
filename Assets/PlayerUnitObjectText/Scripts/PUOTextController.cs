using UnityEngine;
using UnityEngine.UI;

namespace PlayerUnitObjectText.PUOText
{

 	public class PUOTextController : MonoBehaviour
	{
		public PUOTextType puoTextType;
		public bool puoTextShown; 

		protected Text puoText;
		protected int puoTextNumber;
		protected Animator puoTextAnimator;
		protected int puoTextTypeHash = Animator.StringToHash("PUOTextType");

		void Awake()
		{
			puoText = GetComponent<Text>();
			puoTextAnimator = GetComponent<Animator>();
		}

		void Update()
		{
			SetAnimatorsPUOTextType();

			if (!puoTextShown)
			{
				gameObject.SetActive(false);
			}
		}

		public void ShowPUOText(PUOTextType puoTextType, string puoText)
		{
			this.puoTextShown = true;
			this.puoText.text = puoText;
			this.puoTextType = puoTextType;

			gameObject.SetActive(true);
		}


		public void ShowPUOText(PUOTextType puoTextType, int puoTextNumber)
		{
			if (puoTextShown)
				this.puoTextNumber += puoTextNumber;
			else
				this.puoTextNumber = puoTextNumber;

			ShowPUOText(puoTextType, this.puoTextNumber.ToString());
		}

		protected void SetAnimatorsPUOTextType()
		{
			if (puoTextAnimator.GetInteger(puoTextTypeHash) != (int)puoTextType)
			{
				puoTextAnimator.SetInteger(puoTextTypeHash, (int)puoTextType);
			}
		}
	}
}