using System;
using KSP;
using UnityEngine;

namespace NavUtilities
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
                Runway rwy = new Runway();

                rwy.ident = node.GetValue("ident");
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
            }

            return runwayList;
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

        public static float GetGUIScale(string sSettingURL)
        {
            ConfigNode settings = ConfigNode.Load(KSPUtil.ApplicationRootPath + sSettingURL);
            float scale = 1;

            foreach (ConfigNode node in settings.GetNodes("Main_Setting"))
            {
                scale = float.Parse(node.GetValue("guiScale"));
            }
            return scale;
        }
    }
}
