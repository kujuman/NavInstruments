using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using KSP;

namespace NavUtilLib
{
    namespace GlobalVariables
    {
        public static class Settings
        {
            public static string settingsFileURL = "GameData/KerbalScienceFoundation/NavInstruments/settings.cfg";
            public static bool navAidsIsLoaded = false;

            public static void loadNavAids()
            {
                Debug.Log("NavUtilLib: Loading NavAid database...");
                FlightData.rwyList.Clear();
                Debug.Log("_+");
                FlightData.rwyList = ConfigLoader.GetRunwayListFromConfig(settingsFileURL);
                Debug.Log("1");
                FlightData.gsList.Clear();
                FlightData.gsList = ConfigLoader.GetGlideslopeListFromConfig(settingsFileURL);

                Debug.Log("2");

                DirectoryInfo folder = new DirectoryInfo(KSPUtil.ApplicationRootPath + "GameData/KerbalScienceFoundation/NavInstruments/Runways");

                Debug.Log("3");

                if (folder.Exists)
                {
                    Debug.Log("4");

                    FileInfo[] addlNavAidFiles = folder.GetFiles("*_rwy.cfg");

                    Debug.Log("5");

                    foreach (FileInfo finfo in addlNavAidFiles)
                    {
                        Debug.Log("A");

                        FlightData.rwyList.AddRange(NavUtilLib.ConfigLoader.GetRunwayListFromConfig("GameData/KerbalScienceFoundation/NavInstruments/Runways/" + finfo.Name));
                        Debug.Log("B");
                        FlightData.gsList.AddRange(NavUtilLib.ConfigLoader.GetGlideslopeListFromConfig("GameData/KerbalScienceFoundation/NavInstruments/Runways/" + finfo.Name));
                        Debug.Log("C");
                    }
                    
                }
                Debug.Log("6");
                navAidsIsLoaded = true;
            }
        }

        public static class FlightData
        {
            public static List<Runway> rwyList = new List<Runway>();
            public static int rwyIdx;

            public static List<float> gsList = new List<float>();
            public static int gsIdx;

            public static Runway selectedRwy;
            public static float selectedGlideSlope;
            public static Vessel currentVessel;

            /// <summary>
            /// /////////
            /// </summary>
            private static double lastNavUpdateUT;
            public static void SetLastNavUpdateUT()
            {
                lastNavUpdateUT = Planetarium.GetUniversalTime();
            }
            public static double GetLastNavUpdateUT()
            {
                return lastNavUpdateUT;
            }

            public static double bearing;
            public static double dme;
            public static double elevationAngle;
            public static float locDeviation;
            public static float gsDeviation;

            public static void updateNavigationData()
            {
                selectedGlideSlope = gsList[gsIdx];
                selectedRwy = rwyList[rwyIdx];
                currentVessel = FlightGlobals.ActiveVessel;

                bearing = NavUtilLib.Utils.CalcBearingToBeacon(currentVessel, selectedRwy);
                dme = NavUtilLib.Utils.CalcDistanceToBeacon(currentVessel, selectedRwy);
                elevationAngle = NavUtilLib.Utils.CalcElevationAngle(currentVessel, selectedRwy);
                locDeviation = NavUtilLib.Utils.CalcLocalizerDeviation(bearing, selectedRwy);
                gsDeviation = NavUtilLib.Utils.CalcGlideslopeDeviation(elevationAngle, selectedGlideSlope);

                SetLastNavUpdateUT();
            }
        }


        public class Materials
        {
            private static Materials instance;
            private Materials() { }

            public static Materials Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new Materials();
                    }
                    return instance;
                }
            }

            public static bool isLoaded = false;
            public Material overlay = null;
            public Material pointer = null;
            public Material headingCard = null;
            public Material NDBneedle = null;
            public Material course = null;
            public Material localizer = null;
            public Material mkrbcn = null;
            public Material flag = null;
            public Material back = null;

            public static void loadMaterials()
            {
                Debug.Log("NavUtilLib: Updating materials...");

                Debug.Log("loadMaterials()");
                string texName;

                texName = "hsi_overlay.png";
                Materials.Instance.overlay = Graphics.loadMaterial(texName, Materials.Instance.overlay, 640, 640);

                texName = "hsi_gs_pointer.png";
                Materials.Instance.pointer = Graphics.loadMaterial(texName, Materials.Instance.pointer, 640, 24);

                texName = "hsi_large_heading_card.png";
                Materials.Instance.headingCard = Graphics.loadMaterial(texName, Materials.Instance.headingCard, 501, 501);

                texName = "hsi_NDB_needle.png";
                Materials.Instance.NDBneedle = Graphics.loadMaterial(texName, Materials.Instance.NDBneedle, 15, 501);

                texName = "hsi_course_needle.png";
                Materials.Instance.course = Graphics.loadMaterial(texName, Materials.Instance.course, 221, 481);

                texName = "hsi_course_deviation_needle.png";
                Materials.Instance.localizer = Graphics.loadMaterial(texName, Materials.Instance.localizer, 5, 251);

                texName = "hsi_markerIndicator.png";
                Materials.Instance.mkrbcn = Graphics.loadMaterial(texName, Materials.Instance.mkrbcn, 175, 180);

                texName = "hsi_flags.png";
                Materials.Instance.flag = Graphics.loadMaterial(texName, Materials.Instance.flag, 64, 64);

                texName = "hsi_back.png";
                Materials.Instance.back = Graphics.loadMaterial(texName, Materials.Instance.back, 32, 32);

                isLoaded = true;
            }
        }

        public static class Audio
        {
            public static bool isLoaded = false;

            public static GameObject audioplayer = new GameObject();
            public static AudioSource markerAudio = new AudioSource();

            public static void initializeAudio()
            {
                Debug.Log("NavUtilLib: InitializingAudio...");
                markerAudio = audioplayer.AddComponent<AudioSource>();
                markerAudio.volume = GameSettings.VOICE_VOLUME;
                markerAudio.pan = 0;
                markerAudio.dopplerLevel = 0;
                markerAudio.bypassEffects = true;
                markerAudio.loop = true;

                isLoaded = true;
            }
        }
    }
}