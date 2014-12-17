using System;
using UnityEngine;
using Toolbar;
using NavUtilLib;
using System.Linq;
using var = NavUtilLib.GlobalVariables;


[KSPAddon(KSPAddon.Startup.EveryScene, false)]
class btnCreate : MonoBehaviour
{
    private IButton b;

    //public static bool HSIguiState = false;

    //private Rect windowPosition;
    //private RenderTexture rt;

    //private bool rwyHover = false;
    //private bool gsHover = false;
    //private bool closeHover = false;

    private NavUtilLibApp app;

    internal btnCreate()
    {
        Debug.Log("NavUtil: Try to start ToolBar...");

        NavUtilLib.ConfigLoader.LoadSettings(NavUtilLib.GlobalVariables.Settings.settingsFileURL);

        if (NavUtilLib.GlobalVariables.Settings.useBlizzy78ToolBar)
        {

        Debug.Log(NavUtilLib.GlobalVariables.Settings.useBlizzy78ToolBar);
            Debug.Log("NavUtil: Starting ToolBar");

            b = ToolbarManager.Instance.add("NavUtil", "NavUtilBtn");
            b.TexturePath = "KerbalScienceFoundation/NavInstruments/CommonTextures/toolbarButton";
            b.ToolTip = "View Navigation Utilites";
            b.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
            b.OnClick += (e) => togglePopupMenu(b);

            Debug.Log("NavUtil: Started ToolBar");
            //app = FindObjectOfType<NavUtilLibApp>();
        }
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

            option2.OnClick += (e2) => tryActivate();
        }
        else
        {

            IButton option2 = menu.AddOption("Open HSI Window");
            option2.OnClick += (e2) => tryActivate();
        }

        IButton option3 = menu.AddOption("Custom Runways");
        option3.OnClick += (e2) => NavUtilLib.GlobalVariables.Settings.rE.startGUI();

        IButton option4 = menu.AddOption("Save Window Layout");
        option4.OnClick += (e2) => NavUtilLib.ConfigLoader.SaveSettings(NavUtilLib.GlobalVariables.Settings.settingsFileURL);


        menu.OnAnyOptionClicked += () => destroyPopupMenu(button);

        button.Drawable = menu;
    }

    private void tryActivate()
    {
        Debug.Log("NavUtil: tryActivate");

        if (app == null)
        {
            Debug.Log("NavUtil: app is null!");

            NavUtilLibApp[] appInstaces;
            appInstaces = FindObjectsOfType<NavUtilLibApp>();

            Debug.Log("NavUtil: Found " + appInstaces.Count() + " app instances");

<<<<<<< HEAD
            foreach (NavUtilLibApp a in appInstaces)
            {
                Debug.Log("NavUtil: Match? " + NavUtilLib.GlobalVariables.Settings.appInstance + " " + a.GetInstanceID());
=======
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
>>>>>>> pr/1

                if (NavUtilLib.GlobalVariables.Settings.appInstance == a.GetInstanceID())
                {
                    app = a;
                    goto Finish;
                }


            }




        Finish:
            ;
        }
        Debug.Log("NavUtil: Try to start app...");

            app.displayHSI();
    }


<<<<<<< HEAD
    private void destroyPopupMenu(IButton button)
=======
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
>>>>>>> pr/1
    {
        ((PopupMenuDrawable)button.Drawable).Destroy();

        button.Drawable = null;
    }


    internal void OnDestroy()
    {
        b.Destroy();

        NavUtilLib.GlobalVariables.Settings.hsiState = false; //thanks to Virindi
    }
}
