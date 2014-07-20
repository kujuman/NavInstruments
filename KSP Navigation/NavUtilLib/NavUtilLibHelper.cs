using System;
using UnityEngine;
using KSP;

namespace NavUtilLib
{
    //[KSPAddon(KSPAddon.Startup.Flight, false)]
    public class NavUtilLibApp :MonoBehaviour
    {
        //this class is to help load textures via GameDatabase since we cannot use static classes

       //// NavUtilLibApp app;

       //// ApplicationLauncherButton appButton;

       //// RUIPanelTabGroup pTG;

       //// public GameObject anchor;

       //// public GameObject cascadeBody;
       //// public GameObject cascadeFooter;
       //// public GameObject cascadeHeader;

       //// public RUICascadingList cascadingList;

       //// public UIButton hoverComponent;

       //// public int maxHeight;
       //// public int minHeight;


       //// void Awake()
       //// {



       ////     GameEvents.onGUIApplicationLauncherReady.Add(OnGUIReady);

       ////     GameEvents.onGameSceneLoadRequested.Add(dEstroy);


       //// }




       ////public void dEstroy(GameScenes g)
       //// {
       ////     GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIReady);

       ////     if(appButton != null)
       ////     {
       ////         ApplicationLauncher.Instance.RemoveModApplication(appButton);
       ////     }
       //// }



       //// void OnGUIReady()
       //// {
       ////     if (ApplicationLauncher.Ready)
       ////     {
       ////         appButton = ApplicationLauncher.Instance.AddModApplication(
       ////             onAppLaunchToggleOn,
       ////             onAppLaunchToggleOff,
       ////             onAppLaunchHoverOn,
       ////             onAppLaunchHoverOff,
       ////             onAppLaunchEnable,
       ////             onAppLaunchDisable,
       ////             ApplicationLauncher.AppScenes.FLIGHT,
       ////             (Texture) GameDatabase.Instance.GetTexture("KerbalScienceFoundation/NavInstruments/CommonTextures/toolbarButton3838",false)
       ////           );
       ////         ;
       ////     }


       //// }

       //// void onAppLaunchToggleOn()
       //// {
       ////     ////Debug.Log("onAppLaunchToggleOn");

       ////     //Debug.Log(appButton.GetAnchor().ToString());
       ////     ////Debug.Log("State: " + appButton.State);
       ////     ////Debug.Log(appButton.transform.ToString());
       ////     //Debug.Log(appButton.transform.position.ToString());
       ////     //Debug.Log();

            
       ////     ;
       //// }

        
       //// void onAppLaunchToggleOff()
       //// {
       ////     //bug.Log("onAppLaunchToggleOff");
       ////     ;
       //// }
       //// void onAppLaunchHoverOn()
       //// {
       ////     Debug.Log("HoverOn");



       ////     pTG = new RUIPanelTabGroup();

       ////     Debug.Log("HoverOn2");

       ////     //pTG.panelManager();

       ////     app = this;

       ////     Debug.Log("HoverOn3");

       ////     var a = ApplicationLauncher.Instance.anchor.transform.position;
       ////     Debug.Log(a);



       ////     //anchor.transform.position = app.transform.position;

       ////     Debug.Log("HoverOn4");


       ////     Debug.Log("anchor " + app.anchor.transform.position);


       ////     //bug.Log("onAppLaunchHoverOn");
       ////     ;
       //// }
       //// void onAppLaunchHoverOff()
       //// {
       ////     //Debug.Log("onAppLaunchHoverOff");
       ////     ;
       //// }
       //// void onAppLaunchEnable()
       //// {
       ////     ;
       //// }
       //// void onAppLaunchDisable()
       //// {
       ////     ;
       //// }

       //// bool isApplicationTrue()
       //// {
       ////     return false;
       //// }

    }
}
