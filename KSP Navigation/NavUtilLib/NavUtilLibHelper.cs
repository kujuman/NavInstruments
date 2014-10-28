//NavUtilities by kujuman, © 2014. All Rights Reserved.

using System;
using UnityEngine;
using KSP;
using NavUtilLib;
using var = NavUtilLib.GlobalVariables;


namespace NavUtilLib
{
    [KSPAddon(KSPAddon.Startup.Flight, false)] //used to start up in flight, and be false
    public class NavUtilLibApp : MonoBehaviour
    {
        //this class is to help load textures via GameDatabase since we cannot use static classes

        NavUtilLibApp app;

        ApplicationLauncherButton appButton;

        public bool isHovering = false;

        //RUIPanelTabGroup pTG;

        //public GameObject anchor;

        //public GameObject cascadeBody;
        //public GameObject cascadeFooter;
        //public GameObject cascadeHeader;

        //public RUICascadingList cascadingList;

        //public UIButton hoverComponent;

        //public int maxHeight = 200;
        //public int minHeight = 100;

        //public UIInteractivePanel panel;

        //private System.Collections.Generic.List<bool> bList;





        private Rect windowPosition;
        private RenderTexture rt;

        private bool rwyHover = false;
        private bool gsHover = false;
        private bool closeHover = false;

        public void displayHSI()
        {
            if (!NavUtilLib.GlobalVariables.Settings.hsiState)
            {
                Activate(true);

                NavUtilLib.GlobalVariables.Settings.hsiState = true;
            }
            else
            {
                Activate(false);

                NavUtilLib.GlobalVariables.Settings.hsiState = false;
            }
        }

        public void Activate(bool state)
        {
            if (state)
            {
                RenderingManager.AddToPostDrawQueue(3, OnDraw);

                rt = new RenderTexture(640, 640, 24, RenderTextureFormat.ARGB32);

                if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: Starting systems...");
                if (!var.Settings.navAidsIsLoaded)
                    var.Settings.loadNavAids();

                if (!var.Materials.isLoaded)
                    var.Materials.loadMaterials();

                //if (!var.Audio.isLoaded)
                var.Audio.initializeAudio();

                //load settings to config
                ConfigLoader.LoadSettings(var.Settings.settingsFileURL);

                //ConfigureCamera();

                windowPosition.x = NavUtilLib.GlobalVariables.Settings.hsiPosition.x;
                windowPosition.y = NavUtilLib.GlobalVariables.Settings.hsiPosition.y;


                if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: Systems started successfully!");



            }
            else
            {
                state = false;
                RenderingManager.RemoveFromPostDrawQueue(3, OnDraw); //close the GUI
                NavUtilLib.GlobalVariables.Settings.hsiPosition.x = windowPosition.x;
                NavUtilLib.GlobalVariables.Settings.hsiPosition.y = windowPosition.y;

                ConfigLoader.SaveSettings(var.Settings.settingsFileURL);
            }
        }

        private void OnDraw()
        {
            //Debug.Log("HSI: OnDraw()");
            if (CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.Flight || ((CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.Internal || CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.IVA) && GlobalVariables.Settings.enableWindowsInIVA))
            {
                if ((windowPosition.xMin + windowPosition.width) < 20) windowPosition.xMin = 20 - windowPosition.width;
                if (windowPosition.yMin + windowPosition.height < 20) windowPosition.yMin = 20 - windowPosition.height;
                if (windowPosition.xMin > Screen.width - 20) windowPosition.xMin = Screen.width - 20;
                if (windowPosition.yMin > Screen.height - 20) windowPosition.yMin = Screen.height - 20;

                windowPosition = new Rect(windowPosition.x,
             windowPosition.y,
             (int)(NavUtilLib.GlobalVariables.Settings.hsiPosition.width * NavUtilLib.GlobalVariables.Settings.hsiGUIscale),
             (int)(NavUtilLib.GlobalVariables.Settings.hsiPosition.height * NavUtilLib.GlobalVariables.Settings.hsiGUIscale));

                windowPosition = GUI.Window(1, windowPosition, OnWindow, "Horizontal Situation Indicator");

            }
            //Debug.Log(windowPosition.ToString());
        }

