//NavUtilities by kujuman, © 2014. All Rights Reserved.

using System;
using KSP;
using UnityEngine;

namespace NavUtilLib
{
    public static class ConfigLoader
    {
        public static System.Collections.Generic.List<Runway> GetRunwayListFromConfig(string sSettingURL)
        {
            System.Collections.Generic.List<Runway> runwayList = new System.Collections.Generic.List<Runway>();
            runwayList.Clear();

            ConfigNode runways = ConfigNode.Load(KSPUtil.ApplicationRootPath + sSettingURL);
            foreach (ConfigNode node in runways.GetNodes("Runway"))
            {
                if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: Found Runway Node");

                try
                {
                    Runway rwy = new Runway();

                    rwy.ident = node.GetValue("ident");

                    if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: Loading " + rwy.ident);

                    
                    rwy.shortID = node.GetValue("shortID");
                    if (rwy.shortID.Length > 4)
                        rwy.shortID.Remove(4);

                    rwy.hdg = float.Parse(node.GetValue("hdg"));
                    rwy.body = node.GetValue("body");
                    rwy.altMSL = float.Parse(node.GetValue("altMSL"));
                    rwy.gsLatitude = float.Parse(node.GetValue("gsLatitude"));
                    rwy.gsLongitude = float.Parse(node.GetValue("gsLongitude"));
                    rwy.locLatitude = float.Parse(node.GetValue("locLatitude"));
                    rwy.locLongitude = float.Parse(node.GetValue("locLongitude"));

                    rwy.outerMarkerDist = float.Parse(node.GetValue("outerMarkerDist"));
                    rwy.middleMarkerDist = float.Parse(node.GetValue("middleMarkerDist"));
                    rwy.innerMarkerDist = float.Parse(node.GetValue("innerMarkerDist"));

                    runwayList.Add(rwy);

                    if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: Found " + rwy.ident);
                }
                catch (Exception)
                {
                    if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: Error loading runway");
                    throw;
                }

            }

            return runwayList;
        }

        public static void WriteCustomRunwaysToConfig(System.Collections.Generic.List<Runway> runwayList, string fileName)
        {
            ConfigNode runways = new ConfigNode();
            foreach (Runway r in runwayList)
            {
                ConfigNode rN = new ConfigNode();

                rN.name = "Runway";

                rN.AddValue("ident", r.ident);
                rN.AddValue("shortID", r.shortID);
                rN.AddValue("hdg", r.hdg);
                rN.AddValue("body", r.body);
                rN.AddValue("altMSL", r.altMSL);
                rN.AddValue("gsLatitude", r.gsLatitude);
                rN.AddValue("gsLongitude", r.gsLongitude);
                rN.AddValue("locLatitude", r.locLatitude);
                rN.AddValue("locLongitude", r.locLongitude);

                rN.AddValue("outerMarkerDist", r.outerMarkerDist);
                rN.AddValue("middleMarkerDist", r.middleMarkerDist);
                rN.AddValue("innerMarkerDist", r.innerMarkerDist);

                runways.AddNode(rN);
            }

            runways.Save(KSPUtil.ApplicationRootPath + "GameData/KerbalScienceFoundation/NavInstruments/Runways/" + fileName, "CustomRunways");
        }

        public static System.Collections.Generic.List<float> GetGlideslopeListFromConfig(string sSettingURL)
        {
            System.Collections.Generic.List<float> gsList = new System.Collections.Generic.List<float>();
            gsList.Clear();

            ConfigNode gs = ConfigNode.Load(KSPUtil.ApplicationRootPath + sSettingURL);
            foreach (ConfigNode node in gs.GetNodes("Glideslope"))
            {
                float f = new float();

                f = float.Parse(node.GetValue("glideslope"));

                gsList.Add(f);
            }
            return gsList;
        }

