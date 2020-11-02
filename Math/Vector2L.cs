using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Math
{
    public struct Vector2L
    {
        int x, y;

        public float X
        {
            get
            {
                return x / 10f;
            }
            set
            {
                x = (int)(value * 10f);
            }
        }

        public float Y
        {
            get
            {
                return y / 10f;
            }
            set
            {
                y = (int)(value * 10f);
            }
        }

        public static Vector2L Zero = new Vector2L(0f, 0f);

        public Vector2L(float ix, float iy)
        {
            x = (int)(ix * 10f);
            y = (int)(iy * 10f);
        }
        
        /// <summary>
        /// 使用通信数据初始化
        /// </summary>
        /// <param name="xyz">X:Y:Z</param>
        public Vector2L(string xy)
        {
            string[] l = xy.Split(':');
            if (l.Length == 2)
            {
                if (float.TryParse(l[0], out float ix) &&
                    float.TryParse(l[1], out float iy))
                {
                    x = (int)(ix * 10f);
                    y = (int)(iy * 10f);
                }
                else
                {
                    x = y = 0;
                }
            }
            else x = y = 0;
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
        public Vector2L ADD(Vector2L vec)
        {
            return new Vector2L(X + vec.X, Y + vec.Y);
        }

        /// <summary>
        /// 向量减法
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vector2L RED(Vector2L vec)
        {
            return new Vector2L(X - vec.X, Y - vec.Y);
        }

        /// <summary>
        /// 如果向量表示点的坐标，计算这个点和指定参数点之间的距离
        /// </summary>
        /// <param name="vec">指定点</param>
        /// <returns></returns>
        public float DistanceOf(Vector2L vec)
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
            if (!(obj is Vector2L)) return false;
            Vector2L v2 = (Vector2L)obj;
            return X == v2.X && Y == v2.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Vector2L a, Vector2L b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2L a, Vector2L b)
        {
            return !a.Equals(b);
        }

        public int ID
        {
            get
            {
                return x.GetHashCode() ^ (y.GetHashCode() << 2);
            }
        }
    }
}
