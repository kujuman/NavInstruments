using System;
using System.Collections.Generic;
using UnityEngine;
using KSP;

using var = NavUtilLib.GlobalVariables;

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

            private static byte inBeaconArea(float locDeviation, Vessel v, Runway r)
            {
                if (3f > locDeviation && locDeviation > -3f)
                {
                    var gcd = NavUtilLib.Utils.CalcGreatCircleDistance(v.latitude, v.longitude, r.gsLatitude, r.gsLongitude, r.body);

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
                var.FlightData.updateNavigationData();

                locFlag = gsFlag = bcFlag = false;

                if (var.FlightData.locDeviation > 10 && var.FlightData.locDeviation < 170 || var.FlightData.locDeviation < -10 && var.FlightData.locDeviation > -170)
                    locFlag = true;

                if (var.FlightData.locDeviation < -90 || var.FlightData.locDeviation > 90)
                {
                    bcFlag = true;
                }

                GL.PushMatrix();

                GL.LoadPixelMatrix(0, screen.width, screen.height, 0);
                GL.Viewport(new Rect(0, 0, screen.width, screen.height));
                GL.Clear(true, true, Color.black);

                screen = NavUtilLib.Graphics.drawMovedImage(var.Materials.Instance.back, screen, new Vector2(0, 0), false, true);
                screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading, new Vector2(.5f, .5f), var.Materials.Instance.headingCard, screen, 0, 0);
                screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading + (float)var.FlightData.bearing, new Vector2(.5f, .5f), var.Materials.Instance.NDBneedle, screen, 0, 0);
                screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading + (float)var.FlightData.selectedRwy.hdg, new Vector2(.5f, .5f), var.Materials.Instance.course, screen, 0, 0);

                if (!locFlag)
                {
                    float deviationCorrection;
                    if (bcFlag)
                    {
                        deviationCorrection = ((360 + var.FlightData.locDeviation) % 360 - 180) * 0.078125f;
                        screen = NavUtilLib.Graphics.drawMovedImagePortion(var.Materials.Instance.flag, 0, .3125f, 0, 1, screen, new Vector2(.821875f, .1703125f), false);
                    }
                    else //not backcourse
                    {
                        deviationCorrection = var.FlightData.locDeviation * -.078125f;
                    }
                    deviationCorrection = Mathf.Clamp(deviationCorrection, -0.234375f, 0.234375f);
                    screen = NavUtilLib.Graphics.drawCenterRotatedImage(360 - FlightGlobals.ship_heading + (float)var.FlightData.selectedRwy.hdg, new Vector2(.5f, .5f), var.Materials.Instance.localizer, screen, deviationCorrection, 0);
                }
                else //draw flag
                {
                    screen = NavUtilLib.Graphics.drawMovedImagePortion(var.Materials.Instance.flag, .34375f, .65625f, 0, 1, screen, new Vector2(.821875f, 0.2046875f), false);
                }


                screen = NavUtilLib.Graphics.drawMovedImage(var.Materials.Instance.overlay, screen, new Vector2(0, 0), false, false);
                //marker beacons
                //imageBox takes bottom x, 
                if (var.FlightData.dme < 30000)
                {
                    //checkmkrbcn
                    lastBcnCode = bcnCode;

                    var fB = NavUtilLib.Utils.CalcBearingTo(Utils.CalcRadiansFromDeg(var.FlightData.selectedRwy.gsLongitude - var.FlightData.currentVessel.longitude),
                                                                Utils.CalcRadiansFromDeg(var.FlightData.currentVessel.latitude),
                                                                Utils.CalcRadiansFromDeg(var.FlightData.selectedRwy.gsLatitude));

                    bcnCode = inBeaconArea(Utils.CalcLocalizerDeviation(fB, var.FlightData.selectedRwy), var.FlightData.currentVessel,var.FlightData.selectedRwy);

                    bool drawUnlit = false;

                    if (lastBcnCode != bcnCode)
                    {
                        timer = Planetarium.GetUniversalTime();

                        switch (bcnCode)
                        {
                            case 1:
                                var.Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KerbalScienceFoundation/NavInstruments/CommonAudio/outer"));
                                break;

                            case 2:
                                var.Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KerbalScienceFoundation/NavInstruments/CommonAudio/middle"));
                                break;

                            case 3:
                                var.Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip("KerbalScienceFoundation/NavInstruments/CommonAudio/inner"));
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
                            var.Audio.markerAudio.Stop();
                            break;
                    }

                    if (drawUnlit || bcnCode == 0)
                    {
                        screen = NavUtilLib.Graphics.drawMovedImagePortion(var.Materials.Instance.mkrbcn, .75f, 1, 0, 1, screen, new Vector2(.046875f, .0203125f), false); ;
                    }
                    else
                    {
                        switch (bcnCode)
                        {
                            case 1:
                                screen = NavUtilLib.Graphics.drawMovedImagePortion(var.Materials.Instance.mkrbcn, .5f, .75f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                                break;

                            case 2:
                                screen = NavUtilLib.Graphics.drawMovedImagePortion(var.Materials.Instance.mkrbcn, .25f, .5f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                                break;

                            case 3:
                                screen = NavUtilLib.Graphics.drawMovedImagePortion(var.Materials.Instance.mkrbcn, 0f, .25f, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                                break;

                            default:
                                break;
                        }
                    }
                }
                else
                {
                    screen = NavUtilLib.Graphics.drawMovedImagePortion(var.Materials.Instance.mkrbcn, .75f, 1, 0, 1, screen, new Vector2(.046875f, .0203125f), false);
                }

                //find vertical location of pointer
                float yO = -0.3125f * var.FlightData.gsDeviation;
                //0.3125 per degree
                yO = Mathf.Clamp(yO, -0.21875f, 0.21875f); //.7 degrees either direction
                yO += 0.3609375f;

                screen = NavUtilLib.Graphics.drawMovedImage(var.Materials.Instance.pointer, screen, new Vector2(0.5f, yO), true, false);

                GL.PopMatrix();
            }
        }



    
}