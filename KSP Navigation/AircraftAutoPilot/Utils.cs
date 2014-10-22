//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using NavUtilLib;
//using KSP;

//namespace AircraftAutoPilot
//{
//    [KSPAddon(KSPAddon.Startup.Flight, false)]
//    public class AutoPilotApp : MonoBehaviour
//    {
//        private LineRenderer line = null;

//        [Flags]
//        public enum Regime
//        {
//            Autopilot_Off = 0,
//            LNav = 0x01,
//            VNav = 0x02,
//            Flare = 0x04,
//            Rollout = 0x08
//        }


//        Rect windowPosition;

//        Regime CurrentRegime;

//        public void Awake()
//        {
//            RenderingManager.AddToPostDrawQueue(3, DrawWindow);

//            windowPosition.x = 0;
//            windowPosition.y = 150;
//            windowPosition.width = 215;
//            windowPosition.height = 250;

//            GameObject obj = new GameObject("Line");
//            line = obj.AddComponent<LineRenderer>();
//            line.transform.parent = transform; // ...child to our part...
//            line.useWorldSpace = false; // ...and moving along with it (rather 
//            // than staying in fixed world coordinates)
//            line.transform.localPosition = Vector3.zero;
//            line.transform.localEulerAngles = Vector3.zero;

//            // Make it render a red to yellow triangle, 1 meter wide and 2 meters long
//            line.material = new Material(Shader.Find("Particles/Additive"));
//            line.SetColors(Color.red, Color.yellow);
//            line.SetWidth(2, 0);
//            line.SetVertexCount(2);
//            line.SetPosition(0, Vector3.zero);
//            line.SetPosition(1, Vector3.forward * 4); 
//        }

//        public void Update()
//        {
//            ;
//        }

//        void DrawWindow()
//        {
//                if ((windowPosition.xMin + windowPosition.width) < 20) windowPosition.xMin = 20 - windowPosition.width;
//                if (windowPosition.yMin + windowPosition.height < 20) windowPosition.yMin = 20 - windowPosition.height;
//                if (windowPosition.xMin > Screen.width - 20) windowPosition.xMin = Screen.width - 20;
//                if (windowPosition.yMin > Screen.height - 20) windowPosition.yMin = Screen.height - 20;

//                windowPosition = GUI.Window(80661, windowPosition, DrawWindow, "Autopilot");
//        }

//        private void DrawWindow(int WindowID)
//        {
//            GUI.Label(new Rect(5, 20, 205, 20), CurrentRegime.ToString());

//            if(GUI.Button(new Rect(5,50,100,20),"LNav"))
//            {
//               CurrentRegime ^= Regime.LNav;

//               if ((CurrentRegime & Regime.LNav) == Regime.LNav)
//               {
//                   Debug.Log("LNav Now On");

//                   Variables.LNavTarget = (float)FlightGlobals.ship_heading;
//               }
//            }

//            if (GUI.Button(new Rect(110, 50, 100, 20), "VNav"))
//            {
//                CurrentRegime ^= Regime.VNav;

//                if((CurrentRegime & Regime.VNav) == Regime.VNav)
//                {
//                    Debug.Log("VNav Now On");

//                    Variables.VNavTarget = (float)FlightGlobals.ship_altitude;
//                }
//            }




//            if (GUI.Button(new Rect(5, 80, 100, 20), "Turn 90° Left"))
//            {
//                Variables.LNavTarget = (float)NavUtilLib.Utils.makeAngle0to360(FlightGlobals.ship_heading - 90f);
//            }

//            if (GUI.Button(new Rect(110, 80, 100, 20), "Turn 45° Right"))
//            {
//                Variables.LNavTarget = (float)NavUtilLib.Utils.makeAngle0to360(FlightGlobals.ship_heading + 45f);
//            }

//            GUI.Label(new Rect(5, 110, 205, 35), "LNav Target "+Variables.LNavTarget.ToString()+"°  VNav Target "+Variables.VNavTarget.ToString()+"m");

//            Vessel vessel = FlightGlobals.ActiveVessel;

//            Vector3 attitude;

//            Vector3 CoM = vessel.findWorldCenterOfMass();

//            Vector3d north = Vector3d.Exclude(vessel.upAxis, (vessel.mainBody.position + vessel.mainBody.transform.up * (float)vessel.mainBody.Radius) - CoM).normalized;
//            Quaternion rotationSurface = Quaternion.LookRotation(north, vessel.upAxis);

//            Quaternion rotationVesselSurface = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(vessel.GetTransform().rotation) * rotationSurface);

//                attitude.x = (rotationVesselSurface.eulerAngles.x > 180) ? (float)(360.0 - rotationVesselSurface.eulerAngles.x) : -rotationVesselSurface.eulerAngles.x;
//                attitude.z = (rotationVesselSurface.eulerAngles.z > 180) ? (float)(rotationVesselSurface.eulerAngles.z - 360.0) : rotationVesselSurface.eulerAngles.z;

//                GUI.Label(new Rect(5, 145, 205, 45), "Pitch " + attitude.x.ToString("F3") + "°  Roll " + attitude.z.ToString("F3") + "°");

//                attitude = vessel.srf_velocity;

//                attitude = FlightGlobals.ship_velocity;

//                attitude = vessel.acceleration;

//                Transform t = new GameObject().transform;

//                t.position = vessel.GetWorldPos3D();
//                t.rotation = Quaternion.Euler(vessel.upAxis);
//                t.rotation = Quaternion.Euler(Vector3.Cross(Vector3.right, vessel.upAxis));
//                attitude = (Vector3)t.InverseTransformDirection(vessel.srf_velocity); //this gives velocity vector in worldspace?


//                attitude = vessel.srf_velocity;

//            //vessel.



//                t.rotation = vessel.transform.rotation;

//                attitude = (Vector3)t.InverseTransformDirection(vessel.srf_velocity);


//                //rotationVesselSurface = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(Quaternion.Euler(attitude)) * rotationSurface);

//                //attitude.x = (rotationVesselSurface.eulerAngles.x > 180) ? (float)(360.0 - rotationVesselSurface.eulerAngles.x) : -rotationVesselSurface.eulerAngles.x;
//                //attitude.z = (rotationVesselSurface.eulerAngles.z > 180) ? (float)(rotationVesselSurface.eulerAngles.z - 360.0) : rotationVesselSurface.eulerAngles.z;


//                GUI.Label(new Rect(5, 195, 205, 45), "X " + attitude.x.ToString("F2") + "m  Y " + attitude.y.ToString("F2") + "m  Z " + attitude.z.ToString("F2") + "m");

//                line.transform.position = vessel.transform.position;

//                line.transform.rotation = Quaternion.LookRotation(attitude);

//            GUI.DragWindow();
//        }
//    }


//        public static class Utils
//        {
//            public static float CalcStdRateTurnRadius(float Airspeed, float degPerSec)
//            {
//                float r = 0;

//                r = (Airspeed * 3600) / (degPerSec * 20 * Mathf.PI);
//                return r;
//            }

//            public static float CalcBankAngle(float Airspeed, float Radius, float Gee)
//            {
//                float A = 0;

//                Debug.Log((Airspeed * Airspeed) / Radius / Gee);

//                A = (float)NavUtilLib.Utils.CalcDegFromRadians(Math.Atan((Airspeed * Airspeed) / Radius / Gee));

//                return A;
//            }
//        }

//        public static class Variables
//        {
//            public static float LNavTarget = 0;
//            public static float VNavTarget = 0;
//        }
    
//}
