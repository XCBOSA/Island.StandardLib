using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Math
{
    public struct Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static Vector2 Zero = new Vector2(0, 0);

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
        
        /// <summary>
        /// 使用通信数据初始化
        /// </summary>
        /// <param name="xyz">X:Y:Z</param>
        public Vector2(string xy)
        {
            string[] l = xy.Split(':');
            if (l.Length == 2)
            {
                if (float.TryParse(l[0], out float x) &&
                    float.TryParse(l[1], out float y))
                {
                    X = x;
                    Y = y;
                }
                else
                {
                    X = Y = 0;
                }
            }
            else X = Y = 0;
        }

        /// <summary>
        /// 提升维度
        /// </summary>
        /// <param name="y">初始高度</param>
        /// <returns></returns>
        public Vector3 UpperXZ(float y)
        {
            return new Vector3(X, y, Y);
        }

        /// <summary>
        /// 向量加法
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vector2 ADD(Vector2 vec)
        {
            return new Vector2(X + vec.X, Y + vec.Y);
        }

        /// <summary>
        /// 向量减法
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vector2 RED(Vector2 vec)
        {
            return new Vector2(X - vec.X, Y - vec.Y);
        }

        /// <summary>
        /// 如果向量表示点的坐标，计算这个点和指定参数点之间的距离
        /// </summary>
        /// <param name="vec">指定点</param>
        /// <returns></returns>
        public float DistanceOf(Vector2 vec)
        {
            float x1 = (float)System.Math.Pow(X - vec.X, 2);
            float x2 = (float)System.Math.Pow(Y - vec.Y, 2);
            return (float)System.Math.Pow(x1 + x2, 0.5d);
        }

        public string ToXY()
        {
            return X + ":" + Y;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2)) return false;
            Vector2 v2 = (Vector2)obj;
            return X == v2.X && Y == v2.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !a.Equals(b);
        }
    }
}
