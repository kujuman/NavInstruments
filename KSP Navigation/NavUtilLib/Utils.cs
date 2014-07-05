using System;
using UnityEngine;

namespace NavUtilLib
{
    public static class Utils
    {
        #region friendly HSI/ILS calculators
        /// <returns>double in degrees</returns>
        public static double CalcBearingToBeacon(Vessel thisVessel, Runway currentRwy)
        {
            var lonDelta = currentRwy.locLongitude - thisVessel.longitude;
            return Utils.CalcBearingTo(Utils.CalcRadiansFromDeg(lonDelta), Utils.CalcRadiansFromDeg(thisVessel.latitude), Utils.CalcRadiansFromDeg(currentRwy.locLatitude));
        }

        /// <returns>float in degrees</returns>
        public static float CalcLocalizerDeviation(double bearingToLoc, Runway currentRwy)
        {
            var RwyHdgCorrection = 180 - Utils.makeAngle0to360(currentRwy.hdg);
            
            return (float) (Utils.makeAngle0to360(currentRwy.hdg + RwyHdgCorrection) - Utils.makeAngle0to360(bearingToLoc + RwyHdgCorrection));


            //return (float)(Utils.makeAngle0to360(currentRwy.hdg) - Utils.makeAngle0to360(bearingToLoc)) * -1;
        }

        ///<returns>double in meters</returns>
        public static double CalcDistanceToBeacon(Vessel thisVessel, Runway currentRwy)
        {
            return Utils.CalcDistance(thisVessel.latitude, thisVessel.longitude, thisVessel.altitude, currentRwy.gsLatitude, currentRwy.gsLongitude, currentRwy.altMSL, currentRwy.body);
        }

        /// <returns>double in degrees</returns>
        public static double CalcElevationAngle(Vessel thisVessel, Runway currentRwy)
        {
            return Utils.CalcElevationFrom(thisVessel.latitude, thisVessel.longitude, thisVessel.altitude, currentRwy.gsLatitude, currentRwy.gsLongitude, currentRwy.altMSL, currentRwy.body) - 90;  //we subtract 90 degrees since 0 is towards the center of the body and 180 is directly outwards from the body; we want to know the angle relative to the tangent
        }

        /// <returns>float in degrees</returns>
        public static float CalcGlideslopeDeviation(double elevationAngle, double glideslope)
        {
            return (float)(elevationAngle - glideslope);
        }
        #endregion


        #region relative position calculators
        public static double CalcDistance(double lat1, double lon1, double altMSL1, double lat2, double lon2, double altMSL2, string bodyName)
        {
            var cBody = FlightGlobals.Bodies.Find(body => body.name == bodyName);

            var x = cBody.GetWorldSurfacePosition(lat1, lon1, altMSL1);
            var y = cBody.GetWorldSurfacePosition(lat2, lon2, altMSL2);

            var dist = Vector3d.Distance(x, y);

            return dist;
        }

        public static double CalcElevationFrom(double lat1, double lon1, double altMSL1, double lat2, double lon2, double altMSL2, string bodyName)
        {
            var cBody = FlightGlobals.Bodies.Find(body => body.name == bodyName);

            var x = cBody.GetWorldSurfacePosition(lat1, lon1, altMSL1);
            var y = cBody.GetWorldSurfacePosition(lat2, lon2, altMSL2);

            Vector3d targetDir = y - x;

            Vector3d up = (y - cBody.position).normalized;

            double angle = Vector3.Angle(targetDir, up);

            //("Elevation angle = " + (angle - 90).ToString("F1") + "°");

            return angle;
        }

        public static double CalcBearingTo(double deltaLon, double startLat, double endLat)
        {
            var y = Math.Sin(deltaLon) * Math.Cos(endLat);

            var x = (Math.Cos(startLat) * Math.Sin(endLat)) - (Math.Sin(startLat) * Math.Cos(endLat) * Math.Cos(deltaLon));

            var brng = Math.Atan2(y, x);

            brng = CalcDegFromRadians(brng);

            brng = makeAngle0to360(brng);

            return brng;
        }

        public static double CalcGreatCircleDistance(double lat1, double lon1, double lat2, double lon2, string bodyName)
        {
            var φ1 = CalcRadiansFromDeg(lat1);
            var φ2 = CalcRadiansFromDeg(lat2);
            var Δφ = CalcRadiansFromDeg(lat2 - lat1);
            var Δλ = CalcRadiansFromDeg(lon2 - lon1);

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                Math.Cos(φ1) * Math.Cos(φ2) *
                Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return c * FlightGlobals.Bodies.Find(body => body.name == bodyName).Radius;
        }
        #endregion


        #region working with angles tools
        public static double CalcDegFromRadians(double radian)
        {
            return radian * (180 / Math.PI);
        }

        public static double CalcRadiansFromDeg(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public static double makeAngle0to360(double angle)
        {
            angle = (angle + 360) % 360;

            return angle;
        }

        public static double oppositeHeading(double heading)
        {
            heading = (heading + 180) % 360;
            return heading;
        }
        #endregion


        #region ILS elevation at distance
        public static double CalcSurfaceAltAtDME(float DMEinMeters, string bodyName, float glideSlope, float rwyElevation)
        {
            double bodyRadius = FlightGlobals.Bodies.Find(body => body.name == bodyName).Radius;

            double numerator = (bodyRadius + rwyElevation) * Math.Cos(CalcRadiansFromDeg(glideSlope));
            double denominator = Math.Cos(DMEinMeters / bodyRadius) + CalcRadiansFromDeg(glideSlope);
            return numerator / denominator - bodyRadius;
        }
        #endregion

        #region Format text for display on a screen
        
        #endregion
    }
}
