using System;
using KSP;
using UnityEngine;

namespace KSF_NavInstruments
{
    class ConfigLoader
    {
        public static System.Collections.Generic.List<KSF_RunwayInformation> GetRunwayListFromConfig()
        {
            System.Collections.Generic.List<KSF_RunwayInformation> runwayList = new System.Collections.Generic.List<KSF_RunwayInformation>();
            runwayList.Clear();

            ConfigNode runways = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/KerbalScienceFoundation/NavInstruments/settings.cfg");
            foreach (ConfigNode node in runways.GetNodes("Runway"))
            {
                KSF_RunwayInformation rwy = new KSF_RunwayInformation();
                
                rwy.ident = node.GetValue("ident");
                rwy.hdg = float.Parse(node.GetValue("hdg"));
                //rwy.identOfOpposite = node.GetValue("identOfOpposite");
                rwy.body = node.GetValue("body");
                rwy.altMSL = float.Parse(node.GetValue("altMSL"));
                rwy.gsLatitude = float.Parse(node.GetValue("gsLatitude"));
                rwy.gsLongitude = float.Parse(node.GetValue("gsLongitude"));
                rwy.locLatitude = float.Parse(node.GetValue("locLatitude"));
                rwy.locLongitude = float.Parse(node.GetValue("locLongitude"));

                runwayList.Add(rwy);
            }

            return runwayList;
        }

        public static float GetGUIScale()
        {
            ConfigNode settings = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/KerbalScienceFoundation/NavInstruments/settings.cfg");
            float scale = 1;


            foreach (ConfigNode node in settings.GetNodes("Main_Setting"))
            {
                scale = float.Parse(node.GetValue("guiScale"));
            }
            return scale;
        }
    }
}
