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
        public static Rect winPos;
        public static bool isActive = false;

        public static void startSettingsGUI()
        {
            if (!isActive)
            {

                SettingsGUI.winPos = NavUtilLib.GlobalVariables.Settings.settingsGUI;

                if (!NavUtilLib.GlobalVariables.Settings.navAidsIsLoaded)
                {
                    NavUtilLib.GlobalVariables.Settings.loadNavAids();
                }
                isActive = !isActive;
            }
            else
            {

                NavUtilLib.GlobalVariables.Settings.settingsGUI = SettingsGUI.winPos;
                isActive = !isActive;
            }
        }


        public static void OnDraw()
        {
                if ((SettingsGUI.winPos.xMin + SettingsGUI.winPos.width) < 20) SettingsGUI.winPos.xMin = 20 - SettingsGUI.winPos.width;
                if (SettingsGUI.winPos.yMin + SettingsGUI.winPos.height < 20) SettingsGUI.winPos.yMin = 20 - SettingsGUI.winPos.height;
                if (SettingsGUI.winPos.xMin > Screen.width - 20) SettingsGUI.winPos.xMin = Screen.width - 20;
                if (SettingsGUI.winPos.yMin > Screen.height - 20) SettingsGUI.winPos.yMin = Screen.height - 20;
                SettingsGUI.winPos = GUI.Window(206574909, SettingsGUI.winPos, OnWindow, "NavUtil Settings");
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
                NavUtilGUI.RunwaysEditor.startGUI();

            //GUI.Label(new Rect(125, 150, 90, 20), "Popup in IVA?");

            GlobalVariables.Settings.enableWindowsInIVA = GUI.Toggle(new Rect(125, 150, 120, 20), GlobalVariables.Settings.enableWindowsInIVA, "Popup in IVA?");


            NavUtilLib.GlobalVariables.Settings.hsiGUIscale = GUI.HorizontalSlider(new Rect(5, 105, 240, 30),NavUtilLib.GlobalVariables.Settings.hsiGUIscale, 0.1f, 1.0f);

            GUI.DragWindow();
        }

    }
}