        public static void LoadSettings(string sSettingURL)
        {
            Debug.Log("NavUtil: Loading Settings");

            ConfigNode settings = ConfigNode.Load(KSPUtil.ApplicationRootPath + sSettingURL);

            foreach (ConfigNode node in settings.GetNodes("NavUtilSettings"))
            {
                try
                {
                    GlobalVariables.Settings.hsiGUIscale = float.Parse(node.GetValue("guiScale"));
                }
                catch (Exception)
                {
                    Debug.Log("NavUtil: Error loading guiScale from config");
                    throw;
                }
                try
                {
                    GlobalVariables.Settings.enableFineLoc = bool.Parse(node.GetValue("enableFineLoc"));
                }
                catch (Exception)
                {
                    Debug.Log("NavUtil: Error loading enableFineLoc from config");
                    throw;
                }
                try
                {
                    GlobalVariables.Settings.enableWindowsInIVA = bool.Parse(node.GetValue("enableWindowsInIVA"));
                }
                catch (Exception)
                {
                    Debug.Log("NavUtil: Error loading enableWindowsInIVA from config");
                    throw;
                }
                try
                {
                    GlobalVariables.Settings.loadCustom_rwyCFG = bool.Parse(node.GetValue("loadCustom_rwyCFG"));
                }
                catch (Exception)
                {
                    Debug.Log("NavUtil: Error loading loadCustom_rwyCFG from config");
                    throw;
                }
                try
                {
                    GlobalVariables.Settings.useBlizzy78ToolBar = bool.Parse(node.GetValue("useBlizzy78ToolBar"));
                }
                catch (Exception)
                {
                    Debug.Log("NavUtil: Error loading useBlizzy78ToolBar from config");
                    throw;
                }



                try
                {
                    GlobalVariables.Settings.hsiPosition.x = float.Parse(node.GetValue("hsiPositionX"));
                }
                catch (Exception)
                {
                    Debug.Log("NavUtil: Error loading hsiPositionX from config");
                    throw;
                }
                try
                {
                    GlobalVariables.Settings.hsiPosition.y = float.Parse(node.GetValue("hsiPositionY"));
                }
                catch (Exception)
                {
                    Debug.Log("NavUtil: Error loading hsiPositionY from config");
                    throw;
                }
                //try
                //{
                //    GlobalVariables.Settings.hsiPosition.width = float.Parse(node.GetValue("hsiPositionWidth"));
                //}
                //catch (Exception)
                //{
                //    Debug.Log("NavUtil: Error loading hsiPositionWidth from config");
                //    throw;
                //}
                //try
                //{
                //    GlobalVariables.Settings.hsiPosition.height = float.Parse(node.GetValue("hsiPositionHeight"));
                //}
                //catch (Exception)
                //{
                //    Debug.Log("NavUtil: Error loading hsiPositionHeight from config");
                //    throw;
                //}


                try
                {
                    GlobalVariables.Settings.rwyEditorGUI.x = float.Parse(node.GetValue("rwyEditorGUIX"));
                }
                catch (Exception)
                {
                    Debug.Log("NavUtil: Error loading rwyEditorGUIX from config");
                    throw;
                }
                try
                {
                    GlobalVariables.Settings.rwyEditorGUI.y = float.Parse(node.GetValue("rwyEditorGUIY"));
                }
                catch (Exception)
                {
                    Debug.Log("NavUtil: Error loading rwyEditorGUIY from config");
                    throw;
                }

                //


                //try
                //{
                //    GlobalVariables.Settings.rwyEditorGUI.width = float.Parse(node.GetValue("rwyEditorGUIWidth"));
                //}
                //catch (Exception)
                //{
                //    Debug.Log("NavUtil: Error loading rwyEditorGUIWidth from config");
                //    throw;
                //}
                //try
                //{
                //    GlobalVariables.Settings.rwyEditorGUI.height = float.Parse(node.GetValue("rwyEditorGUIHeight"));
                //}
                //catch (Exception)
                //{
                //    Debug.Log("NavUtil: Error loading rwyEditorGUIHeight from config");
                //    throw;
                //}




                try
                {
                    GlobalVariables.Settings.settingsGUI.x = float.Parse(node.GetValue("settingsGUIX"));
                }
                catch (Exception)
                {
                    Debug.Log("NavUtil: Error loading settingsGUIX from config");
                    throw;
                }
                try
                {
                    GlobalVariables.Settings.settingsGUI.y = float.Parse(node.GetValue("settingsGUIY"));
                }
                catch (Exception)
                {
                    Debug.Log("NavUtil: Error loading settingsGUIY from config");
                    throw;
                }
                //try
                //{
                //    GlobalVariables.Settings.settingsGUI.width = float.Parse(node.GetValue("settingsGUIWidth"));
                //}
                //catch (Exception)
                //{
                //    Debug.Log("NavUtil: Error loading settingsGUIWidth from config");
                //    throw;
                //}
                //try
                //{
                //    GlobalVariables.Settings.settingsGUI.height = float.Parse(node.GetValue("settingsGUIHeight"));
                //}
                //catch (Exception)
                //{
                //    Debug.Log("NavUtil: Error loading settingsGUIHeight from config");
                //    throw;
                //}
            }
        }

        public static void SaveSettings(string sSettingURL)
        {
            Debug.Log("NavUtil: Saving Settings");

            ConfigNode settings = new ConfigNode();

            //settings.name = "NavUtilSettings";



            ConfigNode sN = new ConfigNode();

            sN.name = "NavUtilSettings";


            sN.AddValue("guiScale", GlobalVariables.Settings.hsiGUIscale);
            sN.AddValue("enableFineLoc", GlobalVariables.Settings.enableFineLoc);
            sN.AddValue("enableWindowsInIVA", GlobalVariables.Settings.enableWindowsInIVA);
            sN.AddValue("loadCustom_rwyCFG", GlobalVariables.Settings.loadCustom_rwyCFG);
            sN.AddValue("useBlizzy78ToolBar", GlobalVariables.Settings.useBlizzy78ToolBar);


            sN.AddValue("hsiPositionX", GlobalVariables.Settings.hsiPosition.x);
            sN.AddValue("hsiPositionY", GlobalVariables.Settings.hsiPosition.y);
            //sN.AddValue("hsiPositionWidth", GlobalVariables.Settings.hsiPosition.width);
            //sN.AddValue("hsiPositionHeight", GlobalVariables.Settings.hsiPosition.height);
            sN.AddValue("rwyEditorGUIX", GlobalVariables.Settings.rwyEditorGUI.x);
            sN.AddValue("rwyEditorGUIY", GlobalVariables.Settings.rwyEditorGUI.y);
            //sN.AddValue("rwyEditorGUIWidth", GlobalVariables.Settings.rwyEditorGUI.width);
            //sN.AddValue("rwyEditorGUIHeight", GlobalVariables.Settings.rwyEditorGUI.height);
            sN.AddValue("settingsGUIX", GlobalVariables.Settings.settingsGUI.x);
            sN.AddValue("settingsGUIY", GlobalVariables.Settings.settingsGUI.y);
            //sN.AddValue("settingsGUIWidth", GlobalVariables.Settings.settingsGUI.width);
            //sN.AddValue("settingsGUIHeight", GlobalVariables.Settings.settingsGUI.height);

            settings.AddNode(sN);

            settings.Save(KSPUtil.ApplicationRootPath + sSettingURL, "NavUtil Setting File");


            //ConfigNode settings = ConfigNode.Load();

        }
    }
}

