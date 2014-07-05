using System;
using UnityEngine;
using KSP;

namespace KSF_NavInstruments
{
    public class KSF_HSI_Display : PartModule
    {
        [KSPField]
        double LocalizerRange = 40;

        [KSPField]
        double GSRange = 40;

        public bool bState = false;

        [KSPEvent(guiActive = true, guiName = "Turn ILS On", active = true)]
        public void ActivateILSDisplay()
        {
            Events["DeactivateILSDisplay"].active = true;
            Events["ActivateILSDisplay"].active = false;
            bState = !bState;
        }

        [KSPEvent(guiActive = true, guiName = "Turn ILS Off", active = false)]
        public void DeactivateILSDisplay()
        {
            Events["DeactivateILSDisplay"].active = false;
            Events["ActivateILSDisplay"].active = true;
            bState = !bState;
        }

        [KSPEvent(guiActive = true, guiName = "Next Glideslope", active = true)]
        public void NextGlideslope()
        {
            if (glideslopeList.Count - 1 == glideslopeIdx)
            {
                glideslopeIdx = 0;
            }
            else
            {
                glideslopeIdx++;
            }
        }

        [KSPEvent(guiActive = true, guiName = "Next Runway", active = true)]
        public void NextRunway()
        {
            print(rwyList.Count + " " + rwyIdx);

            if (rwyList.Count - 1 == rwyIdx)
            {
                rwyIdx = 0;
            }
            else
            {
                rwyIdx++;
            }
        }

        [KSPField(guiActive = true, guiName = "Current Runway")]
        string runwayName = "None";

        [KSPField(guiActive = true, guiName = "Current Glideslope", guiUnits = "°", guiFormat = "F1")]
        float glideslope = 3f;

        RenderTexture screenRT;


        System.Collections.Generic.List<KSF_RunwayInformation> rwyList = new System.Collections.Generic.List<KSF_RunwayInformation>();

        //public KSF_RunwayInformation[] rwyList = new KSF_RunwayInformation[];

        public static int rwyIdx = 0;

      System.Collections.Generic.List<float> glideslopeList = new System.Collections.Generic.List<float>();
        public int glideslopeIdx = 0;

        //bool fineLoc = true;

        public bool locFlag = true;
        public bool gsFlag = true;
        public bool inZoneOfConfusion = true;

        public double bearingToBeacon = 0;
        public double distanceToBeacon = 0;
        public double elevationAngle = 0;

        public float glideslopeDeviation = 0;


        //gui junk
        public static Rect windowPosition = new Rect(250, 50, 501, 501);       //The position for our little window (left, top, width, height)
        //private static Rect lastPosition = windowPosition;                    //Used so I don't over-save
        //public static bool isMinimized;                                       //Is the window currently minimized?
        private static float Scale = 1;
        private static float Alpha = 1;

        //[KSPField(guiActive = true, guiName = "Loc Deviation")]
        private float localizerDeviation = 0;

        //private static float localizerDeviation = 0;


//private static Texture2D hsi_back;

        public override void OnAwake()
        {
            /*
            KSF_RunwayInformation rwy = new KSF_RunwayInformation();

            rwyList.Clear();

            rwy.ident = "KSC 09";
            rwy.hdg = 90.4;
            rwy.identOfOpposite = "KSC 27";
            rwy.body = "Kerbin";
            rwy.altMSL = 70;
            rwy.gsLatitude = -0.04877658f;
            rwy.gsLongitude = 285.29973537f;
            rwyList.Add(rwy);

            rwy = new KSF_RunwayInformation();

            rwy.ident = "KSC 27";
            rwy.hdg = 270.4;
            rwy.identOfOpposite = "KSC 09";
            rwy.body = "Kerbin";
            rwy.altMSL = 70;
            rwy.gsLatitude = -0.05005861f;
            rwy.gsLongitude = 285.4824816f;
            rwyList.Add(rwy);

            rwy = new KSF_RunwayInformation();

            rwy.ident = "Island 09";
            rwy.hdg = 89.38;
            rwy.identOfOpposite = "Island 27";
            rwy.body = "Kerbin";
            rwy.altMSL = 134;
            rwy.gsLatitude = -1.517162103f;
            rwy.gsLongitude = 288.04726296f;
            rwyList.Add(rwy);

            rwy = new KSF_RunwayInformation();

            rwy.ident = "Island 27";
            rwy.hdg = 269.38;
            rwy.identOfOpposite = "Island 09";
            rwy.body = "Kerbin";
            rwy.altMSL = 134;
            rwy.gsLatitude = -1.517162103f;
            rwy.gsLongitude = 288.04726296f;
            rwyList.Add(rwy);
            */

            rwyList = ConfigLoader.GetRunwayListFromConfig();

            Scale = ConfigLoader.GetGUIScale();

            glideslopeList.Clear();
            glideslopeList.Add(3);
            glideslopeList.Add(5);
            glideslopeList.Add(8);
            glideslopeList.Add(10);
            glideslopeList.Add(15);
            glideslopeList.Add(20);
            glideslopeList.Add(25);

            //load image assets
            //print("load assests");
            Resources.loadAssets();

            //initialize gui

            //hsi_back = new Texture2D(Convert.ToInt16(501 * Scale), Convert.ToInt16(501 * Scale));

            //Color c = new Color(0, 0, 0, 1);

            //for (int y = 0; y < hsi_back.height; y++)
            //{
            //    for (int x = 0; x < hsi_back.width; x++)
            //    {
            //        hsi_back.SetPixel(x, y, c);
            //    }
            //}

        }


