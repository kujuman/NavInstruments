using System;
using UnityEngine;
using KSP;
using NavUtilLib;
using var = NavUtilLib.GlobalVariables;

namespace GUIInstruments
{

    public class HSI_Display :MonoBehaviour
    {
        private Rect windowPosition;
        private RenderTexture rt;

        private GameObject go;
        private Camera c;


        public void Activate(bool state)
        {
            if (state)
            {
                RenderingManager.AddToPostDrawQueue(3, OnDraw);

                //Scale = NavUtilLib.GlobalVariables.Settings.hsiScale;
                windowPosition = NavUtilLib.GlobalVariables.Settings.hsiPosition;

                rt = new RenderTexture((int)windowPosition.width, (int)windowPosition.height, 24, RenderTextureFormat.ARGB32);

                Debug.Log("ILS: Starting systems...");
                if (!var.Settings.navAidsIsLoaded)
                    var.Settings.loadNavAids();

                if (!var.Materials.isLoaded)
                    var.Materials.loadMaterials();

                //if (!var.Audio.isLoaded)
                var.Audio.initializeAudio();

                //ConfigureCamera();

                Debug.Log("ILS: Systems started successfully!");
            }
            else
            {
                state = false;
                RenderingManager.RemoveFromPostDrawQueue(3, OnDraw); //close the GUI
                NavUtilLib.GlobalVariables.Settings.hsiPosition = windowPosition;
            }
        }

        private void ConfigureCamera()
        {
            c = new Camera();
            Debug.Log("HSI: 5");

            c.cullingMask &= ~(1 << 16 | 1 << 20);
            Debug.Log("HSI: 5.1");
            c.enabled = true;
            Debug.Log("HSI: 5.2");
            c.aspect = 1;
            Debug.Log("HSI: 5.3");



            Debug.Log("HSI: 5.2");

            c.targetTexture = rt;
            Debug.Log("HSI: 7");
        }

        private void OnDraw()
        {
            if ((windowPosition.xMin + windowPosition.width) < 20) windowPosition.xMin = 20 - windowPosition.width;
            if (windowPosition.yMin + windowPosition.height < 20) windowPosition.yMin = 20 - windowPosition.height;
            if (windowPosition.xMin > Screen.width - 20) windowPosition.xMin = Screen.width - 20;
            if (windowPosition.yMin > Screen.height - 20) windowPosition.yMin = Screen.height - 20;
            windowPosition = GUI.Window(1, windowPosition, OnWindow, "Horizontal Situation Indicator");
        }

        private void OnWindow(int WindowID)
        {
            Debug.Log("HSI: 1");
            Debug.Log("HSI: " + CameraManager.Instance.currentCameraMode.ToString());

            if (CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.Flight)
            {
                Debug.Log("HSI: 2");
                RenderTexture prior = RenderTexture.active;



                Debug.Log("HSI: 4");
                if(!rt.IsCreated()) rt.Create();

                Debug.Log("HSI: 3");
                rt.DiscardContents();
                Debug.Log("HSI: 5");

                RenderTexture.active = rt;

                Debug.Log("HSI: 6");
                //UnityEngine.Graphics.SetRenderTarget(rt);
                Debug.Log("HSI: 7");

                NavUtilLib.GlobalVariables.DisplayData.DrawHSI(rt, windowPosition.width / windowPosition.height);


                UnityEngine.Graphics.SetRenderTarget(rt);




                Debug.Log("HSI: 9");

                //GUI.DrawTexture(new Rect(0, 0, rt.width, rt.height), rt, ScaleMode.ScaleToFit, true);
                GUILayout.Box(rt);

                Debug.Log("HSI: 10");
                Debug.Log("HSI: 8");
                RenderTexture.active = prior;
                Debug.Log("HSI: 11");
                GUI.DragWindow();
                Debug.Log("HSI: 12");
            }
        }

        //private void OnWindow(int WindowID)
        //{
        //    GUI.DrawTexture(new Rect(0, 0, Resources.hsi_back.width * Scale, Resources.hsi_back.height * Scale), Resources.hsi_back, ScaleMode.ScaleToFit, true); //background

