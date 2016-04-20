//using System;
//using UnityEngine;
////using Toolbar;
//using NavUtilLib;
//using System.Linq;
//using var = NavUtilLib.GlobalVariables;


//[KSPAddon(KSPAddon.Startup.EveryScene, false)]
//class btnCreate : MonoBehaviour
//{
//    private IButton b;

//    //public static bool HSIguiState = false;

//    //private Rect windowPosition;
//    //private RenderTexture rt;

//    //private bool rwyHover = false;
//    //private bool gsHover = false;
//    //private bool closeHover = false;

//    private NavUtilLibApp app;

//    internal btnCreate()
//    {
//        Debug.Log("NavUtil: Try to start ToolBar...");

//        NavUtilLib.ConfigLoader.LoadSettings(NavUtilLib.GlobalVariables.Settings.settingsFileURL);

//        if (NavUtilLib.GlobalVariables.Settings.useBlizzy78ToolBar)
//        {

//        Debug.Log(NavUtilLib.GlobalVariables.Settings.useBlizzy78ToolBar);
//            Debug.Log("NavUtil: Starting ToolBar");

//            b = ToolbarManager.Instance.add("NavUtil", "NavUtilBtn");
//            b.TexturePath = "KerbalScienceFoundation/NavInstruments/CommonTextures/toolbarButton";
//            b.ToolTip = "View Navigation Utilites";
//            b.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
//            b.OnClick += (e) => toggleToolbarButton(e, b);

//            Debug.Log("NavUtil: Started ToolBar");
//            //app = FindObjectOfType<NavUtilLibApp>();
//        }
//    }


//    private void toggleToolbarButton(ClickEvent e, IButton button)
//    {
//        switch (e.MouseButton)
//        {
//            case 0:     // left click
//                tryActivate();
//                break;
//            case 1:     // right click
//                NavUtilLib.SettingsGUI.startSettingsGUI();
//                break;
//            default:
//                break;
//        }
//    }


//    private void tryActivate()
//    {
//        Debug.Log("NavUtil: tryActivate");

//        if (app == null)
//        {
//            Debug.Log("NavUtil: app is null!");

//            NavUtilLibApp[] appInstaces;
//            appInstaces = FindObjectsOfType<NavUtilLibApp>();

//            Debug.Log("NavUtil: Found " + appInstaces.Count() + " app instances");

//            foreach (NavUtilLibApp a in appInstaces)
//            {
//                Debug.Log("NavUtil: Match? " + NavUtilLib.GlobalVariables.Settings.appInstance + " " + a.GetInstanceID());

//                if (NavUtilLib.GlobalVariables.Settings.appInstance == a.GetInstanceID())
//                {
//                    app = a;
//                    goto Finish;
//                }


//            }

//            //create a new app instance
//            Debug.Log("NavUtil: adding reference from GlobalVariables...");
//            app = NavUtilLib.GlobalVariables.Settings.appReference;


//        Finish:
//            ;
//        }
//        Debug.Log("NavUtil: Try to start app...");

//            app.displayHSI();
//    }


//    internal void OnDestroy()
//    {
//        b.Destroy();

//        NavUtilLib.GlobalVariables.Settings.hsiState = false; //thanks to Virindi
//    }
//}
