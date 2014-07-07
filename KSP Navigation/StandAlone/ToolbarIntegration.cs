using System;
using UnityEngine;
using Toolbar;
using NavUtilLib;
using var = NavUtilLib.GlobalVariables;


[KSPAddon(KSPAddon.Startup.EveryScene, false)]
class btnCreate : MonoBehaviour
{
    private IButton b;

    private bool HSIguiState = false;

    private GUIInstruments.HSI_Display hsiD;

    private Rect windowPosition;
    private RenderTexture rt;

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
        option1.OnClick += (e2) => Debug.Log("menu option 1 clicked");

        if (HSIguiState)
        {
            IButton option2 = menu.AddOption("Close HSI Window");
            option2.OnClick += (e2) => displayHSI();
        }
        else
        {
            IButton option2 = menu.AddOption("Open HSI Window");
            option2.OnClick += (e2) => displayHSI();
        }

        menu.OnAnyOptionClicked += () => destroyPopupMenu(button);

        button.Drawable = menu;
    }

    private void displayHSI()
    {
        if(!HSIguiState)
        {
            
            Activate(true);

            HSIguiState = true;
        }
        else
        {
            Activate(false);

            HSIguiState = false;
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
    }


    public void Activate(bool state)
    {
        if (state)
        {
            RenderingManager.AddToPostDrawQueue(3, OnDraw);

            windowPosition = NavUtilLib.GlobalVariables.Settings.hsiPosition;

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
            NavUtilLib.GlobalVariables.Settings.hsiPosition = windowPosition;
        }
    }

    private void OnDraw()
    {
        if ((windowPosition.xMin + windowPosition.width) < 20) windowPosition.xMin = 20 - windowPosition.width;
        if (windowPosition.yMin + windowPosition.height < 20) windowPosition.yMin = 20 - windowPosition.height;
        if (windowPosition.xMin > Screen.width - 20) windowPosition.xMin = Screen.width - 20;
        if (windowPosition.yMin > Screen.height - 20) windowPosition.yMin = Screen.height - 20;
        windowPosition = GUI.Window(1, windowPosition, OnWindow, "Horizontal Situation Indicator");
    }

    private void DrawGauge(RenderTexture screen)
    {
                        if(!NavUtilLib.GlobalVariables.Materials.isLoaded) NavUtilLib.GlobalVariables.Materials.loadMaterials();
                        if (!NavUtilLib.GlobalVariables.Settings.navAidsIsLoaded) NavUtilLib.GlobalVariables.Settings.loadNavAids();
                NavUtilLib.GlobalVariables.Audio.initializeAudio();
                NavUtilLib.GlobalVariables.FlightData.updateNavigationData();

                RenderTexture pt = RenderTexture.active;
                RenderTexture.active = screen;

                if (!screen.IsCreated()) screen.Create();

                NavUtilLib.GlobalVariables.DisplayData.DrawHSI(screen,1);

                RenderTexture.active = pt;
    }

    private void OnWindow(int WindowID)
    {
        if (CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.Flight)
        {
            DrawGauge(rt);
            GUI.DrawTexture(new Rect(0, 0, NavUtilLib.GlobalVariables.Settings.hsiPosition.width,NavUtilLib.GlobalVariables.Settings.hsiPosition.height), rt, ScaleMode.ScaleToFit);
            GUI.DragWindow();
        }
    }


}