        public override void OnStart(StartState state)
        {
            RenderingManager.AddToPostDrawQueue(3, OnDraw);
        }

        public override void OnInactive()
        {
            RenderingManager.RemoveFromPostDrawQueue(3, OnDraw); //close the GUI
        }



        private void OnDraw()
        {
            if (bState)   //Only draw the window if we want to show it
            {
                windowPosition.width = Resources.hsi_back.width * Scale;
                windowPosition.height = Resources.hsi_back.height * Scale;
                if ((windowPosition.xMin + windowPosition.width) < 20) windowPosition.xMin = 20 - windowPosition.width; //left limit
                if (windowPosition.yMin + windowPosition.height < 20) windowPosition.yMin = 20 - windowPosition.height; //top limit
                if (windowPosition.xMin > Screen.width - 20) windowPosition.xMin = Screen.width - 20;   //right limit
                if (windowPosition.yMin > Screen.height - 20) windowPosition.yMin = Screen.height - 20; //bottom limit
                windowPosition = GUI.Window(1, windowPosition, OnWindow, "Horizontal Situation Indicator");
            }
        }

        private void OnWindowRT(int WindowID)
        {
            GUI.DrawTexture(new Rect(0,0,screenRT.width,screenRT.height),screenRT,ScaleMode.ScaleToFit,true);
            GUI.DragWindow();
        }

        private void OnWindow(int WindowID)
        {
            GUI.DrawTexture(new Rect(0, 0, Resources.hsi_back.width * Scale, Resources.hsi_back.height * Scale), Resources.hsi_back, ScaleMode.ScaleToFit, true); //background

            drawCenterRotatedImage((float)makeAngle0to360(360 - FlightGlobals.ship_heading), Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_heading_card, 0, 0); //compass

            drawCenterRotatedImage((float)makeAngle0to360(360 - FlightGlobals.ship_heading) + (float)makeAngle0to360(bearingToBeacon), Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_NDB_needle, 0, 0); //NDB

            drawCenterRotatedImage((float)makeAngle0to360(360 - FlightGlobals.ship_heading) + (float)rwyList[rwyIdx].hdg, Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_course_needle, 0, 0); //course

            float deviationCorrection;
            deviationCorrection = localizerDeviation * 50 * Scale;
            deviationCorrection = Math.Min(150 * Scale, deviationCorrection);
            deviationCorrection = Math.Max(-150 * Scale, deviationCorrection);

            drawCenterRotatedImage((float)makeAngle0to360(360 - FlightGlobals.ship_heading) + (float)rwyList[rwyIdx].hdg, Resources.hsi_back.width, Resources.hsi_back.height, Resources.hsi_course_deviation_needle, deviationCorrection, 0); //localizer needle

            GUI.DrawTexture(new Rect(0, 0, Resources.hsi_overlay.width * Scale, Resources.hsi_overlay.height * Scale), Resources.hsi_overlay, ScaleMode.ScaleToFit, true);//overlay

            deviationCorrection = glideslopeDeviation * 200;
            deviationCorrection = Math.Min(140, deviationCorrection);
            deviationCorrection = Math.Max(-140, deviationCorrection);
            //deviationCorrection *= -1;

            GUI.DrawTexture(new Rect(0,(329 + deviationCorrection) * Scale, Resources.hsi_GS_pointer.width * Scale, Resources.hsi_GS_pointer.height * Scale), Resources.hsi_GS_pointer, ScaleMode.ScaleToFit, true);//glideslope pointer

            drawNumbers(50, 473, (float)Math.Min(999, Math.Round(distanceToBeacon / 1000f, 1)), false); //draw DME
            drawNumbers(30, 40, (float)Math.Round(rwyList[rwyIdx].hdg), true); //draw Course
            drawNumbers(450, 10, (float)Math.Round(FlightGlobals.ship_heading), true); //draw heading
            drawNumbers(450, 39, (float)Math.Round(makeAngle0to360(bearingToBeacon)), true); //draw bearing


            GUI.DragWindow();
        }

