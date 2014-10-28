//NavUtilities by kujuman, © 2014. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NavUtilLib
{
    public static class SettingsGUI
    {
        public static Rect guiwindowPosition;
        public static bool isActive = false;

        public static void startSettingsGUI()
        {
            if (!isActive)
            {
                RenderingManager.AddToPostDrawQueue(3, OnDraw);
                guiwindowPosition = NavUtilLib.GlobalVariables.Settings.settingsGUI;

                if (!NavUtilLib.GlobalVariables.Settings.navAidsIsLoaded)
                {
                    NavUtilLib.GlobalVariables.Settings.loadNavAids();
                }
                isActive = !isActive;
            }
            else
            {
                RenderingManager.RemoveFromPostDrawQueue(3, OnDraw);
                NavUtilLib.GlobalVariables.Settings.settingsGUI = guiwindowPosition;
                isActive = !isActive;
            }
        }


        private static void OnDraw()
        {
            if ((guiwindowPosition.xMin + guiwindowPosition.width) < 20) guiwindowPosition.xMin = 20 - guiwindowPosition.width;
            if (guiwindowPosition.yMin + guiwindowPosition.height < 20) guiwindowPosition.yMin = 20 - guiwindowPosition.height;
            if (guiwindowPosition.xMin > Screen.width - 20) guiwindowPosition.xMin = Screen.width - 20;
            if (guiwindowPosition.yMin > Screen.height - 20) guiwindowPosition.yMin = Screen.height - 20;
            guiwindowPosition = GUI.Window(594, guiwindowPosition, OnWindow, "NavUtil Settings");
        }

        private static void OnWindow(int winID)
        {
            if(GUI.Button(new Rect(5,15,115,20),"Previous Runway"))
            {
                NavUtilLib.GlobalVariables.FlightData.rwyIdx--;
                NavUtilLib.GlobalVariables.FlightData.rwyIdx = NavUtilLib.Utils.indexChecker(NavUtilLib.GlobalVariables.FlightData.rwyIdx, NavUtilLib.GlobalVariables.FlightData.rwyList.Count()-1, 0);
            }

            if (GUI.Button(new Rect(130, 15, 115, 20), "Next Runway"))
            {
                NavUtilLib.GlobalVariables.FlightData.rwyIdx++;
                NavUtilLib.GlobalVariables.FlightData.rwyIdx = NavUtilLib.Utils.indexChecker(NavUtilLib.GlobalVariables.FlightData.rwyIdx, NavUtilLib.GlobalVariables.FlightData.rwyList.Count() - 1, 0);
            }

            if (GUI.Button(new Rect(5, 40, 115, 20), "Previous G/S"))
            {
                NavUtilLib.GlobalVariables.FlightData.gsIdx--;
                NavUtilLib.GlobalVariables.FlightData.gsIdx=NavUtilLib.Utils.indexChecker(NavUtilLib.GlobalVariables.FlightData.gsIdx, NavUtilLib.GlobalVariables.FlightData.gsList.Count() - 1, 0);
            }

            if (GUI.Button(new Rect(130, 40, 115, 20), "Next G/S"))
            {
                NavUtilLib.GlobalVariables.FlightData.gsIdx++;
                NavUtilLib.GlobalVariables.FlightData.gsIdx=NavUtilLib.Utils.indexChecker(NavUtilLib.GlobalVariables.FlightData.gsIdx, NavUtilLib.GlobalVariables.FlightData.gsList.Count() - 1, 0);
            }

            GUI.Label(new Rect(5, 75, 115, 25), "HSI GUI scale");

            if (GUI.Button(new Rect(130, 75, 100, 25), "Default Scale"))
            {
                NavUtilLib.GlobalVariables.Settings.hsiGUIscale = 0.5f;


            }

            if (GUI.Button(new Rect(5, 150, 115, 20), "Custom Rwys"))
                NavUtilLib.GlobalVariables.Settings.rE.startGUI();

            //GUI.Label(new Rect(125, 150, 90, 20), "Popup in IVA?");

            GlobalVariables.Settings.enableWindowsInIVA = GUI.Toggle(new Rect(125, 150, 120, 20), GlobalVariables.Settings.enableWindowsInIVA, "Popup in IVA?");


            NavUtilLib.GlobalVariables.Settings.hsiGUIscale = GUI.HorizontalSlider(new Rect(5, 105, 240, 30),NavUtilLib.GlobalVariables.Settings.hsiGUIscale, 0.1f, 1.0f);

            GUI.DragWindow();
        }

    }
}
