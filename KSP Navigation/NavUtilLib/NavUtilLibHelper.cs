//NavUtilities by kujuman, © 2014. All Rights Reserved.

using System;
using UnityEngine;
using KSP;
using UnityEngine.UI;
using NavUtilLib;
using var = NavUtilLib.GlobalVariables;


namespace NavUtilLib
{
    [KSPAddon(KSPAddon.Startup.Flight, false)] //used to start up in flight, and be false
    public class NavUtilLibApp : MonoBehaviour
    {
        private void OnGUI()
        {
            //if (NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtils: OnGUI()");


            if (NavUtilLib.GlobalVariables.Settings.isKSPGUIActive) // will hide GUI is F2 is pressed
            {
                if (NavUtilLib.GlobalVariables.Settings.hsiState) OnDraw();
                if (NavUtilLib.SettingsGUI.isActive) NavUtilLib.SettingsGUI.OnDraw();
                if (NavUtilLib.GlobalVariables.Settings.rwyEditorState) NavUtilGUI.RunwaysEditor.OnDraw();
            }
        }


        //this class is to help load textures via GameDatabase since we cannot use static classes

        NavUtilLibApp app;

        KSP.UI.Screens.ApplicationLauncherButton appButton;

        public bool isHovering = false;

        //private bool visible = false;

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
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging)
                Debug.Log("NavUtils: NavUtilLibApp.displayHSI()");