        private void drawCenterRotatedImage(float heading, float width, float height, Texture2D img, float xOffset, float yOffset)
        {
            float w = img.width;
            float h = img.height;

            float x = (width - w) * Scale / 2 + xOffset * Scale;
            float y = (height - h) * Scale / 2 + yOffset * Scale;
            float widthP = w * Scale;
            float heightP = h * Scale;

            Vector2 pivotPoint = new Vector2(width * Scale / 2, height*Scale / 2);

            GUIUtility.RotateAroundPivot(heading, pivotPoint);
            GUI.DrawTexture(new Rect(x, y, widthP, heightP), img, ScaleMode.ScaleToFit, true);
            GUI.matrix = Matrix4x4.identity;
        }

        private void drawNumbers(float leftX, float topY, float numToDisplay, bool isHeading)
        {
            Texture2D img = Resources.base_digit_strip;

            topY *= Scale;
            leftX *= Scale;

            int tenths = (int)((numToDisplay * 10) % 10);
            int ones = (int)Math.Abs(numToDisplay / 1 % 10);
            int tens = (int)Math.Abs(numToDisplay / 10 % 10);
            int hundreds = (int)Math.Abs(numToDisplay / 100 % 10);

            float digitStart = 0;

            float offSet = 0;
            float width = 0;

            if (numToDisplay >= 100 || isHeading)
            {
                offSet = 14f * hundreds / 146f;
                width = 14f / 146f;
                GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

                digitStart += 14 * Scale ;

                offSet = 14 * tens / 146f;
                GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

                digitStart += 14 * Scale;

                offSet = 14 * ones / 146f;
                GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

                return;
            }

            if (numToDisplay < 100)
            {
                width = 14 / 146f;

                if (tens != 0)
                {
                    offSet = 14 * tens / 146f;
                    GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));
                }

                digitStart += 14 * Scale;

                offSet = 14 * ones / 146f;
                GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

                digitStart += 14 * Scale;

                width = 2 / 146f;
                offSet = 142 / 146f;
                GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 2* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

                digitStart += 2 * Scale;

                width = 14 / 146f;
                offSet = 14 * tenths / 146f;
                GUI.DrawTextureWithTexCoords(new Rect(leftX + digitStart, topY, 14* Scale, 17* Scale), img, new Rect(offSet, 0, width, 1));