        private void DrawGauge(RenderTexture screen)
        {
            NavUtilLib.GlobalVariables.FlightData.updateNavigationData();

            RenderTexture pt = RenderTexture.active;
            RenderTexture.active = screen;

            if (!screen.IsCreated()) screen.Create();

            NavUtilLib.DisplayData.DrawHSI(screen, 1);

            //write text to screen
            //write runway info


            if (rwyHover)
                NavUtilLib.TextWriter.addTextToRT(screen, "→Runway: " + NavUtilLib.GlobalVariables.FlightData.selectedRwy.ident, new Vector2(20, screen.height - 40), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            else
                NavUtilLib.TextWriter.addTextToRT(screen, " Runway: " + NavUtilLib.GlobalVariables.FlightData.selectedRwy.ident, new Vector2(20, screen.height - 40), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);

            if (gsHover)
                NavUtilLib.TextWriter.addTextToRT(screen, "→Glideslope: " + string.Format("{0:F1}", NavUtilLib.GlobalVariables.FlightData.selectedGlideSlope) + "°  Elevation: " + string.Format("{0:F0}", NavUtilLib.GlobalVariables.FlightData.selectedRwy.altMSL) + "m", new Vector2(20, screen.height - 64), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            else
                NavUtilLib.TextWriter.addTextToRT(screen, " Glideslope: " + string.Format("{0:F1}", NavUtilLib.GlobalVariables.FlightData.selectedGlideSlope) + "°  Elevation: " + string.Format("{0:F0}", NavUtilLib.GlobalVariables.FlightData.selectedRwy.altMSL) + "m", new Vector2(20, screen.height - 64), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);



            NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.Utils.makeAngle0to360(FlightGlobals.ship_heading), true).ToString(), new Vector2(584, screen.height - 102), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.Utils.makeAngle0to360(NavUtilLib.GlobalVariables.FlightData.bearing), true).ToString(), new Vector2(584, screen.height - 131), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.Utils.makeAngle0to360(NavUtilLib.GlobalVariables.FlightData.selectedRwy.hdg), true).ToString(), new Vector2(35, screen.height - 124), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
            NavUtilLib.TextWriter.addTextToRT(screen, NavUtilLib.Utils.numberFormatter((float)NavUtilLib.GlobalVariables.FlightData.dme / 1000, false).ToString(), new Vector2(45, screen.height - 563), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);

            if (closeHover)
                NavUtilLib.TextWriter.addTextToRT(screen, "    Close HSI", new Vector2(340, 15), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);

            RenderTexture.active = pt;
        }

        private void OnWindow(int WindowID)
        {
            //Debug.Log("HSI: OnWindow()");



            Rect rwyBtn = new Rect(20 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale,
                13 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale,
                200 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale,
                20 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale);

            Rect gsBtn = new Rect(20 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale,
        38 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale,
        200 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale,
        20 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale);

            Rect closeBtn = new Rect(330 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale,
                580 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale,
                300 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale,
                50 * NavUtilLib.GlobalVariables.Settings.hsiGUIscale);

            if (GUI.Button(closeBtn, new GUIContent("CloseBtn", "closeOn")))
            {
                //displayHSI();
                //Debug.Log("CloseHSI");
                appButton.SetFalse();
                //goto CloseWindow;
            }

            if (GUI.tooltip == "closeOn")
                closeHover = true;
            else
                closeHover = false;


            if (GUI.Button(rwyBtn, new GUIContent("Next Runway", "rwyOn")))
            {
                if (Event.current.button == 0)
                {
                    NavUtilLib.GlobalVariables.FlightData.rwyIdx++;
                }
                else
                {
                    NavUtilLib.GlobalVariables.FlightData.rwyIdx--;
                }

                NavUtilLib.GlobalVariables.FlightData.rwyIdx = NavUtilLib.Utils.indexChecker(NavUtilLib.GlobalVariables.FlightData.rwyIdx, NavUtilLib.GlobalVariables.FlightData.rwyList.Count - 1, 0);
            }

            if (GUI.tooltip == "rwyOn")
                rwyHover = true;
            else
                rwyHover = false;


            if (GUI.Button(gsBtn, new GUIContent("Next G/S", "gsOn")))
            {
                if (Event.current.button == 0)
                {
                    NavUtilLib.GlobalVariables.FlightData.gsIdx++;
                }
                else
                {
                    NavUtilLib.GlobalVariables.FlightData.gsIdx--;
                }

                NavUtilLib.GlobalVariables.FlightData.gsIdx = NavUtilLib.Utils.indexChecker(NavUtilLib.GlobalVariables.FlightData.gsIdx, NavUtilLib.GlobalVariables.FlightData.gsList.Count - 1, 0);
            }

            if (GUI.tooltip == "gsOn")
                gsHover = true;
            else
                gsHover = false;

            DrawGauge(rt);
            GUI.DrawTexture(new Rect(0, 0, windowPosition.width, windowPosition.height), rt, ScaleMode.ScaleToFit);

            GUI.DragWindow();
        }







