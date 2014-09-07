using System;
using UnityEngine;
using Toolbar;
using NavUtilLib;
using var = NavUtilLib.GlobalVariables;


[KSPAddon(KSPAddon.Startup.EveryScene, false)]
class btnCreate : MonoBehaviour
{
    private IButton b;

    //public static bool HSIguiState = false;

    private Rect windowPosition;
    private RenderTexture rt;

    private bool rwyHover = false;
    private bool gsHover = false;
    private bool closeHover = false;

    internal btnCreate()
    {
        b = ToolbarManager.Instance.add("NavUtil", "NavUtilBtn");
        b.TexturePath = "KerbalScienceFoundation/NavInstruments/CommonTextures/toolbarButton";
        b.ToolTip = "View Navigation Utilites";
        b.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
        b.OnClick += (e) => togglePopupMenu(b);
    }


    private void togglePopupMenu(IButton button)
    {
        if (button.Drawable == null)
        {
            createPopupMenu(button);
        }
        else
        {
            destroyPopupMenu(button);
        }
    }


    private void createPopupMenu(IButton button)
    {
        PopupMenuDrawable menu = new PopupMenuDrawable();

        IButton option1 = menu.AddOption("Nav Utilities Options");
        option1.OnClick += (e2) => NavUtilLib.SettingsGUI.startSettingsGUI();

        if (NavUtilLib.GlobalVariables.Settings.hsiState)
        {
            IButton option2 = menu.AddOption("Close HSI Window");
            option2.OnClick += (e2) => displayHSI();
        }
        else
        {
            IButton option2 = menu.AddOption("Open HSI Window");
            option2.OnClick += (e2) => displayHSI();
        }

        IButton option3 = menu.AddOption("Custom Runways");
        option3.OnClick += (e2) => NavUtilLib.GlobalVariables.Settings.rE.startGUI();

        menu.OnAnyOptionClicked += () => destroyPopupMenu(button);

        button.Drawable = menu;
    }

    private void displayHSI()
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


    private void destroyPopupMenu(IButton button)
    {
        ((PopupMenuDrawable)button.Drawable).Destroy();

        button.Drawable = null;
    }


    internal void OnDestroy()
    {
        b.Destroy();
		NavUtilLib.GlobalVariables.Settings.hsiState = false;
    }

    public void Activate(bool state)
    {
        if (state)
        {
            RenderingManager.AddToPostDrawQueue(3, OnDraw);

            rt = new RenderTexture(640, 640, 24, RenderTextureFormat.ARGB32);

            Debug.Log("ILS: Starting systems...");
            if (!var.Settings.navAidsIsLoaded)
                var.Settings.loadNavAids();

            if (!var.Materials.isLoaded)
                var.Materials.loadMaterials();

            //if (!var.Audio.isLoaded)
            var.Audio.initializeAudio();

            //ConfigureCamera();

            Debug.Log("ILS: Systems started successfully!");
        }
        else
        {
            state = false;
            RenderingManager.RemoveFromPostDrawQueue(3, OnDraw); //close the GUI
            NavUtilLib.GlobalVariables.Settings.hsiPosition.x = windowPosition.x;
            NavUtilLib.GlobalVariables.Settings.hsiPosition.y = windowPosition.y;
        }
    }

    private void OnDraw()
    {
        //Debug.Log("HSI: OnDraw()");
        if (CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.Flight)
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

		//Debug.Log ("Bearing: " + NavUtilLib.Utils.makeAngle0to360(NavUtilLib.GlobalVariables.FlightData.bearing).ToString());
		//Debug.Log ("Centerline dist: " + NavUtilLib.GlobalVariables.FlightData.DistanceFromCenterline.ToString());

        if(closeHover)
            NavUtilLib.TextWriter.addTextToRT(screen,"    Close HSI",new Vector2(340,15),NavUtilLib.GlobalVariables.Materials.Instance.whiteFont,.64f);

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

        if (GUI.Button(closeBtn, new GUIContent("Next Runway", "closeOn")))
        {
           
           displayHSI();
        }

        if (GUI.tooltip == "closeOn")
            closeHover = true;
        else
            closeHover = false;


        if (GUI.Button(rwyBtn, new GUIContent ("Next Runway", "rwyOn")))
        {
            if(Event.current.button == 0)
            {
                NavUtilLib.GlobalVariables.FlightData.rwyIdx++;
            }
            else
            {
                NavUtilLib.GlobalVariables.FlightData.rwyIdx--;
            }

            NavUtilLib.GlobalVariables.FlightData.rwyIdx = NavUtilLib.Utils.indexChecker(NavUtilLib.GlobalVariables.FlightData.rwyIdx, NavUtilLib.GlobalVariables.FlightData.rwyList.Count-1, 0);
        }

        if (GUI.tooltip == "rwyOn")
            rwyHover = true;
        else
            rwyHover = false;


        if (GUI.Button(gsBtn, new GUIContent ( "Next G/S", "gsOn")))
        {
            if(Event.current.button == 0)
            {
                NavUtilLib.GlobalVariables.FlightData.gsIdx++;
            }
            else
            {
                NavUtilLib.GlobalVariables.FlightData.gsIdx--;
            }

            NavUtilLib.GlobalVariables.FlightData.gsIdx= NavUtilLib.Utils.indexChecker(NavUtilLib.GlobalVariables.FlightData.gsIdx, NavUtilLib.GlobalVariables.FlightData.gsList.Count-1, 0);
        }


        if (GUI.tooltip == "gsOn")
            gsHover = true;
        else
            gsHover = false;

        DrawGauge(rt);
        GUI.DrawTexture(new Rect(0, 0, windowPosition.width, windowPosition.height), rt, ScaleMode.ScaleToFit);

        



        GUI.DragWindow();
    }


}