        //    drawCenterRotatedImage((float)makeAngle0to360(360 - FlightGlobals.ship_heading), Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_heading_card, 0, 0); //compass

        //    drawCenterRotatedImage((float)makeAngle0to360(360 - FlightGlobals.ship_heading) + (float)makeAngle0to360(bearingToBeacon), Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_NDB_needle, 0, 0); //NDB

        //    drawCenterRotatedImage((float)makeAngle0to360(360 - FlightGlobals.ship_heading) + (float)rwyList[rwyIdx].hdg, Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_course_needle, 0, 0); //course

        //    float deviationCorrection;
        //    deviationCorrection = localizerDeviation * 50 * Scale;
        //    deviationCorrection = Math.Min(150 * Scale, deviationCorrection);
        //    deviationCorrection = Math.Max(-150 * Scale, deviationCorrection);

        //    drawCenterRotatedImage((float)makeAngle0to360(360 - FlightGlobals.ship_heading) + (float)rwyList[rwyIdx].hdg, Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_course_deviation_needle, deviationCorrection, 0); //localizer needle

        //    GUI.DrawTexture(new Rect(0, 0, Resources.hsi_overlay.width * Scale, Resources.hsi_overlay.height * Scale), Resources.hsi_overlay, ScaleMode.ScaleToFit, true);//overlay

        //    deviationCorrection = glideslopeDeviation * 200;
        //    deviationCorrection = Math.Min(140, deviationCorrection);
        //    deviationCorrection = Math.Max(-140, deviationCorrection);
        //    //deviationCorrection *= -1;

        //    GUI.DrawTexture(new Rect(0, (329 + deviationCorrection) * Scale, Resources.hsi_GS_pointer.width * Scale, Resources.hsi_GS_pointer.height * Scale), Resources.hsi_GS_pointer, ScaleMode.ScaleToFit, true);//glideslope pointer

        //    drawNumbers(50, 473, (float)Math.Min(999, Math.Round(distanceToBeacon / 1000f, 1)), false); //draw DME
        //    drawNumbers(30, 40, (float)Math.Round(rwyList[rwyIdx].hdg), true); //draw Course
        //    drawNumbers(450, 10, (float)Math.Round(FlightGlobals.ship_heading), true); //draw heading
        //    drawNumbers(450, 39, (float)Math.Round(makeAngle0to360(bearingToBeacon)), true); //draw bearing


        //    GUI.DragWindow();
        //}

        //private void drawNumbers(float leftX, float topY, float numToDisplay, bool isHeading)
        //{
        //    Texture2D img = Resources.base_digit_strip;

        //    topY *= Scale;
        //    leftX *= Scale;

        //    int tenths = (int)((numToDisplay * 10) % 10);
        //    int ones = (int)Math.Abs(numToDisplay / 1 % 10);
        //    int tens = (int)Math.Abs(numToDisplay / 10 % 10);
        //    int hundreds = (int)Math.Abs(numToDisplay / 100 % 10);

        //    float digitStart = 0;

        //    float offSet = 0;
        //    float width = 0;

        //    if (numToDisplay >= 100 || isHeading)
        //    {
        //        offSet = 14f * hundreds / 146f;
        //        width = 14f / 146f;
        //        GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

        //        digitStart += 14 * Scale ;

        //        offSet = 14 * tens / 146f;
        //        GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

        //        digitStart += 14 * Scale;

        //        offSet = 14 * ones / 146f;
        //        GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

        //        return;
        //    }

        //    if (numToDisplay < 100)
        //    {
        //        width = 14 / 146f;

        //        if (tens != 0)
        //        {
        //            offSet = 14 * tens / 146f;
        //            GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));
        //        }

        //        digitStart += 14 * Scale;

        //        offSet = 14 * ones / 146f;
        //        GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

        //        digitStart += 14 * Scale;

        //        width = 2 / 146f;
        //        offSet = 142 / 146f;
        //        GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 2* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

        //        digitStart += 2 * Scale;

        //        width = 14 / 146f;
        //        offSet = 14 * tenths / 146f;
        //        GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

        //        return;
        //    }
        //}

    }
}