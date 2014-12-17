//NavUtilities by kujuman, © 2014. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using KSP;

using gvar = NavUtilLib.GlobalVariables;

namespace NavUtilLib
{

        public static class DisplayData
        {
            public static byte bcnCode = 0;
            public static byte lastBcnCode;

            private static bool gsFlag;
            private static bool bcFlag;
            private static bool locFlag;

            public static double timer;

            private static byte inBeaconArea(double locDeviation, Vessel v, Runway r)
            {
                if (3d > locDeviation && locDeviation > -3d)
                {
                    double gcd = NavUtilLib.Utils.CalcGreatCircleDistance(v.latitude, v.longitude, r.gsLatitude, r.gsLongitude, r.body);

                    if (r.outerMarkerDist + 200 > gcd && r.outerMarkerDist - 200 < gcd)
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

            public static void DrawHSI(RenderTexture screen, float aspectRatio)
            {
			gvar.FlightData.updateNavigationData();

                locFlag = bcFlag = false;
                gsFlag = true; 



			if (gvar.FlightData.locDeviation > 10 && gvar.FlightData.locDeviation < 170 || gvar.FlightData.locDeviation < -10 && gvar.FlightData.locDeviation > -170)
                    locFlag = true;

			if (gvar.FlightData.locDeviation < -90 || gvar.FlightData.locDeviation > 90)
                {
                    bcFlag = true;
                }

                GL.PushMatrix();

                GL.LoadPixelMatrix(0, screen.width, screen.height, 0);
                GL.Viewport(new Rect(0, 0, screen.width, screen.height));
                GL.Clear(true, true, Color.black);

			screen = NavUtilLib.Graphics.drawMovedImage(gvar.Materials.Instance.back, screen, new Vector2(0, 0), false, true);
			screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading, new Vector2(.5f, .5f), gvar.Materials.Instance.headingCard, screen, 0, 0);
			screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading + (float)gvar.FlightData.bearing, new Vector2(.5f, .5f), gvar.Materials.Instance.NDBneedle, screen, 0, 0);
			screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading + (float)gvar.FlightData.runwayHeading, new Vector2(.5f, .5f), gvar.Materials.Instance.course, screen, 0, 0);

                bool fineLoc = false;

                if (!locFlag)
                {

                    if (NavUtilLib.GlobalVariables.Settings.enableFineLoc && NavUtilLib.GlobalVariables.FlightData.dme < 7500)
                    {
                            fineLoc = true;
                    }

                    double deviationCorrection;
                    if (bcFlag)
                    {
                        deviationCorrection = ((360d + gvar.FlightData.locDeviation) % 360d - 180d) * 0.078125d;
                        screen = NavUtilLib.Graphics.drawMovedImagePortion(gvar.Materials.Instance.flag, 0, .3125f, 0, 1, screen, new Vector2(.821875f, .1703125f), false);
                    }
                    else //not backcourse
                    {
                        deviationCorrection = gvar.FlightData.locDeviation * -.078125d;
                    }
                    //if fineLoc == false then we use course guidance mode. In this mode each tick on Loc is 1°. in fine guidance mode each tick is 0.25°

                    string locMode = "Loc→Coarse Mode";

                    if (fineLoc && Math.Abs(NavUtilLib.GlobalVariables.FlightData.locDeviation) < 0.75d)
                    {
                        deviationCorrection *= 4d; //we're magnifying the needle deflection, which increases sensetivity

                        //now to inform the user that fine control is enabled
                        locMode = "Loc→Fine Mode";

                        //change the color of the localizer needle/font?
                        NavUtilLib.GlobalVariables.Materials.Instance.whiteFont.color = Color.magenta;
                        screen = NavUtilLib.TextWriter.addTextToRT(screen, locMode, new Vector2(380, screen.height - 570), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, 0.5f);
                        NavUtilLib.GlobalVariables.Materials.Instance.whiteFont.color = Color.white;

                        NavUtilLib.GlobalVariables.Materials.Instance.localizer.color = Color.yellow;

                        NavUtilLib.GlobalVariables.Materials.Instance.NDBneedle.color = Color.clear;
                    }
                    else
                    {
                        fineLoc = false;

                        NavUtilLib.GlobalVariables.Materials.Instance.localizer.color = Color.magenta;
                        screen = NavUtilLib.TextWriter.addTextToRT(screen, locMode, new Vector2(380, screen.height - 570), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, 0.5f);
                    }

                    //deviationCorrection = Math.Clamp(deviationCorrection, -0.234375d, 0.234375d);
					if (deviationCorrection < -0.234375d)
						deviationCorrection = -0.234375d;
					else if (deviationCorrection > 0.234375d)
						deviationCorrection = 0.234375d;


					screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading + (float)gvar.FlightData.runwayHeading, new Vector2(.5f, .5f), gvar.Materials.Instance.localizer, screen, (float)deviationCorrection, 0);
                }
                else //draw flag
                {
                    screen = NavUtilLib.Graphics.drawMovedImagePortion(gvar.Materials.Instance.flag, .34375f, .65625f, 0, 1, screen, new Vector2(.821875f, 0.2046875f), false);
                }

