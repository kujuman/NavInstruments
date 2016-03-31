//NavUtilities by kujuman, © 2014. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NavUtilLib
{
    public static class TextWriter
    {
        public static RenderTexture addTextToRT(RenderTexture rt, string Text, Vector2 position, Material mat, float scale)
        {
            //Debug.Log("Printing: " + Text);

            GL.PushMatrix();
            GL.LoadPixelMatrix(0, rt.width, rt.height, 0);
            GL.Viewport(new Rect(0, 0, rt.width, rt.height));
            var s = Text;
            s = s.ToUpper();
            float xOff = position.x;


            foreach(Char c in s)
            {
                Rect r = getTexCoordinate(c);

                float w = r.width * scale;
                float h = r.height * scale;

                mat.SetPass(0);
                GL.LoadOrtho();
                GL.Color(Color.red);
                GL.Begin(GL.QUADS);

                GL.TexCoord2(r.x/mat.mainTexture.width, r.y/mat.mainTexture.height);
                GL.Vertex3(xOff / rt.width, position.y / rt.height, 0);


                GL.TexCoord2(r.x / mat.mainTexture.width, (r.y + r.height)/mat.mainTexture.height);
                GL.Vertex3(xOff / rt.width, (position.y + h)/ rt.height, 0);


                xOff += w;

                GL.TexCoord2((r.x + r.width) / mat.mainTexture.width, (r.y + r.height) / mat.mainTexture.height);
                GL.Vertex3(xOff / rt.width, (position.y + h) / rt.height, 0);


                GL.TexCoord2((r.x + r.width) / mat.mainTexture.width, r.y / mat.mainTexture.height);
                GL.Vertex3(xOff / rt.width, position.y / rt.height, 0);

                GL.End();

            }
            GL.PopMatrix();

            return rt;
        }

        private static Rect getTexCoordinate(char character)
        {
            switch (character)
            {
                //row 1
                case 'A':
                    return new Rect(0, 210, 25, 50);

                case 'B':
                    return new Rect(25, 210, 25, 50);

                case 'C':
                    return new Rect(50, 210, 25, 50);

                case 'D':
                    return new Rect(75, 210, 25, 50);

                case 'E':
                    return new Rect(100, 210, 25, 50);

                case 'F':
                    return new Rect(125, 210, 25, 50);

                case 'G':
                    return new Rect(150, 210, 25, 50);

                case 'H':
                    return new Rect(175, 210, 25, 50);

                case 'I':
                    return new Rect(200, 210, 25, 50);

                case 'J':
                    return new Rect(225, 210, 25, 50);


                //row 2
                case 'K':
                    return new Rect(0, 160, 25, 50);

                case 'L':
                    return new Rect(25, 160, 25, 50);

                case 'M':
                    return new Rect(50, 160, 25, 50);

                case 'N':
                    return new Rect(75, 160, 25, 50);

                case 'O':
                    return new Rect(100, 160, 25, 50);

                case 'P':
                    return new Rect(125, 160, 25, 50);

                case 'Q':
                    return new Rect(150, 160, 25, 50);

                case 'R':
                    return new Rect(175, 160, 25, 50);

                case 'S':
                    return new Rect(200, 160, 25, 50);

                case 'T':
                    return new Rect(225, 160, 25, 50);

                //row 3

                case 'U':
                    return new Rect(0, 110, 25, 50);

                case 'V':
                    return new Rect(25, 110, 25, 50);

                case 'W':
                    return new Rect(50, 110, 25, 50);

                case 'X':
                    return new Rect(75, 110, 25, 50);

                case 'Y':
                    return new Rect(100, 110, 25, 50);

                case 'Z':
                    return new Rect(125, 110, 25, 50);


                case '0':
                    return new Rect(150, 110, 25, 50);

                case '1':
                    return new Rect(175, 110, 25, 50);

                case '2':
                    return new Rect(200, 110, 25, 50);

                case '3':
                    return new Rect(225, 110, 25, 50);


                //row 4
                case '4':
                    return new Rect(0, 60, 25, 50);

                case '5':
                    return new Rect(25, 60, 25, 50);

                case '6':
                    return new Rect(50, 60, 25, 50);

                case '7':
                    return new Rect(75, 60, 25, 50);

                case '8':
                    return new Rect(100, 60, 25, 50);

                case '9':
                    return new Rect(125, 60, 25, 50);


                case ':':
                    return new Rect(150, 60, 25, 50);

                case '.':
                    return new Rect(175, 60, 25, 50);

                case '°':
                    return new Rect(200, 60, 25, 50);

                case '→':
                    return new Rect(0, 10, 25, 50);

                default:
                    return new Rect(225, 60, 25, 50);
            }
            
        }
    }
}