            if (!NavUtilLib.GlobalVariables.Settings.hsiState)
            {
                Activate(true);
                NavUtilLib.GlobalVariables.Settings.hsiState = true;
                if (NavUtilLib.GlobalVariables.Settings.enableDebugging)
                    Debug.Log("NavUtils: hsiState = " + NavUtilLib.GlobalVariables.Settings.hsiState);
            }
            else
            {
                Activate(false);

                NavUtilLib.GlobalVariables.Settings.hsiState = false;

                if (NavUtilLib.GlobalVariables.Settings.enableDebugging)
                    Debug.Log("NavUtils: hsiState = " + NavUtilLib.GlobalVariables.Settings.hsiState);
            }
        }



        public void Activate(bool state)
        {
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging)
            {
                Debug.Log("NavUtils: NavUtilLibApp.Activate()");
            }

            if (state)
            {
                rt = new RenderTexture(640, 640, 24, RenderTextureFormat.ARGB32);
                rt.Create();

                if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: Starting systems...");
                if (!var.Settings.navAidsIsLoaded)
                    var.Settings.loadNavAids();

                if (!var.Materials.isLoaded)
                    var.Materials.loadMaterials();

                //if (!var.Audio.isLoaded)
                var.Audio.initializeAudio();

                //load settings to config
                //ConfigLoader.LoadSettings(var.Settings.settingsFileURL);

                //ConfigureCamera();
                windowPosition.x = NavUtilLib.GlobalVariables.Settings.hsiPosition.x;
                windowPosition.y = NavUtilLib.GlobalVariables.Settings.hsiPosition.y;


                if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: Systems started successfully!");
            }
            else
            {
                state = false;
                //RenderingManager.RemoveFromPostDrawQueue(3, OnDraw); //close the GUI
                NavUtilLib.GlobalVariables.Settings.hsiPosition.x = windowPosition.x;
                NavUtilLib.GlobalVariables.Settings.hsiPosition.y = windowPosition.y;

                ConfigLoader.SaveSettings(var.Settings.settingsFileURL);
            }
        }

        private void OnDraw()
        {
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging)
            {
                Debug.Log("NavUtils: NavUtilLibApp.OnDraw()");
            }

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

                windowPosition = GUI.Window(-471466245, windowPosition, OnWindow, "Horizontal Situation Indicator");
        }
            //Debug.Log(windowPosition.ToString());
        }

        private void DrawGauge(RenderTexture screen)
        {
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging)
            {
                Debug.Log("NavUtils: NavUtilLibApp.DrawGauge()");
            }

            NavUtilLib.GlobalVariables.FlightData.updateNavigationData();

            RenderTexture pt = RenderTexture.active;
            RenderTexture.active = screen;

            NavUtilLib.DisplayData.DrawHSI(screen, 1);

            //write text to screen
            //write runway info

            string runwayText = (var.FlightData.isINSMode() ? "INS" : "Runway") + ": " + NavUtilLib.GlobalVariables.FlightData.selectedRwy.ident;
            string glideslopeText = var.FlightData.isINSMode() ? ""	: "Glideslope: " + string.Format("{0:F1}", NavUtilLib.GlobalVariables.FlightData.selectedGlideSlope) + "°  ";
            string elevationText = (var.FlightData.isINSMode() ? "Alt MSL" : "Elevation") + ": " + string.Format("{0:F0}", NavUtilLib.GlobalVariables.FlightData.selectedRwy.altMSL) + "m";
            
            runwayText = (rwyHover ? "→" : " ") + runwayText;
            glideslopeText = (gsHover ? "→" : " ") + glideslopeText;
            
	        NavUtilLib.TextWriter.addTextToRT(screen, runwayText, new Vector2(20, screen.height - 40), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);
	        NavUtilLib.TextWriter.addTextToRT(screen, glideslopeText + elevationText, new Vector2(20, screen.height - 64), NavUtilLib.GlobalVariables.Materials.Instance.whiteFont, .64f);

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
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging)
            {
                Debug.Log("NavUtils: NavUtilLibApp.OnWindow()");
            }

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
                appButton.SetFalse(true);
                //goto CloseWindow;
            }

            if (GUI.tooltip == "closeOn")
                closeHover = true;
            else
                closeHover = false;


            if (GUI.Button(rwyBtn, new GUIContent("Next Runway", "rwyOn")) && !var.FlightData.isINSMode()) //doesn't let runway to be switched in INS mode
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

            rt.Create();

            DrawGauge(rt);
            GUI.DrawTexture(new Rect(0, 0, windowPosition.width, windowPosition.height), rt, ScaleMode.ScaleToFit);

            //GUI.DrawTexture(new Rect(0, 0, windowPosition.width, windowPosition.height), var.Materials.Instance.overlay.mainTexture);

            GUI.DragWindow();
        }



        void AddButton()
        {
            if (KSP.UI.Screens.ApplicationLauncher.Ready && !NavUtilLib.GlobalVariables.Settings.useBlizzy78ToolBar)
            {
                appButton = KSP.UI.Screens.ApplicationLauncher.Instance.AddModApplication(
                    onAppLaunchToggleOn,
                    onAppLaunchToggleOff,
                    onAppLaunchHoverOn,
                    onAppLaunchHoverOff,
                    onAppLaunchEnable,
                    onAppLaunchDisable,
                    KSP.UI.Screens.ApplicationLauncher.AppScenes.FLIGHT,
                    (Texture)GameDatabase.Instance.GetTexture("KerbalScienceFoundation/NavInstruments/CommonTextures/toolbarButton3838", false)
                  );
                ;
                app = this;
            }
        }



        void Awake()
        {
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging)
            {
                Debug.Log("NavUtils: NavUtilLibApp.Awake()");
            }

            //load settings to config
            ConfigLoader.LoadSettings(var.Settings.settingsFileURL);

            if(NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: useBlizzy? " + NavUtilLib.GlobalVariables.Settings.useBlizzy78ToolBar);

            if (!NavUtilLib.GlobalVariables.Settings.useBlizzy78ToolBar)
            {


                //GameEvents.onGUIApplicationLauncherReady.Add(OnGUIReady);


                if (appButton == null)
                GameEvents.onGUIApplicationLauncherReady.Add(AddButton);
                GameEvents.onGUIApplicationLauncherUnreadifying.Add(dEstroy);
                GameEvents.onGameSceneLoadRequested.Add(dEstroy);

                GameEvents.onShowUI.Add(ShowGUI);
                GameEvents.onHideUI.Add(HideGUI);
            }



            NavUtilLib.GlobalVariables.Settings.appInstance = this.GetInstanceID();
            NavUtilLib.GlobalVariables.Settings.appReference = this;


        }

        void ShowGUI()
        {
            NavUtilLib.GlobalVariables.Settings.isKSPGUIActive = true;
        }

        void HideGUI()
        {
            NavUtilLib.GlobalVariables.Settings.isKSPGUIActive = false;
        }




        public void dEstroy(GameScenes g)
        {
            //Debug.Log("NavUtils: Destorying App 1");

            GameEvents.onGUIApplicationLauncherReady.Remove(AddButton);

            if (appButton != null)
            {
                //Debug.Log("NavUtils: Destorying App 2");


                //save settings to config
                ConfigLoader.SaveSettings(var.Settings.settingsFileURL);

                NavUtilLib.GlobalVariables.Settings.hsiState = false;

                KSP.UI.Screens.ApplicationLauncher.Instance.RemoveModApplication(appButton);
            }
        }



        //void OnGUIReady()
        //{
        //    if (NavUtilLib.GlobalVariables.Settings.enableDebugging)
        //    {
        //        Debug.Log("NavUtils: NavUtilLibApp.OnGUIReady()");
        //    }

        //    if (KSP.UI.Screens.ApplicationLauncher.Ready && !NavUtilLib.GlobalVariables.Settings.useBlizzy78ToolBar)
        //    {
        //        appButton = KSP.UI.Screens.ApplicationLauncher.Instance.AddModApplication(
        //            onAppLaunchToggleOn,
        //            onAppLaunchToggleOff,
        //            onAppLaunchHoverOn,
        //            onAppLaunchHoverOff,
        //            onAppLaunchEnable,
        //            onAppLaunchDisable,
        //            KSP.UI.Screens.ApplicationLauncher.AppScenes.FLIGHT,
        //            (Texture)GameDatabase.Instance.GetTexture("KerbalScienceFoundation/NavInstruments/CommonTextures/toolbarButton3838", false)
        //          );
        //        ;
        //    }

        //    app = this;

        //    //panel = new UIInteractivePanel();
        //    //panel.draggable = true;
        //    //panel.index = 1;



        //}

        void onAppLaunchToggleOn()
        {
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtils: onAppLaunchToggleOn");
            if(isHovering)
            {
                if (Event.current.alt)
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


        if (NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtils: onAppLaunchToggleOn End");
        }




        void onAppLaunchToggleOff()
        {
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtils: onAppLaunchToggleOff");
            if (isHovering)
            {
                if (Event.current.alt)
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
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("onHover");

            isHovering = true;
        }
        void onAppLaunchHoverOff()
        {
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("offHover");
            isHovering = false;
        }
        void onAppLaunchEnable()
        {
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtils: onAppLaunchEnable");
        }
        void onAppLaunchDisable()
        {
            if (NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtils: onAppLaunchDisable");
        }

        bool isApplicationTrue()
        {
            return false;
        }

    }
}
