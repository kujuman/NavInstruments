//NavUtilities by kujuman, © 2014. All Rights Reserved.

using System;
using UnityEngine;
using KSP;

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

        //the following section is courtesy Virindi
        /// <returns>double in degrees</returns>
        public static double CalcProjectedRunwayHeading(Vessel thisVessel, Runway currentRwy)
        {
            try
            {
                double bodyradius = FlightGlobals.Bodies.Find(body => body.name == currentRwy.body).Radius + (double)currentRwy.altMSL;

                Vector3d rwy = SphericalToCartesianCoordinates(currentRwy.locLatitude, currentRwy.locLongitude, bodyradius);
                Vector3d ship = SphericalToCartesianCoordinates(thisVessel.latitude, thisVessel.longitude, bodyradius);

                Vector3d northpole = new Vector3d(0, bodyradius, 0);
                Vector3d eastfromrwy = Vector3d.Cross(northpole, rwy).normalized;
                Vector3d northfromrwy = Vector3d.Cross(eastfromrwy, rwy).normalized;
                Vector3d eastfromship = Vector3d.Cross(northpole, ship).normalized;
                Vector3d northfromship = Vector3d.Cross(eastfromship, ship).normalized;
                double rwyheadingradians = -Utils.CalcRadiansFromDeg(currentRwy.hdg);
                Vector3d rwynormalized = rwy.normalized;

                //Rodrigues' rotation formula
                //This vector represents the runway direction of travel in the plane of the runway normal to the surface (the surface plane).
                Vector3d offsetloc = northfromrwy * Math.Cos(rwyheadingradians) + Vector3d.Cross(rwynormalized, northfromrwy) * Math.Sin(rwyheadingradians) + rwynormalized * Vector3d.Dot(rwynormalized, northfromrwy) * (1d - Math.Cos(rwyheadingradians));

                //Project runway heading vector on to our bearing axes
                double northportion = Vector3d.Dot(northfromship, offsetloc);
                double eastportion = Vector3d.Dot(eastfromship, offsetloc);

                double ret = makeAngle0to360(CalcDegFromRadians(Math.Atan2(northportion, eastportion)) - 90d);

                //Debug.Log ("Old rwy heading: " + currentRwy.hdg.ToString());
                //Debug.Log ("New rwy heading: " + ret.ToString ());

                return ret;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                return 0d;
            }
        }

        /// <returns>double in degrees</returns>
        public static double CalcLocalizerDeviation(Vessel thisVessel, Runway currentRwy)
        {
            try
            {
                /*
                {
                    //Old algorithm.
                    double rwyhdg = (double)currentRwy.hdg;
                    double RwyHdgCorrection = 180d - Utils.makeAngle0to360(rwyhdg);
                    Debug.Log("Old answer: " + (Utils.makeAngle0to360(rwyhdg + RwyHdgCorrection) - Utils.makeAngle0to360(Utils.CalcBearingToBeacon(thisVessel, currentRwy) + RwyHdgCorrection)).ToString());
                }
                */

                //We are going to measure the angle in the plane of the runway (normal to the surface).
                //So, we will pretend everything is at this altitude.
                double bodyradius = FlightGlobals.Bodies.Find(body => body.name == currentRwy.body).Radius + (double)currentRwy.altMSL;

                Vector3d rwy = SphericalToCartesianCoordinates(currentRwy.locLatitude, currentRwy.locLongitude, bodyradius);
                Vector3d ship = SphericalToCartesianCoordinates(thisVessel.latitude, thisVessel.longitude, bodyradius);

                Vector3d northpole = new Vector3d(0, bodyradius, 0);
                Vector3d eastfromrwy = Vector3d.Cross(northpole, rwy).normalized;
                Vector3d northfromrwy = Vector3d.Cross(eastfromrwy, rwy).normalized;
                double rwyheadingradians = -Utils.CalcRadiansFromDeg(currentRwy.hdg);
                Vector3d rwynormalized = rwy.normalized;

                //Rodrigues' rotation formula
                //This vector represents the runway direction of travel in the plane of the runway normal to the surface (the surface plane).
                Vector3d offsetloc = northfromrwy * Math.Cos(rwyheadingradians) + Vector3d.Cross(rwynormalized, northfromrwy) * Math.Sin(rwyheadingradians) + rwynormalized * Vector3d.Dot(rwynormalized, northfromrwy) * (1d - Math.Cos(rwyheadingradians));

                //Normal for the plane encompassing the runway, in the direction of travel, and the center of the body (the centerline plane).
                Vector3d planenormal = Vector3d.Cross(offsetloc, rwy).normalized;

                //Distance left to right from the centerline plane.
                double cldist = Vector3d.Dot(planenormal, ship);
                //Distance forward and back to the plane encompassing the runway point, the runway normal, and the origin (how far in front or behind you are from the marker)
                double fwbackdist = Vector3d.Dot(offsetloc.normalized, ship);

                //We are computing the angle that is projected into the surface plane.
                double ret = -CalcDegFromRadians(Math.Atan2(cldist, fwbackdist));

                //Debug.Log ("new answer: " + ret.ToString ());
                return ret;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                return 0d;
            }
        }

        public static Vector3d SphericalToCartesianCoordinates(double lat, double lng, double radius)
        {
            lat = Utils.CalcRadiansFromDeg(lat);
            lng = Utils.CalcRadiansFromDeg(lng);
            double t = radius * Math.Cos(lat);
            double y = radius * Math.Sin(lat);
            double x = t * Math.Sin(lng);
            double z = t * Math.Cos(lng);
            return new Vector3d(x, y, z);
        }
        //end Virindi contribution :D

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

            double angle = Vector3d.Angle(targetDir, up);

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

        public static Vector2d CalcCoordinatesFromInitialPointBearingDistance(Vector2d initPoint, double bearingDeg, double distMeters, double bodyRadius)
        {
            var φ1 = CalcRadiansFromDeg(initPoint.x);
            var λ1 = CalcRadiansFromDeg(initPoint.y);

            bearingDeg = makeAngle0to360(bearingDeg);

            bearingDeg = CalcRadiansFromDeg(bearingDeg);

            var φ2 = Math.Asin(Math.Sin(φ1) * Math.Cos(distMeters / bodyRadius) +
                    Math.Cos(φ1) * Math.Sin(distMeters / bodyRadius) * Math.Cos(bearingDeg));
            var λ2 = λ1 + Math.Atan2(Math.Sin(bearingDeg) * Math.Sin(distMeters / bodyRadius) * Math.Cos(φ1),
                                     Math.Cos(distMeters / bodyRadius) - Math.Sin(φ1) * Math.Sin(φ2));

            return new Vector2d(CalcDegFromRadians(φ2), CalcDegFromRadians(λ2));
        }
        #endregion


        #region working with angles tools
        //const OneEightyOverPi thanks to Virindi
        const double OneEightyOverPi = 180d / Math.PI;
        public static double CalcDegFromRadians(double radian)
        {
            return radian * OneEightyOverPi;
        }

        //const PiOverOneEighty thanks to Virindi
        const double PiOverOneEighty = Math.PI / 180d;
        public static double CalcRadiansFromDeg(double deg)
        {
            return deg * PiOverOneEighty;
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
       public static string numberFormatter(float numToDisplay, bool isHeading)
        {
            string output = "";

            int tenths = (int)((numToDisplay * 10) % 10);
            int ones = (int)Math.Abs(numToDisplay / 1 % 10);
            int tens = (int)Math.Abs(numToDisplay / 10 % 10);
            int hundreds = (int)Math.Abs(numToDisplay / 100 % 10);

            if (isHeading)
            {
                output = hundreds.ToString() + tens.ToString() + ones.ToString();
                return output;
            }

            if (numToDisplay < 100)
            {
                if (tens != 0)
                    output = tens.ToString();

                output += ones.ToString() + "." + tenths.ToString();
                return output;
            }

            output = hundreds.ToString() + tens.ToString() + ones.ToString();

            if (numToDisplay > 999.5)
                output = "999";


            return output;
        }

        #endregion

       public static int indexChecker(int index, int maxIndex, int minIndex)
       {
           if (maxIndex < index)
           {
               index = minIndex;
           }
           else if (minIndex > index)
           {
               index = maxIndex;
           }

           return index;
       }
    
    }
}
