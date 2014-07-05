/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

namespace HSI
{
    class HSIController
    {
        List<Runway> rwyList;
        int rwyIdx;

        float fGlideslope = 3;


        float scale;


        public double bearingToBeacon = 0;
        public double distanceToBeacon = 0;
        public double elevationAngle = 0;
        public float glideslopeDeviation = 0;
        public float localizerDeviation = 0;


        string sSettingFileURL = "GameData/KerbalScienceFoundation/NavInstruments/settings.cfg";

        public void Activate()
        {
            //Load Runway List
            rwyList.Clear();
            rwyList = ConfigLoader.GetRunwayListFromConfig(sSettingFileURL);

            //Load GUI Scale
            scale = ConfigLoader.GetGUIScale(sSettingFileURL);
        }











        [Obsolete("The calculations in this method are now in Utils.* Use those for greater flexibility.")]
        public void CalculateHSI(Vessel thisVessel, Runway currentRwy) //depreciated
        {
            /*
            Runway currentRwy = rwyList[rwyIdx];
            Vessel thisVessel = FlightGlobals.ActiveVessel;
            */


            /* this worked but is outdated
            var oppositeRunway = rwyList.Find(KSF_RunwayInformation => KSF_RunwayInformation.ident == currentRwy.identOfOpposite);

            var lonDelta = oppositeRunway.gsLongitude - thisVessel.longitude;
            bearingToBeacon = CalcBearingTo(CalcRadiansFromDeg(lonDelta), CalcRadiansFromDeg(thisVessel.latitude), CalcRadiansFromDeg(oppositeRunway.gsLatitude));
            end/

            //now in CalcBearingToBeacon()
            var lonDelta = currentRwy.locLongitude - thisVessel.longitude;
            bearingToBeacon = Utils.CalcBearingTo(Utils.CalcRadiansFromDeg(lonDelta), Utils.CalcRadiansFromDeg(thisVessel.latitude), Utils.CalcRadiansFromDeg(currentRwy.locLatitude));

            //Now in CalcLocalizerDeviation()
            localizerDeviation = (float)(Utils.makeAngle0to360(currentRwy.hdg) - Utils.makeAngle0to360(bearingToBeacon));
            localizerDeviation = localizerDeviation * (-1);

            //Now in CalcDistanceToBeacon()
            distanceToBeacon = Utils.CalcDistance(thisVessel.latitude, thisVessel.longitude, thisVessel.altitude, currentRwy.gsLatitude, currentRwy.gsLongitude, currentRwy.altMSL, currentRwy.body);
            
            //Now in CalcElevationAngle()
            elevationAngle = Utils.CalcElevationFrom(thisVessel.latitude, thisVessel.longitude, thisVessel.altitude, currentRwy.gsLatitude, currentRwy.gsLongitude, currentRwy.altMSL, currentRwy.body) - 90; //we subtract 90 degrees since 0 is towards the center of the body and 180 is directly outwards from the body; we want to know the angle relative to the tangent

            //Now in CalcGlideslopeDeviation()
            glideslopeDeviation = ((float)elevationAngle - fGlideslope);

            //evaluateZoneOfConfusion(elevationAngle);
            //evaluateGSflag(distanceToBeacon, GSRange, elevationAngle);
            //evaluateLocalizerFlag(distanceToBeacon, LocalizerRange);
        }


    }
}
*/