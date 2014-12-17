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

            foreach (NavUtilLibApp a in appInstaces)
            {
                Debug.Log("NavUtil: Match? " + NavUtilLib.GlobalVariables.Settings.appInstance + " " + a.GetInstanceID());

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


    private void destroyPopupMenu(IButton button)
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