                return;
            }
        }

        public override void OnUpdate()
        {
            if (bState)
            {
                //update from user input
                glideslope = glideslopeList[glideslopeIdx];
                runwayName = rwyList[rwyIdx].ident;

                //compute position
                var currentRwy = rwyList[rwyIdx];

                var thisVessel = this.vessel;


                /* this worked but is outdated
                var oppositeRunway = rwyList.Find(KSF_RunwayInformation => KSF_RunwayInformation.ident == currentRwy.identOfOpposite);

                var lonDelta = oppositeRunway.gsLongitude - thisVessel.longitude;
                bearingToBeacon = CalcBearingTo(CalcRadiansFromDeg(lonDelta), CalcRadiansFromDeg(thisVessel.latitude), CalcRadiansFromDeg(oppositeRunway.gsLatitude));
                */

                var lonDelta = currentRwy.locLongitude - thisVessel.longitude;
                bearingToBeacon = CalcBearingTo(CalcRadiansFromDeg(lonDelta), CalcRadiansFromDeg(thisVessel.latitude), CalcRadiansFromDeg(currentRwy.locLatitude));

                localizerDeviation = (float)(makeAngle0to360(currentRwy.hdg) - makeAngle0to360(bearingToBeacon));

                //print("A " + currentRwy.hdg);
                //print("B " + bearingToBeacon);

                localizerDeviation = localizerDeviation * (-1);
                //localizerDeviation1 = localizerDeviation;


                distanceToBeacon = CalcDistance(thisVessel.latitude,thisVessel.longitude,thisVessel.altitude,currentRwy.gsLatitude,currentRwy.gsLongitude,currentRwy.altMSL,currentRwy.body);
                elevationAngle = CalcElevationFrom(thisVessel.latitude, thisVessel.longitude, thisVessel.altitude, currentRwy.gsLatitude, currentRwy.gsLongitude, currentRwy.altMSL, currentRwy.body) - 90; //we subtract 90 degrees since 0 is towards the center of the body and 180 is directly outwards from the body; we want to know the angle relative to the tangent

                glideslopeDeviation = ((float)elevationAngle - glideslope);

                //evaluateZoneOfConfusion(elevationAngle);
                //evaluateGSflag(distanceToBeacon, GSRange, elevationAngle);
                //evaluateLocalizerFlag(distanceToBeacon, LocalizerRange);

                //print(thisVessel.latitude + " " + thisVessel.longitude + " " + thisVessel.altitude);

            }
        }

        public void evaluateGSflag(double distanceToBeacon, double glideslopeRange, double elevationAngle)
        {
            if (inZoneOfConfusion)
            {
                gsFlag = true;
                return;
            }

            if (elevationAngle < 0)
            {
                gsFlag = true;
                return;
            }

            if (distanceToBeacon > glideslopeRange)
            {
                gsFlag = true;
                return;
            }

            gsFlag = false;
        }

        public void evaluateLocalizerFlag(double distanceToBeacon, double localizerRange)
        {
            if (inZoneOfConfusion)
            {
                locFlag = true;
                return;
            }

            if (distanceToBeacon > localizerRange)
            {
                locFlag = true;
                return;
            }

            locFlag = false;
        }

        public void evaluateZoneOfConfusion(double elevationAngle)
        {
            int coneLimit = 75;
            if (elevationAngle > coneLimit)
                inZoneOfConfusion = true;
            else
                inZoneOfConfusion = false;
        }

        public double CalcDistance(double lat1, double lon1, double altMSL1, double lat2, double lon2, double altMSL2, string bodyName)
        {
            var cBody = FlightGlobals.Bodies.Find(body => body.name == bodyName);

            var x = cBody.GetWorldSurfacePosition(lat1, lon1, altMSL1);
            var y = cBody.GetWorldSurfacePosition(lat2, lon2, altMSL2);

            var dist = Vector3d.Distance(x, y);

            return dist;
        }

        public double CalcElevationFrom(double lat1, double lon1, double altMSL1, double lat2, double lon2, double altMSL2, string bodyName)
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

        public double CalcBearingTo(double deltaLon, double startLat, double endLat)//this code returns good results near the runways and then gets inaccurate quickly
        {
            var y = Math.Sin(deltaLon) * Math.Cos(endLat);

            var x = (Math.Cos(startLat) * Math.Sin(endLat)) - (Math.Sin(startLat) * Math.Cos(endLat) * Math.Cos(deltaLon));

            var brng = Math.Atan2(y, x);

            brng = CalcDegFromRadians(brng);

            brng = makeAngle0to360(brng);


            //print("Bearing to runway " + brng.ToString("F1") + "°");

            return brng;
        }

        public double CalcDegFromRadians(double radian)
        {
            return radian * (180 / Math.PI);
        }

        public double CalcRadiansFromDeg(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public double makeAngle0to360(double angle)
        {
            angle = (angle + 360) % 360;

            return angle;
        }

        public double oppositeHeading(double heading)
        {
            heading = (heading + 180) % 360;
            return heading;
        }
    }

        public class KSF_RunwayInformation
        {
            public string ident;
            public float hdg;
            public string identOfOpposite;
            public string body;
            public float altMSL;
            public float gsLatitude;
            public float gsLongitude;

            public float locLatitude;
            public float locLongitude;
        }


}
