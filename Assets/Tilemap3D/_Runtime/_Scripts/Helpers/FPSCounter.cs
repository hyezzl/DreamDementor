using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Tilemap3D
{
    public class FPSCounter : MonoBehaviour
    {
        // framerate is calculated using this interval
        public float checkInterval = 1f;
        public float currentFrameRate = 0f;

        private int currentPassedFrames = 0;
        private float currentPassedTime = 0f;
        private string currentFrameRateString = "";

        private void Update()
        {
            currentPassedFrames++;
            currentPassedTime += Time.deltaTime;

            // if passed time has reached our checkInterval then recalculate framerate
            if (currentPassedTime >= checkInterval)
            {
                currentFrameRate = currentPassedFrames / currentPassedTime;

                // round to 2 decimal places
                currentFrameRate *= 100f;
                currentFrameRate = (int)currentFrameRate;
                currentFrameRate /= 100f;

                currentFrameRateString = currentFrameRate.ToString();

                currentPassedTime = 0f;
                currentPassedFrames = 0;
            }
        }

        private void OnGUI()
        {
            GUI.contentColor = Color.black;

            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontSize = 20;
            guiStyle.normal.textColor = Color.white;

            float labelSize = 80f;
            float offset = 2f;

            GUI.Label(new Rect(Screen.width - labelSize + offset, offset, labelSize, 30f), currentFrameRateString, guiStyle);

            GUI.contentColor = Color.white;

            GUI.Label(new Rect(Screen.width - labelSize, 0f, labelSize, 30f), currentFrameRateString, guiStyle);
        }
    }
}