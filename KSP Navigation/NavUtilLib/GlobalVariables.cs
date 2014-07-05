using System;
using UnityEngine;
using KSP;

namespace NavUtilLib
{
    namespace GlobalVariables
    {
        public class Materials
        {
            private static Materials instance;
                private Materials(){}

            public static Materials Instance
                {
                get
                    {
                    if (instance == null)
                    {
                        instance = new Materials();
                    }
                    return instance;
                    }
                }

            public Material overlay = null;
            public Material pointer = null;
            public Material headingCard = null;
            public Material NDBneedle = null;
            public Material course = null;
            public Material localizer = null;
            public Material mkrbcn = null;
            public Material flag = null;
            public Material back = null;

            public static void loadMaterials()
        {
            Debug.Log("loadMaterials()");
            string texName;

            texName = "hsi_overlay.png";
            Materials.Instance.overlay = Graphics.loadMaterial(texName, Materials.Instance.overlay, 640, 640);

            texName = "hsi_gs_pointer.png";
            Materials.Instance.pointer = Graphics.loadMaterial(texName, Materials.Instance.pointer, 640, 24);

            texName = "hsi_large_heading_card.png";
            Materials.Instance.headingCard = Graphics.loadMaterial(texName, Materials.Instance.headingCard, 501, 501);

            texName = "hsi_NDB_needle.png";
            Materials.Instance.NDBneedle = Graphics.loadMaterial(texName, Materials.Instance.NDBneedle, 15, 501);

            texName = "hsi_course_needle.png";
            Materials.Instance.course = Graphics.loadMaterial(texName, Materials.Instance.course, 221, 481);

            texName = "hsi_course_deviation_needle.png";
            Materials.Instance.localizer = Graphics.loadMaterial(texName, Materials.Instance.localizer, 5, 251);

            texName = "hsi_markerIndicator.png";
            Materials.Instance.mkrbcn = Graphics.loadMaterial(texName, Materials.Instance.mkrbcn, 175, 180);

            texName = "hsi_flags.png";
            Materials.Instance.flag = Graphics.loadMaterial(texName, Materials.Instance.flag, 64, 64);

            texName = "hsi_back.png";
            Materials.Instance.back = Graphics.loadMaterial(texName, Materials.Instance.back, 32, 32);
        }
        }

        public static class Audio
        {
            public static GameObject audioplayer = new GameObject();
            public static AudioSource markerAudio = new AudioSource();

            public static void initializeAudio()
            {
                markerAudio = audioplayer.AddComponent<AudioSource>();
                markerAudio.volume = GameSettings.VOICE_VOLUME;
                markerAudio.pan = 0;
                markerAudio.dopplerLevel = 0;
                markerAudio.bypassEffects = true;
                markerAudio.loop = true;
            }
        }
    }
}