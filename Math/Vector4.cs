using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Math
{
    public struct Vector4
    {
        public static Vector4 Zero = new Vector4(0, 0, 0, 0);

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// 使用通信数据初始化
        /// </summary>
        /// <param name="xyz">X:Y:Z</param>
        public Vector4(string xyzw)
        {
            string[] l = xyzw.Split(':');
            if (l.Length == 3)
            {
                if (float.TryParse(l[0], out float x) &&
                    float.TryParse(l[1], out float y) &&
                    float.TryParse(l[2], out float z) &&
                    float.TryParse(l[2], out float w))
                {
                    X = x;
                    Y = y;
                    Z = z;
                    W = w;
                }
                else
                {
                    X = Y = Z = W = 0;
                }
            }
            else X = Y = Z = W = 0;
        }

        public string ToXYZW()
        {
            return X + ":" + Y + ":" + Z + ":" + W;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ", " + W + ")";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector4)) return false;
            Vector4 d = (Vector4)obj;
            return X == d.X && Y == d.Y && Z == d.Z && W == d.W;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public static bool operator ==(Vector4 a, Vector4 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector4 a, Vector4 b)
        {
            return !a.Equals(b);
        }
    }
}
