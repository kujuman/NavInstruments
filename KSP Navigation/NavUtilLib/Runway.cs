//NavUtilities by kujuman, © 2014. All Rights Reserved.

using System;

namespace NavUtilLib
{
    public class Runway
    {
        //[KSPField]
        public string ident = "runwayID";

        public string shortID = "sID4";

        //[KSPField]
        public float hdg = 90;

        //[KSPField]
        public string body = "Kerbin";

        //[KSPField]
        public float altMSL = 100;

        //[KSPField]
        public float gsLatitude;

        //[KSPField]
        public float gsLongitude;

        //[KSPField]
        public float locLatitude;

        //[KSPField]
        public float locLongitude;

        //[KSPField]
        public float outerMarkerDist = 10000;

        //[KSPField]
        public float middleMarkerDist = 2200;

        //[KSPField]
        public float innerMarkerDist = 200;
        
        public bool isINSTarget = false; //true indicates that the runway is not the actual runway and is used as a target point for INS 
    }
}
