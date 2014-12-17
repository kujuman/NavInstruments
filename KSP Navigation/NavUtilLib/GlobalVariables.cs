//NavUtilities by kujuman, © 2014. All Rights Reserved.

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
            //public static string gsFileURL = "GameData/KerbalScienceFoundation/NavInstruments/glideslopes.cfg";


            public static Rect hsiPosition = new Rect(50,50,640,640);
            public static float hsiGUIscale = 0.5f;
            public static bool hsiState = false;

            public static Rect settingsGUI = new Rect(100,50,250,180);

            public static Rect rwyEditorGUI = new Rect(50, 50, 450, 300);
            public static NavUtilGUI.RunwaysEditor rE = new NavUtilGUI.RunwaysEditor();
            public static bool rwyEditorState = false;

            public static bool navAidsIsLoaded = false;

            public static bool enableFineLoc = true;

            public static bool loadCustom_rwyCFG = true;
            public static bool useBlizzy78ToolBar = false;

            public static bool enableWindowsInIVA = true;

            public static int appInstance;

            public static bool enableDebugging = false;

            public static void loadNavAids()
            {
                if (enableDebugging) Debug.Log("NavUtil: Loading NavAid database...");
                FlightData.rwyList.Clear();
                FlightData.rwyList = ConfigLoader.GetRunwayListFromConfig("GameData/KerbalScienceFoundation/NavInstruments/defaultRunways.cfg");
                FlightData.gsList.Clear();
                FlightData.gsList = ConfigLoader.GetGlideslopeListFromConfig("GameData/KerbalScienceFoundation/NavInstruments/glideslopes.cfg");

                FlightData.customRunways.Clear();

                DirectoryInfo folder = new DirectoryInfo(KSPUtil.ApplicationRootPath + "GameData/KerbalScienceFoundation/NavInstruments/Runways");

                if (folder.Exists)
                {
                    //FileInfo[] addlNavAidFiles = folder.GetFiles("*.rwy"); //this works great :D
                    FileInfo[] addlNavAidFiles = folder.GetFiles("*");


                    foreach (FileInfo f in addlNavAidFiles)
                    {
                        if ((f.Name.EndsWith("_rwy.cfg") && loadCustom_rwyCFG)|| f.Name.EndsWith(".rwy"))
                        {

                            if (enableDebugging)  Debug.Log("NavUtil: found file " + f.Name.ToString());

                            if (f.Name == "custom.rwy" || (f.Name == "custom_rwy.cfg" && GlobalVariables.Settings.loadCustom_rwyCFG))
                            {
                                FlightData.customRunways.AddRange(NavUtilLib.ConfigLoader.GetRunwayListFromConfig("GameData/KerbalScienceFoundation/NavInstruments/Runways/" + f.Name));
                                if (enableDebugging)  Debug.Log("NavUtil: Found " + f.Name + " with " + FlightData.customRunways.Count + " runway definitions");

                            }

                            if (enableDebugging) Debug.Log("NavUtil: Found " + f.Name);

                            FlightData.rwyList.AddRange(NavUtilLib.ConfigLoader.GetRunwayListFromConfig("GameData/KerbalScienceFoundation/NavInstruments/Runways/" + f.Name));
                            FlightData.gsList.AddRange(NavUtilLib.ConfigLoader.GetGlideslopeListFromConfig("GameData/KerbalScienceFoundation/NavInstruments/Runways/" + f.Name));
                            //}
                        }
                    }
                }

                navAidsIsLoaded = true;
            }
        }

        public static class FlightData
        {
            public static List<Runway> rwyList = new List<Runway>();
            public static int rwyIdx;

            public static List<float> gsList = new List<float>();
            public static int gsIdx;

            public static List<Runway> customRunways = new List<Runway>();
            public static int cRwyIdx;

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
            public static float runwayHeading;

            public static void updateNavigationData()
            {
                selectedGlideSlope = gsList[gsIdx];
                selectedRwy = rwyList[rwyIdx];
                currentVessel = FlightGlobals.ActiveVessel;

                bearing = NavUtilLib.Utils.CalcBearingToBeacon(currentVessel, selectedRwy);
                dme = NavUtilLib.Utils.CalcDistanceToBeacon(currentVessel, selectedRwy);
                elevationAngle = NavUtilLib.Utils.CalcElevationAngle(currentVessel, selectedRwy);
                //locDeviation = NavUtilLib.Utils.CalcLocalizerDeviation(bearing, selectedRwy);
                locDeviation = (float)NavUtilLib.Utils.CalcLocalizerDeviation(currentVessel, selectedRwy);
                gsDeviation = NavUtilLib.Utils.CalcGlideslopeDeviation(elevationAngle, selectedGlideSlope);

                //
                runwayHeading = (float)NavUtilLib.Utils.CalcProjectedRunwayHeading(currentVessel, selectedRwy);

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
            public Material whiteFont = null;

            public static void loadMaterials()
            {
                if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtilLib: Updating materials...");
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

                texName = "white_font.png";
                Materials.Instance.whiteFont = Graphics.loadMaterial(texName, Materials.Instance.whiteFont, 256, 256);
                 
                isLoaded = true;
            }
        }

        public class Audio
        {
            private static Audio instance;
            private Audio() { }

            public static Audio Instance
            {
                get
                {
                    if(instance == null)
                    {
                        instance = new Audio();
                    }
                    return instance;
                }
            }

            public static bool isLoaded = false;
            
            public static GameObject audioplayer; 
            public static AudioSource markerAudio;
            //public static AudioSource playOnce;

            public static void initializeAudio()
            {
                audioplayer = new GameObject();
                markerAudio = new AudioSource();
                //playOnce = new AudioSource();

                if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtilLib: InitializingAudio...");

                try
                {
                markerAudio = audioplayer.AddComponent<AudioSource>();
                markerAudio.volume = GameSettings.VOICE_VOLUME;
                markerAudio.pan = 0;
                markerAudio.dopplerLevel = 0;
                markerAudio.bypassEffects = true;
                markerAudio.loop = true;
                markerAudio.rolloffMode = AudioRolloffMode.Linear;
                markerAudio.transform.SetParent(FlightCamera.fetch.mainCamera.transform);

                //playOnce = audioplayer.AddComponent<AudioSource>();
                //playOnce.volume = GameSettings.VOICE_VOLUME;
                //playOnce.pan = 0;
                //playOnce.dopplerLevel = 0;
                //playOnce.bypassEffects = true;
                //playOnce.loop = false;
                //playOnce.rolloffMode = AudioRolloffMode.Linear;
                //playOnce.transform.SetParent(FlightCamera.fetch.mainCamera.transform);

                
                }
                catch (Exception)
                {
                    if (NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: Error Loading Audio");

                    throw;
                }


                isLoaded = true;
            }
        }
    }
}