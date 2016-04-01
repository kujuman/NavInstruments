//NavUtilities by kujuman, © 2014. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Rwy = NavUtilLib.GlobalVariables.FlightData;

namespace NavUtilGUI
{
    public static class RunwaysEditor // : Monobehaviour
{
        private static Rect windowPos;

        private static Vector2 rwyListVector =  new Vector2(0,0);
        private static float rwyListLength = 0;

        private static NavUtilLib.Runway tempRwy = new NavUtilLib.Runway();
        private static bool isNewRwy = true;
        private static bool useAutoHdg = false;
        private static bool useAutoElevation = false;
        private static bool makeMarkers = false;

        public static void startGUI()
        {
            if (!NavUtilLib.GlobalVariables.Settings.rwyEditorState)
            {
                windowPos = NavUtilLib.GlobalVariables.Settings.rwyEditorGUI;

                if (!NavUtilLib.GlobalVariables.Settings.navAidsIsLoaded)
                {
                    NavUtilLib.GlobalVariables.Settings.loadNavAids();
                }
            }
            else
            {
                NavUtilLib.GlobalVariables.Settings.rwyEditorGUI = windowPos;

                //DestroyAfterTime.DestroyObject(this);
            }

            NavUtilLib.GlobalVariables.Settings.rwyEditorState = !NavUtilLib.GlobalVariables.Settings.rwyEditorState;
        }

        public static void OnDraw()
        {
            if ((windowPos.xMin + windowPos.width) < 20) windowPos.xMin = 20 - windowPos.width;
            if (windowPos.yMin + windowPos.height < 20) windowPos.yMin = 20 - windowPos.height;
            if (windowPos.xMin > Screen.width - 20) windowPos.xMin = Screen.width - 20;
            if (windowPos.yMin > Screen.height - 20) windowPos.yMin = Screen.height - 20;
            windowPos = GUI.Window(450448971, windowPos, OnWindow, "Runway Customizer");
        }

        private static void OnWindow(int winID)
        {
            rwyListLength = Rwy.customRunways.Count * 40;

            rwyListVector = GUI.BeginScrollView(new Rect(10, 20, 150, 270), rwyListVector, new Rect(0, 0, 135, rwyListLength + 35));

            if (GUI.Button(new Rect(2, 0, 130, 30), "Create Runway"))
            {
                tempRwy = new NavUtilLib.Runway();
                isNewRwy = true;
            }

            for (int i = 0; i < Rwy.customRunways.Count; i++)
            {
                if (i == Rwy.cRwyIdx && !isNewRwy)
                {
                    if (GUI.Button(new Rect(2, (i * 40) + 40, 130, 30), "* " + Rwy.customRunways[i].ident))
                    {
                        Rwy.cRwyIdx = i;
                        tempRwy = Rwy.customRunways[i];
                        isNewRwy = false;
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(2, (i * 40) + 40, 130, 30), Rwy.customRunways[i].ident))
                    {
                        Rwy.cRwyIdx = i;
                        tempRwy = Rwy.customRunways[i];
                        isNewRwy = false;
                    }
                }
            }
            GUI.EndScrollView();
            //begin runway info area

            GUI.Label(new Rect(170, 20, 200, 20), "Runway Ident");

            GUI.Label(new Rect(170, 70, 150, 20), "Runway Heading");

            GUI.Label(new Rect(170, 120, 150, 20), "Runway Elevation (m)");



            GUI.Label(new Rect(170, 185, 240, 20), "Marker distance from current point (m)");

            GUI.Label(new Rect(190, 200, 60, 20), " Inner");
            GUI.Label(new Rect(260, 200, 60, 20), " Middle");
            GUI.Label(new Rect(330, 200, 60, 20), " Outer");


            //isNewRwy


