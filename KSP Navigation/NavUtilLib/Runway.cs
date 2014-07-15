using System;

namespace NavUtilLib
{
    public class Runway
    {
        //[KSPField]
        public string ident = "runwayID";

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
        public float outerMarkerDist = -1000;

        //[KSPField]
        public float middleMarkerDist = -1000;

        //[KSPField]
        public float innerMarkerDist = -1000;
    }
}
