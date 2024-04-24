using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdultLink
{
	
public class HealBar : MonoBehaviour {

        public Material originalMat; // Assign the original material here in the Inspector
        private Material mat; // This will be the instance material for this object
        public float fillTime = 5f;
	public float decreaseTime = 2.5f;
	private float increaseAmount;
	private float decreaseAmount;
	private float fillPercentage;
	public Text countDowntext;

	public Color defaultTextColor;
	public Color highlightColor;
	public Text descriptionText;
	private float initialFillPercentage;
        private float targetFillPercentage;



        private void Start() {
            // Instantiate a new material based on the original
            mat = new Material(originalMat);

            GetComponentInChildren<Image>().material = mat;

            initialFillPercentage = mat.GetFloat("_Fillpercentage");
		fillPercentage = initialFillPercentage;
		mat.SetFloat("_Fillpercentage", fillPercentage);
		increaseAmount = 1f / fillTime * Time.fixedDeltaTime;
		decreaseAmount = 1f / decreaseTime * Time.fixedDeltaTime;
            targetFillPercentage = initialFillPercentage;
        }
        void Update()
        {
            // Gradually move the fillPercentage towards the targetFillPercentage
            if (fillPercentage != targetFillPercentage)
            {
                fillPercentage = Mathf.MoveTowards(fillPercentage, targetFillPercentage, increaseAmount);
                mat.SetFloat("_Fillpercentage", fillPercentage);
            }
        }

        public void changeFill(float amount)
        {

            targetFillPercentage = Mathf.Clamp(amount, 0f, 1f);

        }

        private void setTextColor() {
		descriptionText.color = highlightColor;
	}

	private void resetTextColor() {
		descriptionText.color = defaultTextColor;
	}

	private void OnApplicationQuit() {
		mat.SetFloat("_Fillpercentage", initialFillPercentage);
	}

}

}
