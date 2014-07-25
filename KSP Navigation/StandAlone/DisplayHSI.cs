//NavUtilities by kujuman, © 2014. All Rights Reserved.

//using System;
//using UnityEngine;
//using KSP;
//using NavUtilLib;
//using var = NavUtilLib.GlobalVariables;

//namespace GUIInstruments
//{

//    public class HSI_Display : MonoBehaviour
//    {
//        private Rect windowPosition;
//        private RenderTexture rt;

//        private GameObject go;
//        private Camera c;


//        public void Activate(bool state)
//        {
//            if (state)
//            {
//                RenderingManager.AddToPostDrawQueue(3, OnDraw);

//                //Scale = NavUtilLib.GlobalVariables.Settings.hsiScale;
//                windowPosition = NavUtilLib.GlobalVariables.Settings.hsiPosition;

//                rt = new RenderTexture((int)windowPosition.width, (int)windowPosition.height, 24, RenderTextureFormat.ARGB32);

//                Debug.Log("ILS: Starting systems...");
//                if (!var.Settings.navAidsIsLoaded)
//                    var.Settings.loadNavAids();

//                if (!var.Materials.isLoaded)
//                    var.Materials.loadMaterials();

//                //if (!var.Audio.isLoaded)
//                var.Audio.initializeAudio();

//                //ConfigureCamera();

//                Debug.Log("ILS: Systems started successfully!");
//            }
//            else
//            {
//                state = false;
//                RenderingManager.RemoveFromPostDrawQueue(3, OnDraw); //close the GUI
//                NavUtilLib.GlobalVariables.Settings.hsiPosition = windowPosition;
//            }
//        }

//        private void OnDraw()
//        {
//            if ((windowPosition.xMin + windowPosition.width) < 20) windowPosition.xMin = 20 - windowPosition.width;
//            if (windowPosition.yMin + windowPosition.height < 20) windowPosition.yMin = 20 - windowPosition.height;
//            if (windowPosition.xMin > Screen.width - 20) windowPosition.xMin = Screen.width - 20;
//            if (windowPosition.yMin > Screen.height - 20) windowPosition.yMin = Screen.height - 20;
//            windowPosition = GUI.Window(1, windowPosition, OnWindow, "Horizontal Situation Indicator");
//        }

//        private void OnWindow(int WindowID)
//        {

//            if (CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.Flight)
//            {
//                RenderTexture prior = RenderTexture.active;

//                if(!rt.IsCreated()) rt.Create();

//                rt.DiscardContents();

//                RenderTexture.active = rt;
//                NavUtilLib.DisplayData.DrawHSI(rt, windowPosition.width / windowPosition.height);
//                UnityEngine.Graphics.SetRenderTarget(rt);
//                GUILayout.Box(rt);

//                RenderTexture.active = prior;
//                GUI.DragWindow();
//            }
//        }
//    }
//}