            //only difference is textbox vs label
            if (isNewRwy)
            {
                GUI.Label(new Rect(340, 70, 85, 20), "Auto Hdg?");
                useAutoHdg = GUI.Toggle(new Rect(350, 90, 20, 20), useAutoHdg, "");

                GUI.Label(new Rect(340, 120, 85, 20), "Auto Elevation?");
                useAutoElevation = GUI.Toggle(new Rect(350, 140, 20, 20), useAutoElevation, "");


                tempRwy.ident = GUI.TextField(new Rect(170, 40, 200, 20), tempRwy.ident);

                GUI.Label(new Rect(340, 70, 85, 20), "Auto Hdg?");
                useAutoHdg = GUI.Toggle(new Rect(350, 90, 20, 20), useAutoHdg, "");
                if (useAutoHdg)
                {
                    tempRwy.hdg = (float)Math.Round(FlightGlobals.ship_heading, 2);

                    GUI.Label(new Rect(170, 90, 150, 20), tempRwy.hdg.ToString() + "°");
                }
                else
                {
                    tempRwy.hdg = Convert.ToSingle(GUI.TextField(new Rect(170, 90, 150, 20), Convert.ToString(tempRwy.hdg)));
                }

                GUI.Label(new Rect(340, 120, 85, 20), "Auto Alt?");
                useAutoElevation = GUI.Toggle(new Rect(350, 140, 20, 20), useAutoElevation, "");
                if (useAutoElevation)
                {
                    tempRwy.altMSL = (float)Math.Round(FlightGlobals.ActiveVessel.altitude, 1);

                    GUI.Label(new Rect(170, 140, 150, 20), Convert.ToString(tempRwy.altMSL) + "m");
                }
                else
                {
                    tempRwy.altMSL = Convert.ToSingle(GUI.TextField(new Rect(170, 140, 150, 20), Convert.ToString(tempRwy.altMSL)));
                }



                GUI.Label(new Rect(170, 165, 120, 20), "Use Marker Becons?");
                                makeMarkers = GUI.Toggle(new Rect(350, 168, 20, 20), makeMarkers, "");


                if(makeMarkers)
                {
                    tempRwy.innerMarkerDist = Convert.ToSingle(GUI.TextField(new Rect(190, 220, 60, 20), Convert.ToString(tempRwy.innerMarkerDist)));

                    tempRwy.middleMarkerDist=  Convert.ToSingle( GUI.TextField(new Rect(260, 220, 60, 20), Convert.ToString(tempRwy.middleMarkerDist)));
                    tempRwy.outerMarkerDist =  Convert.ToSingle( GUI.TextField(new Rect(330, 220, 60, 20), Convert.ToString(tempRwy.outerMarkerDist)));

                    if (tempRwy.innerMarkerDist < -500)
                        tempRwy.innerMarkerDist = -500;


                    if (tempRwy.middleMarkerDist < -500)
                        tempRwy.middleMarkerDist = -500;


                    if (tempRwy.outerMarkerDist < -500)
                        tempRwy.outerMarkerDist = -500;
                }
            }
            else
            {
                GUI.Label(new Rect(170, 40, 200, 20), tempRwy.ident);

                GUI.Label(new Rect(170, 90, 150, 20), tempRwy.hdg.ToString() + "°");
                GUI.Label(new Rect(170, 140, 150, 20), Convert.ToString(tempRwy.altMSL) + "m");

                if(tempRwy.innerMarkerDist > -500)
                GUI.Label(new Rect(190, 220, 60, 20), tempRwy.innerMarkerDist.ToString() + "m");
                else
                    GUI.Label(new Rect(190, 220, 60, 20), "  N/A");

                if(tempRwy.middleMarkerDist > -500)
                GUI.Label(new Rect(260, 220, 60, 20), tempRwy.middleMarkerDist.ToString() +"m");
                else
                    GUI.Label(new Rect(260, 220, 60, 20), "  N/A");

                if(tempRwy.outerMarkerDist > -500)
                GUI.Label(new Rect(330, 220, 60, 20), tempRwy.outerMarkerDist.ToString() + "m");
                else
                    GUI.Label(new Rect(330, 220, 60, 20), "  N/A");
            }

            if (isNewRwy)
            {
                if (GUI.Button(new Rect(170, 250, 200, 20), "Create Runway"))
                {
                    //create the runway and add to database
                    tempRwy.body = FlightGlobals.currentMainBody.bodyName;
                    tempRwy.gsLatitude = (float)FlightGlobals.ship_latitude;
                    tempRwy.gsLongitude = (float)FlightGlobals.ship_longitude;

                    var loc = NavUtilLib.Utils.CalcCoordinatesFromInitialPointBearingDistance(new Vector2d(tempRwy.gsLatitude, tempRwy.gsLongitude), tempRwy.hdg, 1000, FlightGlobals.currentMainBody.Radius);

                    tempRwy.locLatitude = (float)loc.x;
                    tempRwy.locLongitude = (float)loc.y;

                    if (!makeMarkers)
                    {
                        tempRwy.innerMarkerDist = -1000;
                        tempRwy.middleMarkerDist = -1000;
                        tempRwy.outerMarkerDist = -1000;
                    }


                    Rwy.customRunways.Add(tempRwy);
                    Rwy.rwyList.Add(tempRwy);

                    Rwy.cRwyIdx = Rwy.customRunways.FindIndex(r => r.ident == tempRwy.ident);

                    WriteCustomRwys();

                    isNewRwy = false;
                }
            }
            else
            {
                //show delete button
                if(GUI.Button(new Rect(170, 250, 200, 20),"Delete This Runway"))
                {
                    Rwy.rwyList.Remove(Rwy.customRunways[Rwy.cRwyIdx]);
                    Rwy.customRunways.Remove(Rwy.customRunways[Rwy.cRwyIdx]);

                    WriteCustomRwys();

                    Rwy.cRwyIdx = 0;

                    isNewRwy = true;
                    tempRwy = new NavUtilLib.Runway();
                }
            }
            GUI.DragWindow();
        }

        public static void WriteCustomRwys()
        {
            NavUtilLib.ConfigLoader.WriteCustomRunwaysToConfig(NavUtilLib.GlobalVariables.FlightData.customRunways, "custom.rwy");

            NavUtilLib.GlobalVariables.FlightData.updateNavigationData();
        }

    }
}
