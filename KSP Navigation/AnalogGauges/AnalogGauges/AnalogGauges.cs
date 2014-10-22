using System;
using UnityEngine;
using NavUtilLib;

namespace NavUtilLib.Analog
{
    public class AnalogHSI : InternalModule
    {
        [KSPField]
        string compassObject = "compassObject";

        [KSPField]
        string crsRotObject = "crsRotObject";

        [KSPField]
        string brgObject = "brgObject";

        [KSPField]
        string locNeedleObject = "locNeedleObject";
        [KSPField]
        float locMinDevMM = -111;
        [KSPField]
        float locMaxDevMM = 111;
        [KSPField]
        float locMMpDeg = 37;


        [KSPField]
        string gsNeedleObject = "gsNeedleObject";
        [KSPField]
        float gsMinDevMM = -150;
        [KSPField]
        float gsMaxDevMM = 150;
        [KSPField]
        float gsMMpDeg = 50;

        [KSPField]
        string dmeTenthsObject = "dmeTenthsObject";
        [KSPField]
        string dmeOnesObject = "dmeOnesObject";
        [KSPField]
        string dmeTensObject = "dmeTensObject";
        [KSPField]
        string dmeHundredsObject = "dmeHundredsObject";

        [KSPField]
        float rotRateDeg = 80;

        [KSPField]
        float transRateMM = 30;

        Transform compass;
        Quaternion compassInit;
        float compassCurrent;

        Transform locNeedle;
        Vector3 locInit;
        float locCurrent;

        Transform gsNeedle;
        Vector3 gsInit;
        float gsCurrent;

        Transform crsBug;
        Quaternion crsBugInt;

        Transform brgBug;
        Quaternion brgBugInt;

        Transform[] dme = new Transform[4];
        Quaternion[] dmeInt = new Quaternion[4];


        public override void OnAwake()
        {
            compass = internalProp.FindModelTransform(compassObject);
            compassInit = compass.transform.localRotation;

            locNeedle = internalProp.FindModelTransform(locNeedleObject);
            locInit = locNeedle.transform.localPosition;

            gsNeedle = internalProp.FindModelTransform(gsNeedleObject);
            gsInit = gsNeedle.transform.localPosition;


            crsBug = internalProp.FindModelTransform(crsRotObject);
            crsBugInt = crsBug.transform.localRotation;

            brgBug = internalProp.FindModelTransform(brgObject);
            brgBugInt = brgBug.transform.localRotation;

            dme[0] = internalProp.FindModelTransform(dmeTenthsObject);
            dme[1] = internalProp.FindModelTransform(dmeOnesObject);
            dme[2] = internalProp.FindModelTransform(dmeTensObject);
            dme[3] = internalProp.FindModelTransform(dmeHundredsObject);

            dmeInt[0] = dme[0].transform.localRotation;
            dmeInt[1] = dme[1].transform.localRotation;
            dmeInt[2] = dme[2].transform.localRotation;
            dmeInt[3] = dme[3].transform.localRotation;






            Debug.Log("MLS: Starting systems...");
            if (!NavUtilLib.GlobalVariables.Settings.navAidsIsLoaded)
                NavUtilLib.GlobalVariables.Settings.loadNavAids();

            if (!NavUtilLib.GlobalVariables.Materials.isLoaded)
                NavUtilLib.GlobalVariables.Materials.loadMaterials();

            //if (!var.Audio.isLoaded)
            NavUtilLib.GlobalVariables.Audio.initializeAudio();

            Debug.Log("MLS: Systems started successfully!");

        }

