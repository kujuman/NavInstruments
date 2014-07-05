/*
 * This class adapted from Trueborn's SteamGauges for KSP mod. http://forum.kerbalspaceprogram.com/threads/40730-0-21-1-SteamGauges-Release-V1-2?highlight=gauges
 * 
 * Therefore, it is published under a Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License. http://creativecommons.org/licenses/by-nc-sa/3.0/
 * 
 */

using System;
using KSP;
using UnityEngine;

namespace KSF_NavInstruments
{
        public static class Resources
        {
            //Horizontal Situation Indicator                                                
            public static Texture2D hsi_heading_card = new Texture2D(501, 501);
            public static Texture2D hsi_course_needle = new Texture2D(221, 481);
            public static Texture2D hsi_course_deviation_needle = new Texture2D(5, 251);
            public static Texture2D hsi_back = new Texture2D(501, 501);
            public static Texture2D hsi_overlay = new Texture2D(501, 501);
            public static Texture2D hsi_NDB_needle = new Texture2D(15, 501);
            public static Texture2D hsi_GS_pointer = new Texture2D(501, 24);

            public static Texture2D base_digit_strip = new Texture2D(146, 17);

            public static void loadAssets()
            {
                Byte[] arrBytes;

                arrBytes = KSP.IO.File.ReadAllBytes<KSF_HSI_Display>("large_heading_card.png");
                hsi_heading_card.LoadImage(arrBytes);
                arrBytes = KSP.IO.File.ReadAllBytes<KSF_HSI_Display>("course_needle.png");
                hsi_course_needle.LoadImage(arrBytes);
                arrBytes = KSP.IO.File.ReadAllBytes<KSF_HSI_Display>("course_deviation_needle.png");
                hsi_course_deviation_needle.LoadImage(arrBytes);
                arrBytes = KSP.IO.File.ReadAllBytes<KSF_HSI_Display>("hsi_back.png");
                hsi_back.LoadImage(arrBytes);
                arrBytes = KSP.IO.File.ReadAllBytes<KSF_HSI_Display>("digit_strip.png");
                base_digit_strip.LoadImage(arrBytes);
                arrBytes = KSP.IO.File.ReadAllBytes<KSF_HSI_Display>("overlay.png");
                hsi_overlay.LoadImage(arrBytes);
                arrBytes = KSP.IO.File.ReadAllBytes<KSF_HSI_Display>("NDB_needle.png");
                hsi_NDB_needle.LoadImage(arrBytes);
                arrBytes = KSP.IO.File.ReadAllBytes<KSF_HSI_Display>("gs_pointer.png");
                hsi_GS_pointer.LoadImage(arrBytes);
            }

        }
    }