        void Awake()
        {
            //load settings to config
            ConfigLoader.LoadSettings(var.Settings.settingsFileURL);

            if(NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: useBlizzy? " + NavUtilLib.GlobalVariables.Settings.useBlizzy78ToolBar);

            if (!NavUtilLib.GlobalVariables.Settings.useBlizzy78ToolBar)
            {


                GameEvents.onGUIApplicationLauncherReady.Add(OnGUIReady);

                GameEvents.onGameSceneLoadRequested.Add(dEstroy);
            }

            NavUtilLib.GlobalVariables.Settings.appInstance = this.GetInstanceID();


        }




        public void dEstroy(GameScenes g)
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIReady);

            if (appButton != null)
            {
                //save settings to config
                ConfigLoader.SaveSettings(var.Settings.settingsFileURL);

                NavUtilLib.GlobalVariables.Settings.hsiState = false;

                ApplicationLauncher.Instance.RemoveModApplication(appButton);
            }
        }



        void OnGUIReady()
        {
            if (ApplicationLauncher.Ready && !NavUtilLib.GlobalVariables.Settings.useBlizzy78ToolBar)
            {
                appButton = ApplicationLauncher.Instance.AddModApplication(
                    onAppLaunchToggleOn,
                    onAppLaunchToggleOff,
                    onAppLaunchHoverOn,
                    onAppLaunchHoverOff,
                    onAppLaunchEnable,
                    onAppLaunchDisable,
                    ApplicationLauncher.AppScenes.FLIGHT,
                    (Texture)GameDatabase.Instance.GetTexture("KerbalScienceFoundation/NavInstruments/CommonTextures/toolbarButton3838", false)
                  );
                ;
            }

            app = this;

            //panel = new UIInteractivePanel();
            //panel.draggable = true;
            //panel.index = 1;



        }

        void onAppLaunchToggleOn()
        {
            if(isHovering)
            {
                if (Event.current.button == 1)
                {
                    NavUtilLib.SettingsGUI.startSettingsGUI();
                    goto Finish;
                }
            }
            displayHSI();

        Finish:
            ;
            ////Debug.Log("onAppLaunchToggleOn");

            //Debug.Log(appButton.GetAnchor().ToString());
            ////Debug.Log("State: " + appButton.State);
            ////Debug.Log(appButton.transform.ToString());
            //Debug.Log(appButton.transform.position.ToString());
            //Debug.Log();



        }




        void onAppLaunchToggleOff()
        {
            if (isHovering)
            {
                if (Event.current.button == 1)
                {
                    NavUtilLib.SettingsGUI.startSettingsGUI();
                    goto Finish;
                }
            }
            displayHSI();

        Finish:
            ;
            //bug.Log("onAppLaunchToggleOff");
            ;
        }
        void onAppLaunchHoverOn()
        {
            //bList.Clear();

            //foreach(ApplicationLauncherButton b in ApplicationLauncher.FindObjectsOfType<ApplicationLauncherButton>())
            //{
            //    if (b.State == RUIToggleButton.ButtonState.TRUE)
            //        bList.Add(true);
            //    else
            //        bList.Add(false);

            //    if (b != appButton)
            //    {
            //        b.SetFalse();
            //    }
            //}

            //appButton.SetTrue();

            //foreach(Staging s in GameObject.FindObjectsOfType<Staging>()) 
            //{
            //    Debug.Log("staging " + s.stageSpacing);

            //    s.stageSpacing++;
            //}

            //Debug.Log("onHover!");

            isHovering = true;
            //var a = ApplicationLauncher.Instance.anchor.transform.position;
            //Debug.Log(a);

            //Debug.Log("anchor " + appButton.GetAnchor().ToString());
        }
        void onAppLaunchHoverOff()
        {
            isHovering = false;

            //int cB = 0;
            //foreach (ApplicationLauncherButton b in ApplicationLauncher.FindObjectsOfType<ApplicationLauncherButton>())
            //{
            //    if (bList[cB])
            //    {
            //        b.SetTrue();
            //    }
            //    else
            //        b.SetFalse();

            //    if (b == appButton)
            //    {
            //        b.SetFalse();
            //    }
            //}
            //bList.Clear();
            ;
        }
        void onAppLaunchEnable()
        {
            ;
        }
        void onAppLaunchDisable()
        {
            ;
        }

        bool isApplicationTrue()
        {
            return false;
        }

    }
}