        public override void OnUpdate()
        {
            float deltaNum = new float();
            float rotateLim = new float();
            float transLim = new float();

            rotateLim = rotRateDeg * Time.deltaTime;
            transLim = transRateMM * Time.deltaTime;

            //Update Data
            NavUtilLib.GlobalVariables.FlightData.updateNavigationData();


            //bool locFlag;
            //bool bcFlag;
            //bool gsFlag;

            //locFlag = bcFlag = false;
            //gsFlag = true;
            //if (NavUtilLib.GlobalVariables.FlightData.locDeviation > 10 && NavUtilLib.GlobalVariables.FlightData.locDeviation < 170 || NavUtilLib.GlobalVariables.FlightData.locDeviation < -10 && NavUtilLib.GlobalVariables.FlightData.locDeviation > -170)
            //    locFlag = true;

            //if (NavUtilLib.GlobalVariables.FlightData.locDeviation < -90 || NavUtilLib.GlobalVariables.FlightData.locDeviation > 90)
            //{
            //    bcFlag = true;
            //}



            //Rotate the compass card (not subject to rate limits)?
            //compassCurrent = Mathf.Clamp((float)Utils.makeAngle0to360(FlightGlobals.ship_heading - compassCurrent), -rotateLim, rotateLim);
            compass.localRotation = compassInit * Quaternion.AngleAxis(-FlightGlobals.ship_heading, Vector3.forward);

            //Rotate the course bug (not subject to rate limits)?
            crsBug.localRotation = crsBugInt * Quaternion.AngleAxis(-FlightGlobals.ship_heading + NavUtilLib.GlobalVariables.FlightData.selectedRwy.hdg, Vector3.forward);

            //BRG
            brgBug.localRotation = brgBugInt * Quaternion.AngleAxis(-FlightGlobals.ship_heading + (float)NavUtilLib.GlobalVariables.FlightData.bearing, Vector3.forward);

            //Translate LOC needle
            //locCurrent = Mathf.Clamp((NavUtilLib.GlobalVariables.FlightData.locDeviation - locCurrent) * locMMpDeg, -transLim, transLim); //number of mm to move
            //locCurrent = Mathf.Clamp(locCurrent,locMinDevMM,locMaxDevMM); //limit the range of the needle


            locCurrent = Mathf.Clamp(NavUtilLib.GlobalVariables.FlightData.locDeviation * locMMpDeg, locMinDevMM, locMaxDevMM);
            locNeedle.localPosition = locInit + Vector3.right * locCurrent / 1000;

            gsCurrent = Mathf.Clamp(NavUtilLib.GlobalVariables.FlightData.gsDeviation * gsMMpDeg, gsMinDevMM, gsMaxDevMM);
            gsNeedle.localPosition = gsInit + Vector3.forward * gsCurrent / 1000;


            //deltaNum = Mathf.Clamp((NavUtilLib.GlobalVariables.FlightData.locDeviation - currentLoc) * locMMpDeg, -transLim, transLim); //number of mm to move
            //deltaNum = Mathf.Clamp(deltaNum + currentLoc,);
            //deltaNum = 


            //Rotate DME
            float numHelper = 0;
            float fDME = (float)NavUtilLib.GlobalVariables.FlightData.dme / 1000;
            numHelper = fDME % 1; //tenths
            float digit;

            dme[0].localRotation = dmeInt[0] * Quaternion.AngleAxis((numHelper) * 360 + 18, Vector3.forward);

            if (numHelper <= .9f) //not rolling over
            {
                digit = (int)Math.Abs(fDME / 1 % 10);
                dme[1].localRotation = dmeInt[1] * Quaternion.AngleAxis(
    digit * 36 + 18,
    Vector3.forward);

                digit = (int)Math.Abs(fDME / 10 % 10);
                dme[2].localRotation = dmeInt[2] * Quaternion.AngleAxis(
    digit * 36 + 18,
    Vector3.forward);

                digit = (int)Math.Abs(fDME / 100 % 10);
                dme[3].localRotation = dmeInt[3] *
                    Quaternion.AngleAxis(digit * 36 + 18,
    Vector3.forward);
            }


            else
            {
                digit = Math.Abs(fDME / 1 % 10);
                dme[1].localRotation = dmeInt[1] * Quaternion.AngleAxis(
                    AnalogGaugeUtils.numberRot(digit, numHelper),
                    Vector3.forward);


                digit = Math.Abs(fDME / 10 % 10);
                dme[2].localRotation = dmeInt[2] * Quaternion.AngleAxis(
                    AnalogGaugeUtils.numberRot(digit, numHelper),
                    Vector3.forward);

                digit = Math.Abs(fDME / 100 % 10);
                dme[3].localRotation = dmeInt[3] * Quaternion.AngleAxis(
                    AnalogGaugeUtils.numberRot(digit, numHelper),
                    Vector3.forward);
            }

            //Translate GS needle

            //Translate BKCRS Marker

            //Rotate GS Flag

            //Rotate LOC Flag


            ;
        }

    }



    public static class AnalogGaugeUtils
    {
        public static float numberRot(float value, float dec)
        {
            float amt = (int)(value/1) * 36 + 18;

            if((value %1) <= .9f || (int)(value * 10) % 10 <= .9f)
            {
              return amt;
            }

                //if (dec > .9f)
                //{
                    dec -= .9f;
                    amt += dec * 360;
                //}

            return amt;
        }


        public static void single_Axis_Rotate(GameObject gameObject, Char axisOfRot, float amountDegs)
        {
            Vector3 axisVector = Vector3.zero;

            switch (axisOfRot)
            {
                case 'X':
                case 'x':
                    axisVector.x = 1;
                    break;

                case 'Y':
                case 'y':
                    axisVector.y = 1;
                    break;

                case 'Z':
                case 'z':
                    axisVector.z = 1;
                    break;

                default:
                        Debug.Log("NavUtilLib.AnalogGaugeUtils.single_Axis_Rotate: No axis found");
                        break;
            }

            gameObject.transform.Rotate(axisVector, amountDegs);
        }

        public static void single_Axis_Translate(GameObject gameObject, Char axisOfTrans, float amountMillimeters)
        {
            Vector3 axisVector = Vector3.zero;

            switch (axisOfTrans)
            {
                case 'X':
                case 'x':
                    axisVector.x = amountMillimeters;
                    break;

                case 'Y':
                case 'y':
                    axisVector.y = amountMillimeters;
                    break;

                case 'Z':
                case 'z':
                    axisVector.z = amountMillimeters;
                    break;

                default:
                    Debug.Log("NavUtilLib.AnalogGaugeUtils.single_Axis_Translate: No axis found");
                    break;
            }

            gameObject.transform.Translate(axisVector);
        }

    }
}
