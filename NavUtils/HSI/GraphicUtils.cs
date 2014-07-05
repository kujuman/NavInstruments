using System;
using UnityEngine;
using KSP;

namespace NavUtilities
{
    public static class GraphicUtils
    {


        public static Texture2D OnWindow(float brightness)
        {
            float Scale = 1;

            Texture2D displayTex = new Texture2D(640,640);
            Color backgroundColor;
            backgroundColor.a = 1;
            backgroundColor.b = 0.15f;
            backgroundColor.r = 0.15f;
            backgroundColor.g = 0.15f;

            backgroundColor.b = brightness;
            backgroundColor.r = brightness;
            backgroundColor.g = brightness;

            var fillColorArray =  displayTex.GetPixels();

            for (var i = 0; i < fillColorArray.Length; ++i)
            {
                fillColorArray[i] = backgroundColor;
            }


            displayTex.Apply();

            return displayTex;


            /*




            GUI.DrawTexture(new Rect(0, 0, Resources.hsi_back.width * Scale, Resources.hsi_back.height * Scale), Resources.hsi_back, ScaleMode.ScaleToFit, true); //background

            drawCenterRotatedImage((float)Utils.makeAngle0to360(360 - FlightGlobals.ship_heading), Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_heading_card, 0, 0); //compass

            drawCenterRotatedImage((float)Utils.makeAngle0to360(360 - FlightGlobals.ship_heading) + (float)Utils.makeAngle0to360(bearingToBeacon), Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_NDB_needle, 0, 0); //NDB

            drawCenterRotatedImage((float)Utils.makeAngle0to360(360 - FlightGlobals.ship_heading) + (float)rwyList[rwyIdx].hdg, Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_course_needle, 0, 0); //course

            float deviationCorrection;
            deviationCorrection = localizerDeviation * 50 * Scale;
            deviationCorrection = Math.Min(150 * Scale, deviationCorrection);
            deviationCorrection = Math.Max(-150 * Scale, deviationCorrection);

            drawCenterRotatedImage((float)Utils.makeAngle0to360(360 - FlightGlobals.ship_heading) + (float)rwyList[rwyIdx].hdg, Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_course_deviation_needle, deviationCorrection, 0); //localizer needle

            GUI.DrawTexture(new Rect(0, 0, Resources.hsi_overlay.width * Scale, Resources.hsi_overlay.height * Scale), Resources.hsi_overlay, ScaleMode.ScaleToFit, true);//overlay

            deviationCorrection = glideslopeDeviation * 200;
            deviationCorrection = Math.Min(140, deviationCorrection);
            deviationCorrection = Math.Max(-140, deviationCorrection);
            //deviationCorrection *= -1;

            GUI.DrawTexture(new Rect(0, (329 + deviationCorrection) * Scale, Resources.hsi_GS_pointer.width * Scale, Resources.hsi_GS_pointer.height * Scale), Resources.hsi_GS_pointer, ScaleMode.ScaleToFit, true);//glideslope pointer

            //drawNumbers(50, 473, (float)Math.Min(999, Math.Round(distanceToBeacon / 1000f, 1)), false); //draw DME
            //drawNumbers(30, 40, (float)Math.Round(rwyList[rwyIdx].hdg), true); //draw Course
            //drawNumbers(450, 10, (float)Math.Round(FlightGlobals.ship_heading), true); //draw heading
            //drawNumbers(450, 39, (float)Math.Round(makeAngle0to360(bearingToBeacon)), true); //draw bearing
             * 
             * */
        }

        public static void drawCenterRotatedImage(float heading, float width, float height, Texture2D img, float xOffset, float yOffset)
        {
            float Scale = 1;

            float w = img.width;
            float h = img.height;

            float x = (width - w) * Scale / 2 + xOffset * Scale;
            float y = (height - h) * Scale / 2 + yOffset * Scale;
            float widthP = w * Scale;
            float heightP = h * Scale;

            Vector2 pivotPoint = new Vector2(width * Scale / 2, height * Scale / 2);

            GUIUtility.RotateAroundPivot(heading, pivotPoint);
            GUI.DrawTexture(new Rect(x, y, widthP, heightP), img, ScaleMode.ScaleToFit, true);
            GUI.matrix = Matrix4x4.identity;
        }
    }
}
