using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;
using KSP;
using NavUtilLib;

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

        private GameObject audioplayer = new GameObject();
        private AudioSource markerAudio = new AudioSource();


        string settingsFileURL = "GameData/KerbalScienceFoundation/NavInstruments/settings.cfg";

        //Color32 backgroundColorValue = Color.red;
        Texture2D hsi_back = new Texture2D(640, 640);

        private Material overlayMaterial;
        private Material pointerMaterial;
        private Material headingCardMaterial;
        private Material NDBneedleMaterial;
        private Material courseMaterial;
        private Material localizerMaterial;
        private Material mkrbcnMaterial;
        private Material flagMaterial;
        //private string staticOverlay = "KerbalScienceFoundation/NavInstruments/MFD/Textures/troll.png";

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

            //Debug.Log("Loc Deviation: " + locDeviation);

            if (locDeviation > 10 && locDeviation < 170 || locDeviation < -10 && locDeviation >-170)
                locFlag = true;

            if (locDeviation < -90 || locDeviation > 90)
            {
                bcFlag = true;
            }

            Graphics.Blit(hsi_back, screen);

            GL.PushMatrix();

            GL.LoadPixelMatrix(0, screen.width, screen.height, 0);
            GL.Viewport(new Rect(0, 0, screen.width, screen.height));

            drawCenterRotatedImage(360-FlightGlobals.ship_heading, new Vector2(.5f, .5f), headingCardMaterial, screen,0,0);
            drawCenterRotatedImage(360 -FlightGlobals.ship_heading + (float)bearing, new Vector2(.5f, .5f), NDBneedleMaterial, screen,0,0);
            drawCenterRotatedImage(360 - FlightGlobals.ship_heading + (float)Rwy.hdg,new Vector2(.5f, .5f),courseMaterial,screen,0,0);




            if (!locFlag)
            {
                float deviationCorrection;
                if (bcFlag)
                {

                    deviationCorrection = ((360 + locDeviation)%360 -180) * 0.078125f;

                    drawMovedImagePortion(flagMaterial, 0, .3125f, 0, 1, screen, new Vector2(.821875f, .1703125f), false);
                }
                else //not backcourse
                {
                    deviationCorrection = locDeviation * -.078125f;
                }
                deviationCorrection = Mathf.Clamp(deviationCorrection, -0.234375f, 0.234375f);
                drawCenterRotatedImage(360 - FlightGlobals.ship_heading + (float)Rwy.hdg, new Vector2(.5f, .5f), localizerMaterial, screen, deviationCorrection, 0);
            }
            else //draw flag
            {
                drawMovedImagePortion(flagMaterial, .34375f, .65625f, 0, 1, screen, new Vector2(.821875f, 0.2046875f), false);
            }


            drawMovedImage(overlayMaterial, screen, new Vector2(0, 0),false);

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
                            markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KSFHSI/Sounds/outer"));
                            break;

                        case 2:
                            markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KSFHSI/Sounds/middle"));
                            break;

                        case 3:
                            markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KSFHSI/Sounds/inner"));
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
                        markerAudio.Stop();
                        break;
                }



                if (drawUnlit || bcnCode == 0)
                {
                    drawMovedImagePortion(mkrbcnMaterial, .75f, 1, 0, 1, screen, new Vector2(.046875f, .0203125f), false); ;
                }
                else
                {
                    switch (bcnCode)
                    {
                        case 1:
                            drawMovedImagePortion(mkrbcnMaterial, .5f, .75f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                            //if (!markerAudio.isPlaying) markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KSFHSI/Sounds/outer"));
                            break;

                        case 2:
                            drawMovedImagePortion(mkrbcnMaterial, .25f, .5f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                            //if (!markerAudio.isPlaying) markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KSFHSI/Sounds/middle"));
                            break;

                        case 3:
                            drawMovedImagePortion(mkrbcnMaterial, 0f, .25f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                            //if (!markerAudio.isPlaying) markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KSFHSI/Sounds/inner"));
                            break;

                        default:
                            break;
                    }
                }
            }
            else
            {
                drawMovedImagePortion(mkrbcnMaterial, .75f, 1, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
            }

            //find vertical location of pointer
            float yO = -0.3125f * gsDeviation;
            //0.3125 per degree
            yO = Mathf.Clamp(yO, -0.21875f, 0.21875f); //.7 degrees either direction
            yO += 0.3609375f;

            drawMovedImage(pointerMaterial, screen, new Vector2(0.5f, yO), true);
            
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




        private static void drawCenterRotatedImage(float headingDeg, Vector2 centerPercent, Material mat, RenderTexture screen, float xOffset, float yOffset)
        {
            float w = (float)mat.mainTexture.width / (float)screen.width / 2;
            float h = (float)mat.mainTexture.height / (float)screen.height / 2;

            headingDeg = (float)NavUtilLib.Utils.makeAngle0to360(headingDeg);
            headingDeg = (float)NavUtilLib.Utils.CalcRadiansFromDeg(headingDeg);

            Vector2[] corners = new Vector2[4];

            corners[0] = new Vector2(-w + xOffset, -h + yOffset);
            corners[1] = new Vector2(+w + xOffset, -h + yOffset);
            corners[2] = new Vector2(+w + xOffset, +h + yOffset);
            corners[3] = new Vector2(-w + xOffset, +h + yOffset);

            for (int i = 0; i < corners.Count(); i++)
            {
                //=COS($K$3)*B2+SIN($K$3)*C2
                float x = corners[i].x;
                corners[i].x = (Mathf.Cos(headingDeg) * corners[i].x) + (Mathf.Sin(headingDeg) * corners[i].y);

                //=COS($K$3)*C2-SIN($K$3)*B2
                corners[i].y = (Mathf.Cos(headingDeg) * corners[i].y) - (Mathf.Sin(headingDeg) * x);

            }

            mat.SetPass(0);
            GL.LoadOrtho();
            GL.Color(Color.red);
            GL.Begin(GL.QUADS);

            GL.TexCoord2(0, 0);
            GL.Vertex3(centerPercent.x + corners[0].x, centerPercent.y + corners[0].y, 0);

            GL.TexCoord2(1, 0);
            GL.Vertex3(centerPercent.x + corners[1].x, centerPercent.y + corners[1].y, 0);

            GL.TexCoord2(1, 1);
            GL.Vertex3(centerPercent.x + corners[2].x, centerPercent.y + corners[2].y, 0);

            GL.TexCoord2(0, 1);
            GL.Vertex3(centerPercent.x + corners[3].x, centerPercent.y + corners[3].y, 0);

            GL.End();
        }

        private static void drawMovedImage(Material mat, RenderTexture screen, Vector2 bottomLeftPercent, bool useCenterOfMat)
        {
            float w = (float)mat.mainTexture.width / (float)screen.width;
            float h = (float)mat.mainTexture.height / (float)screen.height;

            if (useCenterOfMat)
            {
                bottomLeftPercent.x -= .5f * w;
                bottomLeftPercent.y -= .5f * h;
            }

            mat.SetPass(0);
            GL.LoadOrtho();
            GL.Color(Color.red);
            GL.Begin(GL.QUADS);

            GL.TexCoord2(0, 0);
            GL.Vertex3(bottomLeftPercent.x, bottomLeftPercent.y, 0);

            GL.TexCoord2(0, 1);
            GL.Vertex3(bottomLeftPercent.x, h + bottomLeftPercent.y, 0);

            GL.TexCoord2(1, 1);
            GL.Vertex3(w + bottomLeftPercent.x, h + bottomLeftPercent.y, 0);

            GL.TexCoord2(1, 0);
            GL.Vertex3(w + bottomLeftPercent.x, bottomLeftPercent.y, 0);
            GL.End();
        }

        private static void drawMovedImagePortion(Material mat,float bottom, float top, float left, float right, RenderTexture screen, Vector2 bottomLeftPercent, bool useCenterOfMat)
        {
            float w = (float)mat.mainTexture.width / (float)screen.width;
            float h = (float)mat.mainTexture.height / (float)screen.height;


            w *= (right - left);
            h *= (top - bottom);

            if (useCenterOfMat)
            {
                bottomLeftPercent.x -= .5f * w;
                bottomLeftPercent.y -= .5f * h;
            }

            mat.SetPass(0);
            GL.LoadOrtho();
            GL.Color(Color.red);
            GL.Begin(GL.QUADS);

            GL.TexCoord2(left, bottom);
            GL.Vertex3(bottomLeftPercent.x, bottomLeftPercent.y, 0);

            GL.TexCoord2(left, top);
            GL.Vertex3(bottomLeftPercent.x, h + bottomLeftPercent.y, 0);

            GL.TexCoord2(right, top);
            GL.Vertex3(w + bottomLeftPercent.x, h + bottomLeftPercent.y, 0);

            GL.TexCoord2(right, bottom);
            GL.Vertex3(w + bottomLeftPercent.x, bottomLeftPercent.y, 0);

            GL.End();
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
                "                                     [@x-4][@y7]" + numberFormatter(FlightGlobals.ship_heading, true) + Environment.NewLine +
                "   [@x-4][@y4]" + numberFormatter(Rwy.hdg, true) + "                               " + numberFormatter((float)bearing, true) + Environment.NewLine
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
                + "   [@y16]" + numberFormatter((float)dme/1000f, false) + Environment.NewLine
                + Environment.NewLine
                + " [@x-5][@y8]               |    Runway    |" + Environment.NewLine
                + " [@x-5]               | Prev | Next  |";

            return output;
        }

        private string numberFormatter(float numToDisplay, bool isHeading)
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







            Texture2D hsi_overlay = new Texture2D(640, 640);
            Texture2D hsi_heading_card = new Texture2D(501, 501);
            Texture2D hsi_GS_pointer = new Texture2D(640, 24);
            Texture2D hsi_course_needle = new Texture2D(221, 481);
            Texture2D hsi_course_deviation_needle = new Texture2D(5, 251);
            Texture2D hsi_NDB_needle = new Texture2D(15, 501);

            Texture2D hsi_flags = new Texture2D(64, 64);

            Texture2D hsi_mkr_bcn = new Texture2D(175, 180);

            Byte[] arrBytes;
            arrBytes = KSP.IO.File.ReadAllBytes<KSF_MLS>("overlay.png");
            hsi_overlay.LoadImage(arrBytes);

            arrBytes = KSP.IO.File.ReadAllBytes<KSF_MLS>("large_heading_card.png");
            hsi_heading_card.LoadImage(arrBytes);

            arrBytes = KSP.IO.File.ReadAllBytes<KSF_MLS>("gs_pointer.png");
            hsi_GS_pointer.LoadImage(arrBytes);

            arrBytes = KSP.IO.File.ReadAllBytes<KSF_MLS>("hsi_back.png");
            hsi_back.LoadImage(arrBytes);

            arrBytes = KSP.IO.File.ReadAllBytes<KSF_MLS>("NDB_needle.png");
            hsi_NDB_needle.LoadImage(arrBytes);

            arrBytes = KSP.IO.File.ReadAllBytes<KSF_MLS>("course_needle.png");
            hsi_course_needle.LoadImage(arrBytes);

            arrBytes = KSP.IO.File.ReadAllBytes<KSF_MLS>("course_deviation_needle.png");
            hsi_course_deviation_needle.LoadImage(arrBytes);

            arrBytes = KSP.IO.File.ReadAllBytes<KSF_MLS>("markerIndicator.png");
            hsi_mkr_bcn.LoadImage(arrBytes);

            arrBytes = KSP.IO.File.ReadAllBytes<KSF_MLS>("flags.png");
            hsi_flags.LoadImage(arrBytes);


            Shader unlit = Shader.Find("KSP/Alpha/Unlit Transparent");

            overlayMaterial = new Material(unlit);
            overlayMaterial.color = new Color(1, 1, 1, 1);
            overlayMaterial.mainTexture = (Texture)hsi_overlay;

            pointerMaterial = new Material(unlit);
            pointerMaterial.color = new Color(1, 1, 1, 1);
            pointerMaterial.mainTexture = hsi_GS_pointer;

            headingCardMaterial = new Material(unlit);
            headingCardMaterial.color = new Color(1, 1, 1, 1);
            headingCardMaterial.mainTexture = hsi_heading_card;

            NDBneedleMaterial = new Material(unlit);
            NDBneedleMaterial.color = new Color(1, 1, 1, 1);
            NDBneedleMaterial.mainTexture = hsi_NDB_needle;

            courseMaterial = new Material(unlit);
            courseMaterial.color = new Color(1, 1, 1, 1);
            courseMaterial.mainTexture = hsi_course_needle;

            localizerMaterial = new Material(unlit);
            localizerMaterial.color = new Color(1, 1, 1, 1);
            localizerMaterial.mainTexture = hsi_course_deviation_needle;

            mkrbcnMaterial = new Material(unlit);
            mkrbcnMaterial.color = new Color(1, 1, 1, 1);
            mkrbcnMaterial.mainTexture = hsi_mkr_bcn;

            flagMaterial = new Material(unlit);
            flagMaterial.color = new Color(1, 1, 1, 1);
            flagMaterial.mainTexture = hsi_flags;


            markerAudio = audioplayer.AddComponent<AudioSource>();
            markerAudio.volume = GameSettings.VOICE_VOLUME;
            markerAudio.pan = 0;
            markerAudio.dopplerLevel = 0;
            markerAudio.bypassEffects=true;
            markerAudio.loop = true;

            doneLoading = true;
        }

       

    }
}
