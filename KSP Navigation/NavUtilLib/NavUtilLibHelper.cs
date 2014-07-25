//NavUtilities by kujuman, © 2014. All Rights Reserved.

using System;
using UnityEngine;
using KSP;

namespace NavUtilLib
{
    //[KSPAddon(KSPAddon.Startup.Flight, false)]
    public class NavUtilLibApp : MonoBehaviour
    {
        //this class is to help load textures via GameDatabase since we cannot use static classes

        //NavUtilLibApp app;

        //ApplicationLauncherButton appButton;

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


        //void Awake()
        //{
        //    GameEvents.onGUIApplicationLauncherReady.Add(OnGUIReady);

        //    GameEvents.onGameSceneLoadRequested.Add(dEstroy);


        //}




        //public void dEstroy(GameScenes g)
        //{
        //    GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIReady);

        //    if (appButton != null)
        //    {
        //        ApplicationLauncher.Instance.RemoveModApplication(appButton);
        //    }
        //}



        //void OnGUIReady()
        //{
        //    if (ApplicationLauncher.Ready)
        //    {
        //        appButton = ApplicationLauncher.Instance.AddModApplication(
        //            onAppLaunchToggleOn,
        //            onAppLaunchToggleOff,
        //            onAppLaunchHoverOn,
        //            onAppLaunchHoverOff,
        //            onAppLaunchEnable,
        //            onAppLaunchDisable,
        //            ApplicationLauncher.AppScenes.FLIGHT,
        //            (Texture)GameDatabase.Instance.GetTexture("KerbalScienceFoundation/NavInstruments/CommonTextures/toolbarButton3838", false)
        //          );
        //        ;
        //    }

        //    app = this;

        //    panel = new UIInteractivePanel();
        //    panel.draggable = true;
        //    panel.index = 1;
            


        //}

        //void onAppLaunchToggleOn()
        //{
        //    ////Debug.Log("onAppLaunchToggleOn");

        //    //Debug.Log(appButton.GetAnchor().ToString());
        //    ////Debug.Log("State: " + appButton.State);
        //    ////Debug.Log(appButton.transform.ToString());
        //    //Debug.Log(appButton.transform.position.ToString());
        //    //Debug.Log();


        //    ;
        //}


        //void onAppLaunchToggleOff()
        //{
        //    //bug.Log("onAppLaunchToggleOff");
        //    ;
        //}
        //void onAppLaunchHoverOn()
        //{
        //    //bList.Clear();

        //    //foreach(ApplicationLauncherButton b in ApplicationLauncher.FindObjectsOfType<ApplicationLauncherButton>())
        //    //{
        //    //    if (b.State == RUIToggleButton.ButtonState.TRUE)
        //    //        bList.Add(true);
        //    //    else
        //    //        bList.Add(false);

        //    //    if (b != appButton)
        //    //    {
        //    //        b.SetFalse();
        //    //    }
        //    //}

        //    //appButton.SetTrue();

        //    //foreach(Staging s in GameObject.FindObjectsOfType<Staging>()) 
        //    //{
        //    //    Debug.Log("staging " + s.stageSpacing);

        //    //    s.stageSpacing++;
        //    //}




        //    var a = ApplicationLauncher.Instance.anchor.transform.position;
        //    Debug.Log(a);

        //    Debug.Log("anchor " + appButton.GetAnchor().ToString());
        //}
        //void onAppLaunchHoverOff()
        //{
        //    //int cB = 0;
        //    //foreach (ApplicationLauncherButton b in ApplicationLauncher.FindObjectsOfType<ApplicationLauncherButton>())
        //    //{
        //    //    if (bList[cB])
        //    //    {
        //    //        b.SetTrue();
        //    //    }
        //    //    else
        //    //        b.SetFalse();

        //    //    if (b == appButton)
        //    //    {
        //    //        b.SetFalse();
        //    //    }
        //    //}
        //    //bList.Clear();
        //    ;
        //}
        //void onAppLaunchEnable()
        //{
        //    ;
        //}
        //void onAppLaunchDisable()
        //{
        //    ;
        //}

        //bool isApplicationTrue()
        //{
        //    return false;
        //}

    }
}