                if(locFlag || !fineLoc)
                    NavUtilLib.GlobalVariables.Materials.Instance.NDBneedle.color = Color.white;


                screen = NavUtilLib.Graphics.drawMovedImage(gvar.Materials.Instance.overlay, screen, new Vector2(0, 0), false, false);
                //marker beacons
                //imageBox takes bottom x, 
                if (gvar.FlightData.dme < 200000)
                {
				/*
                    double fB = NavUtilLib.Utils.CalcBearingTo(Utils.CalcRadiansFromDeg(gvar.FlightData.selectedRwy.gsLongitude - gvar.FlightData.currentVessel.longitude),
                                            Utils.CalcRadiansFromDeg(gvar.FlightData.currentVessel.latitude),
                                            Utils.CalcRadiansFromDeg(gvar.FlightData.selectedRwy.gsLatitude));
				*/
				double gsHorDev = Utils.CalcLocalizerDeviation(gvar.FlightData.currentVessel,gvar.FlightData.selectedRwy);

                    if (Math.Abs(gsHorDev) < 25d)
                        gsFlag = false;


                    //checkmkrbcn
                    lastBcnCode = bcnCode;

                    bcnCode = inBeaconArea(gsHorDev, gvar.FlightData.currentVessel,gvar.FlightData.selectedRwy);

                    bool drawUnlit = false;

                    if (lastBcnCode != bcnCode)
                    {
                        timer = Planetarium.GetUniversalTime();

                        switch (bcnCode)
                        {
                            case 1:
                                gvar.Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KerbalScienceFoundation/NavInstruments/CommonAudio/outer"));
                                break;

                            case 2:
                                gvar.Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KerbalScienceFoundation/NavInstruments/CommonAudio/middle"));
                                break;

                            case 3:
                                gvar.Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KerbalScienceFoundation/NavInstruments/CommonAudio/inner"));
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
                            gvar.Audio.markerAudio.Stop();
                            break;
                    }

                    if (drawUnlit || bcnCode == 0)
                    {
                        screen = NavUtilLib.Graphics.drawMovedImagePortion(gvar.Materials.Instance.mkrbcn, .75f, 1, 0, 1, screen, new Vector2(.046875f, .0203125f), false); ;
                    }
                    else
                    {
                        switch (bcnCode)
                        {
                            case 1:
                                screen = NavUtilLib.Graphics.drawMovedImagePortion(gvar.Materials.Instance.mkrbcn, .5f, .75f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                                break;

                            case 2:
                                screen = NavUtilLib.Graphics.drawMovedImagePortion(gvar.Materials.Instance.mkrbcn, .25f, .5f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                                break;

                            case 3:
                                screen = NavUtilLib.Graphics.drawMovedImagePortion(gvar.Materials.Instance.mkrbcn, 0f, .25f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                                break;

                            default:
                                break;
                        }
                    }
                }
                else
                {
                    screen = NavUtilLib.Graphics.drawMovedImagePortion(gvar.Materials.Instance.mkrbcn, .75f, 1, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                }

                //find vertical location of pointer
                double yO = -0.3125d * gvar.FlightData.gsDeviation;
                //0.3125 per degree
                //yO = Mathf.Clamp(yO, -0.21875f, 0.21875f); //.7 degrees either direction
				if (yO < -0.21875d)
					yO = -0.21875d;
				else if (yO > 0.21875d)
					yO = 0.21875d;

                yO += 0.3609375d;


                if (gsFlag)
                    screen = NavUtilLib.Graphics.drawMovedImagePortion(gvar.Materials.Instance.flag, .65625f, 1, 0, 1, screen, new Vector2(.821875f, 0.2390625f), false);
                else
                    screen = NavUtilLib.Graphics.drawMovedImage(gvar.Materials.Instance.pointer, screen, new Vector2(0.5f, (float)yO), true, false);

                GL.PopMatrix();
            }
        }



    
}