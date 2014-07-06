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
        bool doneLoading = false;

        public bool DrawMLS(RenderTexture screen, float aspectRatio)
        {
            return var.DisplayData.DrawHSI(screen, aspectRatio);
        }

        public string pageAuthor(int screenWidth, int screenHeight)
        {
            if(!(var.FlightData.GetLastNavUpdateUT() + 0.05 > Planetarium.GetUniversalTime()))
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
            if (buttonID == 5)
            {
                var.FlightData.rwyIdx--;
            }
            if (buttonID == 6)
            {
                var.FlightData.rwyIdx++;
            }
            if (buttonID == 1)
            {
                var.FlightData.gsIdx--;
            }
            if (buttonID == 0)
            {
                var.FlightData.gsIdx++;
            }
            if (buttonID == 2) //print coordinates on Debug console
            {
                var v = var.FlightData.currentVessel;
                var r = var.FlightData.selectedRwy;
                Debug.Log("Lat: " + v.latitude + " Lon: " + v.longitude + " GCD: " + NavUtilLib.Utils.CalcGreatCircleDistance(v.latitude, v.longitude, r.gsLatitude, r.gsLongitude, r.body));
            }

            var.FlightData.rwyIdx = Utils.indexChecker(var.FlightData.rwyIdx, var.FlightData.rwyList.Count - 1, 0);
            var.FlightData.gsIdx = Utils.indexChecker(var.FlightData.gsIdx, var.FlightData.gsList.Count - 1, 0);

            if (buttonID == 3)
                var.FlightData.gsIdx = var.FlightData.rwyIdx = 0; //"Back" Key will return the HSI to default runway and GS
        }

        public void Start()
        {
            Debug.Log("MLS: Starting systems...");
            if (!var.Settings.navAidsIsLoaded)
                var.Settings.loadNavAids();

            if (!var.Materials.isLoaded)
                var.Materials.loadMaterials();

            if (!var.Audio.isLoaded)
                var.Audio.initializeAudio();

            Debug.Log("MLS: Systems started successfully!");

            doneLoading = true;
        }
    }
}
