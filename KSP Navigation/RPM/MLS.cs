using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;
using KSP;
using NavUtilLib;
using mat = NavUtilLib.GlobalVariables.Materials;
using aud = NavUtilLib.GlobalVariables.Audio;

namespace KSFRPMHSI
{
    public class KSF_MLS : InternalModule
    {
        bool doneLoading = false;

        List<Runway> rwyList;
        public static int rwyIdx;

        List<float> gsList;
        public static int gsIdx;

        public Runway Rwy;
        public float glideSlope;
        public Vessel thisVessel;

        public static byte bcnCode = 0;
        public static byte lastBcnCode;

        private static bool gsFlag;
        private static bool bcFlag;
        private static bool locFlag;

        public static double timer;

        /*
        private GameObject audioplayer = new GameObject();
        private AudioSource markerAudio = new AudioSource();
        */

        string settingsFileURL = "GameData/KerbalScienceFoundation/NavInstruments/settings.cfg";

        //Color32 backgroundColorValue = Color.red;
        Texture2D hsi_back = new Texture2D(640, 640);

        public bool DrawMLS(RenderTexture screen, float aspectRatio)
        {
            if (doneLoading == false)
                return false;

            locFlag = gsFlag = bcFlag = false;


            thisVessel = this.vessel;
            Rwy = rwyList[rwyIdx];
            glideSlope = gsList[gsIdx];

            double bearing = NavUtilLib.Utils.CalcBearingToBeacon(thisVessel, Rwy);
            double dme = NavUtilLib.Utils.CalcDistanceToBeacon(thisVessel, Rwy);
            double elevation = NavUtilLib.Utils.CalcElevationAngle(thisVessel, Rwy);
            float locDeviation = NavUtilLib.Utils.CalcLocalizerDeviation(bearing, Rwy);
            float gsDeviation = NavUtilLib.Utils.CalcGlideslopeDeviation(elevation, glideSlope);


            if (locDeviation > 10 && locDeviation < 170 || locDeviation < -10 && locDeviation >-170)
                locFlag = true;

            if (locDeviation < -90 || locDeviation > 90)
            {
                bcFlag = true;
            }
            GL.PushMatrix();

            GL.LoadPixelMatrix(0, screen.width, screen.height, 0);
            GL.Viewport(new Rect(0, 0, screen.width, screen.height));

            screen = NavUtilLib.Graphics.drawMovedImage(mat.Instance.back, screen, new Vector2(0, 0), false,true);

            screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading, new Vector2(.5f, .5f), mat.Instance.headingCard, screen, 0, 0);


            screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading + (float)bearing, new Vector2(.5f, .5f), mat.Instance.NDBneedle, screen, 0, 0);
            screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading + (float)Rwy.hdg, new Vector2(.5f, .5f), mat.Instance.course, screen, 0, 0);

            if (!locFlag)
            {
                float deviationCorrection;
                if (bcFlag)
                {

                    deviationCorrection = ((360 + locDeviation)%360 -180) * 0.078125f;

                    screen = NavUtilLib.Graphics.drawMovedImagePortion(mat.Instance.flag, 0, .3125f, 0, 1, screen, new Vector2(.821875f, .1703125f), false);
                }
                else //not backcourse
                {
                    deviationCorrection = locDeviation * -.078125f;
                }
                deviationCorrection = Mathf.Clamp(deviationCorrection, -0.234375f, 0.234375f);
                screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading + (float)Rwy.hdg, new Vector2(.5f, .5f), mat.Instance.localizer, screen, deviationCorrection, 0);
            }
            else //draw flag
            {
                screen = NavUtilLib.Graphics.drawMovedImagePortion(mat.Instance.localizer, .34375f, .65625f, 0, 1, screen, new Vector2(.821875f, 0.2046875f), false);
            }


            screen = NavUtilLib.Graphics.drawMovedImage(mat.Instance.overlay, screen, new Vector2(0, 0), false,false);

            //marker beacons
            //imageBox takes bottom x, 
            if (dme < 30000)
            {
                //checkmkrbcn
                lastBcnCode = bcnCode;

                var fB = NavUtilLib.Utils.CalcBearingTo(Utils.CalcRadiansFromDeg(Rwy.gsLongitude-thisVessel.longitude),
                                                            Utils.CalcRadiansFromDeg(thisVessel.latitude),
                                                            Utils.CalcRadiansFromDeg(Rwy.gsLatitude));

                bcnCode = inBeaconArea(Utils.CalcLocalizerDeviation(fB,Rwy), thisVessel, Rwy);

                bool drawUnlit = false;

                if (lastBcnCode != bcnCode)
                {
                    timer = Planetarium.GetUniversalTime();

                    switch(bcnCode)
                    {
                        case 1:
                            aud.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KerbalScienceFoundation/NavInstruments/CommonAudio/outer"));
                            break;

                        case 2:
                            aud.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KerbalScienceFoundation/NavInstruments/CommonAudio/middle"));
                            break;

                        case 3:
                            aud.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KerbalScienceFoundation/NavInstruments/CommonAudio/inner"));
                            break;

                        default:
                            break;
                    }




                }


                var deltaTime = Planetarium.GetUniversalTime() - timer;

                switch (bcnCode)
                {
                    case 1:
                        deltaTime = deltaTime % .5f; //.5f is the time in seconds for the whole loop to play
                        if (deltaTime > .375f)
                            drawUnlit = true;
                        break;

                    case 2:
                        deltaTime = deltaTime % .75f; //.75f is the time in seconds for the whole loop to play
                        if (deltaTime > .125f && deltaTime < .25f || deltaTime > .625f)
                            drawUnlit = true;
                        break;

                    case 3:
                        deltaTime = deltaTime % .25f; //.25f is the time in seconds for the whole loop to play
                        if (deltaTime > .125f)
                            drawUnlit = true;
                        break;

                    default:
                        aud.markerAudio.Stop();
                        break;
                }



                if (drawUnlit || bcnCode == 0)
                {
                    screen = NavUtilLib.Graphics.drawMovedImagePortion(mat.Instance.mkrbcn, .75f, 1, 0, 1, screen, new Vector2(.046875f, .0203125f), false); ;
                }
                else
                {
                    switch (bcnCode)
                    {
                        case 1:
                            screen = NavUtilLib.Graphics.drawMovedImagePortion(mat.Instance.mkrbcn, .5f, .75f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                            break;

                        case 2:
                            screen = NavUtilLib.Graphics.drawMovedImagePortion(mat.Instance.mkrbcn, .25f, .5f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                            break;

                        case 3:
                            screen = NavUtilLib.Graphics.drawMovedImagePortion(mat.Instance.mkrbcn, 0f, .25f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                            break;

                        default:
                            break;
                    }
                }
            }
            else
            {
                screen = NavUtilLib.Graphics.drawMovedImagePortion(mat.Instance.mkrbcn, .75f, 1, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
            }

            //find vertical location of pointer
            float yO = -0.3125f * gsDeviation;
            //0.3125 per degree
            yO = Mathf.Clamp(yO, -0.21875f, 0.21875f); //.7 degrees either direction
            yO += 0.3609375f;

            screen = NavUtilLib.Graphics.drawMovedImage(mat.Instance.pointer, screen, new Vector2(0.5f, yO), true,false);
            
            GL.PopMatrix();


            return true;
        }

        private byte inBeaconArea(float locDeviation, Vessel v, Runway r)
        {
            if (3f > locDeviation && locDeviation > -3f)
            {
                var gcd = NavUtilLib.Utils.CalcGreatCircleDistance(v.latitude,v.longitude,r.gsLatitude,r.gsLongitude, r.body);

                if(r.outerMarkerDist + 200 > gcd && r.outerMarkerDist -200 < gcd)
                {
                    return 1;
                }
                if (r.middleMarkerDist + 100 > gcd && r.middleMarkerDist - 100 < gcd)
                {
                    return 2;
                }
                if (r.innerMarkerDist + 50 > gcd && r.innerMarkerDist - 50 < gcd)
                {
                    return 3;
                }
            }
            return 0;
        }

        public string pageAuthor(int screenWidth, int screenHeight)
        {
            Vessel thisVessel = this.vessel;
            Runway Rwy = rwyList[rwyIdx];
            float glideSlope = gsList[gsIdx];

            double bearing = NavUtilLib.Utils.CalcBearingToBeacon(thisVessel, Rwy);
            double dme = NavUtilLib.Utils.CalcDistanceToBeacon(thisVessel, Rwy);
            double elevation = NavUtilLib.Utils.CalcElevationAngle(thisVessel, Rwy);

            string output;

            output = "  Runway: " + Rwy.ident + Environment.NewLine +
                "  Glideslope: " + string.Format("{0:F1}", glideSlope) + "°"
            //"  GS Alt MSL: " + Utils.CalcSurfaceAltAtDME((float)dme,Rwy.body,(float)glideSlope,(float)Rwy.altMSL) +"m"
                + Environment.NewLine +
                "                                     [@x-4][@y7]" + NavUtilLib.Utils.numberFormatter(FlightGlobals.ship_heading, true) + Environment.NewLine +
                "   [@x-4][@y4]" + NavUtilLib.Utils.numberFormatter(Rwy.hdg, true) + "                               " + NavUtilLib.Utils.numberFormatter((float)bearing, true) + Environment.NewLine
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
                + "   [@y16]" + NavUtilLib.Utils.numberFormatter((float)dme / 1000f, false) + Environment.NewLine
                + Environment.NewLine
                + " [@x-5][@y8]               |    Runway    |" + Environment.NewLine
                + " [@x-5]               | Prev | Next  |";

            return output;
        }

        public void ButtonProcessor(int buttonID)
        {
            if (buttonID == 5)
            {
                rwyIdx--;
            }
            if (buttonID == 6)
            {
                rwyIdx++;
            }
            if (buttonID == 1)
            {
                gsIdx--;
            }
            if (buttonID == 0)
            {
                gsIdx++;
            }
            if (buttonID == 2) //print coordinates on Debug console
            {
                var v = this.vessel;
                var r = rwyList[rwyIdx];
                Debug.Log("Lat: " + FlightGlobals.ship_latitude + " Lon: " + FlightGlobals.ship_longitude + " GCD: " + NavUtilLib.Utils.CalcGreatCircleDistance(v.latitude, v.longitude, r.gsLatitude, r.gsLongitude, r.body));
            }

            rwyIdx = indexChecker(rwyIdx, rwyList.Count - 1, 0);
            gsIdx = indexChecker(gsIdx, gsList.Count - 1, 0);

            if (buttonID == 3)
                gsIdx = rwyIdx = 0; //"Back" Key will return the HSI to default runway and GS
        }

        private int indexChecker(int index, int maxIndex, int minIndex)
        {
            if (maxIndex < index)
            {
                index = minIndex;
            }
            else if(minIndex > index)
            {
                index = maxIndex;
            }

            return index;
        }



        public void Start()
        {
            rwyList = NavUtilLib.ConfigLoader.GetRunwayListFromConfig(settingsFileURL);
            gsList = NavUtilLib.ConfigLoader.GetGlideslopeListFromConfig(settingsFileURL);


            //string settingsFolderURL = "GameData/KerbalScienceFoundation/NavInstruments/Runways";
            DirectoryInfo folder = new DirectoryInfo(KSPUtil.ApplicationRootPath + "GameData/KerbalScienceFoundation/NavInstruments/Runways");

            if (folder.Exists)
            {
                FileInfo[] addlRunways = folder.GetFiles("*_rwy.cfg");

                foreach (FileInfo finfo in addlRunways)
                {
                    rwyList.AddRange(NavUtilLib.ConfigLoader.GetRunwayListFromConfig("GameData/KerbalScienceFoundation/NavInstruments/Runways/" + finfo.Name));
                }

            }
            aud.initializeAudio();
            Debug.Log("MLS: Loading Materials...");
            mat.loadMaterials();
            Debug.Log("MLS: Done!");
            doneLoading = true;
        }
    }
}
