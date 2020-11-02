using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Math
{
    /// <summary>
    /// 表示一个二维向量，精度为 <see cref="int"/>
    /// </summary>
    public struct Vector2Int
    {
        public static Vector2Int Zero = new Vector2Int(0, 0);

        /// <summary>
        /// X分量
        /// </summary>
        public int X;
        /// <summary>
        /// Y分量
        /// </summary>
        public int Y;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="x">X分量</param>
        /// <param name="y">Y分量</param>
        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 判断对象是否相等
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Vector2Int)) return false;
            Vector2Int dest = (Vector2Int)obj;
            return dest.X == X && dest.Y == Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <summary>
        /// 使用通信数据初始化
        /// </summary>
        /// <param name="xyz">X:Y:Z</param>
        public Vector2Int(string xy)
        {
            string[] l = xy.Split(':');
            if (l.Length == 2)
            {
                if (int.TryParse(l[0], out int x) &&
                    int.TryParse(l[1], out int y))
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
        /// 向量加法
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vector2Int ADD(Vector2Int vec)
        {
            return new Vector2Int(X + vec.X, Y + vec.Y);
        }

        /// <summary>
        /// 向量减法
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vector2Int RED(Vector2Int vec)
        {
            return new Vector2Int(X - vec.X, Y - vec.Y);
        }

        /// <summary>
        /// 向量数乘
        /// </summary>
        /// <param name="vecdx"></param>
        /// <returns></returns>
        public Vector2Int MUL(int vecdx)
        {
            return new Vector2Int(vecdx * X, Y * vecdx);
        }

        /// <summary>
        /// 如果向量表示点的坐标，计算这个点和指定参数点之间的距离
        /// </summary>
        /// <param name="vec">指定点</param>
        /// <returns></returns>
        public int DistanceOf(Vector2Int vec)
        {
            int x1 = (int)System.Math.Pow(X - vec.X, 2);
            int x2 = (int)System.Math.Pow(Y - vec.Y, 2);
            return (int)System.Math.Pow(x1 + x2, 0.5d);
        }

        /// <summary>
        /// 获取通信数据表示
        /// </summary>
        /// <returns></returns>
        public string ToXY()
        {
            return X + ":" + Y;
        }

        /// <summary>
        /// 获取数对表示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }

        public static bool operator ==(Vector2Int a, Vector2Int b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2Int a, Vector2Int b)
        {
            return !a.Equals(b);
        }
    }
}
