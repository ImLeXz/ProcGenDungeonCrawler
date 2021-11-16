using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ATDungeon.UI
{
    public class BarUI : MonoBehaviour
    {
        [Header("Bar UI")]
        [SerializeField]
        private TextMeshProUGUI barTxt;
        [SerializeField]
        private Slider barSlider;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetupBarSlider(float v)
        {
            barSlider.maxValue = v;
        }

        public void UpdateBarUI(float v, string txt)
        {
            barTxt.text = (txt + (float)Math.Round(v, 2) + " / " + barSlider.maxValue);
            barSlider.value = v;
        }
    }
}
