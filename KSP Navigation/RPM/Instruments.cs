using System;
using System.Collections.Generic;
using UnityEngine;
using KSP;
using NavUtilLib;
using var = NavUtilLib.GlobalVariables;

namespace KSFRPMHSI
{
    public class KSF_MLS : InternalModule
    {
        //bool doneLoading = false;

        [KSPField(isPersistant = false)]
        int btnPrevGS = 1;

        [KSPField(isPersistant = false)]
        int btnPrevRwy = 5;

        [KSPField(isPersistant = false)]
        int btnNextGS = 0;

        [KSPField(isPersistant = false)]
        int btnNextRwy = 6;

        [KSPField(isPersistant = false)]
        int btnDefaultRwyGS = 3;

        public bool DrawMLS(RenderTexture screen, float aspectRatio)
        {
            NavUtilLib.GlobalVariables.FlightData.updateNavigationData();

            NavUtilLib.DisplayData.DrawHSI(screen, aspectRatio);


            NavUtilLib.TextWriter.addTextToRT(screen, "Runway: " + NavUtilLib.GlobalVariables.FlightData.selectedRwy.ident, new Vector2(20, screen.height - 40), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            NavUtilLib.TextWriter.addTextToRT(screen, "Glideslope: " + string.Format("{0:F1}", NavUtilLib.GlobalVariables.FlightData.selectedGlideSlope) + "°  Elevation: " + string.Format("{0:F0}", NavUtilLib.GlobalVariables.FlightData.selectedRwy.altMSL) + "m", new Vector2(20, screen.height - 64), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);

            NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.Utils.makeAngle0to360(FlightGlobals.ship_heading), true).ToString(), new Vector2(584, screen.height - 102), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.Utils.makeAngle0to360(NavUtilLib.GlobalVariables.FlightData.bearing), true).ToString(), new Vector2(584, screen.height - 131), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.Utils.makeAngle0to360(NavUtilLib.GlobalVariables.FlightData.selectedRwy.hdg), true).ToString(), new Vector2(35, screen.height - 124), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.GlobalVariables.FlightData.dme / 1000, false).ToString(), new Vector2(45, screen.height - 563), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);

            return true;
        }

        [Obsolete("Text now rendered on display in DrawMLS()")]
        public string pageAuthor(int screenWidth, int screenHeight)
        {
            //if(!(var.FlightData.GetLastNavUpdateUT() + 0.05 > Planetarium.GetUniversalTime()))

                var.FlightData.updateNavigationData();

            string output;

            output = "  Runway: " + var.FlightData.selectedRwy.ident + Environment.NewLine +
                "  Glideslope: " + string.Format("{0:F1}", var.FlightData.selectedGlideSlope) + "°"
            //"  GS Alt MSL: " + Utils.CalcSurfaceAltAtDME((float)dme,Rwy.body,(float)glideSlope,(float)Rwy.altMSL) +"m"
                + Environment.NewLine +
                "                                     [@x-4][@y7]" + NavUtilLib.Utils.numberFormatter(FlightGlobals.ship_heading, true) + Environment.NewLine +
                "   [@x-4][@y4]" + NavUtilLib.Utils.numberFormatter(var.FlightData.selectedRwy.hdg, true) + "                               " + NavUtilLib.Utils.numberFormatter((float)var.FlightData.bearing, true) + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + "   [@y16]" + NavUtilLib.Utils.numberFormatter((float)var.FlightData.dme / 1000f, false) + Environment.NewLine
                + Environment.NewLine
                + " [@x-5][@y8]               |    Runway    |" + Environment.NewLine
                + " [@x-5]               | Prev | Next  |";

            return output;
        }

        public void ButtonProcessor(int buttonID)
        {
            if (buttonID == btnPrevRwy)
            {
                var.FlightData.rwyIdx--;
            }
            if (buttonID == btnNextRwy)
            {
                var.FlightData.rwyIdx++;
            }
            if (buttonID == btnPrevGS)
            {
                var.FlightData.gsIdx--;
            }
            if (buttonID == btnNextGS)
            {
                var.FlightData.gsIdx++;
            }
            //if (buttonID == 2) //print coordinates on Debug console
            //{
            //    var v = var.FlightData.currentVessel;
            //    var r = var.FlightData.selectedRwy;
            //    Debug.Log("Lat: " + v.latitude + " Lon: " + v.longitude + " GCD: " + NavUtilLib.Utils.CalcGreatCircleDistance(v.latitude, v.longitude, r.gsLatitude, r.gsLongitude, r.body));
            //}

            //Debug.Log("ButtonID: " + buttonID);

            var.FlightData.rwyIdx = Utils.indexChecker(var.FlightData.rwyIdx, var.FlightData.rwyList.Count - 1, 0);
            var.FlightData.gsIdx = Utils.indexChecker(var.FlightData.gsIdx, var.FlightData.gsList.Count - 1, 0);

            if (buttonID == btnDefaultRwyGS)
                var.FlightData.gsIdx = var.FlightData.rwyIdx = 0; //"Back" Key will return the HSI to default runway and GS
        }

        public void Start()
        {
            if (var.Settings.enableDebugging) Debug.Log("MLS: Starting systems...");
            if (!var.Settings.navAidsIsLoaded)
                var.Settings.loadNavAids();

            if (!var.Materials.isLoaded)
                var.Materials.loadMaterials();

            //if (!var.Audio.isLoaded)
                var.Audio.initializeAudio();

                if (var.Settings.enableDebugging) Debug.Log("MLS: Systems started successfully!");

            //doneLoading = true;
        }
    }

    public class KSF_AI : InternalModule
    {

        public bool DrawAI(RenderTexture screen, float aspectRatio)
        {
            NavUtilLib.GlobalVariables.FlightData.updateNavigationData();

            NavUtilLib.DisplayData.DrawAI(screen, aspectRatio);


            //NavUtilLib.TextWriter.addTextToRT(screen, "Runway: " + NavUtilLib.GlobalVariables.FlightData.selectedRwy.ident, new Vector2(20, screen.height - 40), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            //NavUtilLib.TextWriter.addTextToRT(screen, "Glideslope: " + string.Format("{0:F1}", NavUtilLib.GlobalVariables.FlightData.selectedGlideSlope) + "°  Elevation: " + string.Format("{0:F0}", NavUtilLib.GlobalVariables.FlightData.selectedRwy.altMSL) + "m", new Vector2(20, screen.height - 64), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);

            //NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.Utils.makeAngle0to360(FlightGlobals.ship_heading), true).ToString(), new Vector2(584, screen.height - 102), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            //NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.Utils.makeAngle0to360(NavUtilLib.GlobalVariables.FlightData.bearing), true).ToString(), new Vector2(584, screen.height - 131), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            //NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.Utils.makeAngle0to360(NavUtilLib.GlobalVariables.FlightData.selectedRwy.hdg), true).ToString(), new Vector2(35, screen.height - 124), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            //NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.GlobalVariables.FlightData.dme / 1000, false).ToString(), new Vector2(45, screen.height - 563), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);

            return true;
        }

        public void Start()
        {
            if (var.Settings.enableDebugging) Debug.Log("NavUtils AI: Starting systems...");

            if (!var.Materials.isLoaded)
                var.Materials.loadMaterials();

            if (var.Settings.enableDebugging) Debug.Log("NavUtils AI: Systems started successfully!");

        }
    }

}
