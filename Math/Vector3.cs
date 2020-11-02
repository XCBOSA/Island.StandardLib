using Island.StandardLib.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Math
{
    public struct Vector3 : IStorable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public static Vector3 Zero = new Vector3(0f, 0f, 0f);

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 Make(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }
        
        /// <summary>
        /// 使用通信数据初始化
        /// </summary>
        /// <param name="xyz">X:Y:Z</param>
        public Vector3(string xyz)
        {
            string[] l = xyz.Split(':');
            if (l.Length == 3)
            {
                if (float.TryParse(l[0], out float x) &&
                    float.TryParse(l[1], out float y) &&
                    float.TryParse(l[2], out float z))
                {
                    X = x;
                    Y = y;
                    Z = z;
                }
                else
                {
                    X = Y = Z = 0;
                }
            }
            else X = Y = Z = 0;
        }

        /// <summary>
        /// 获取以XZ为坐标系的法点
        /// </summary>
        /// <returns></returns>
        public Vector2 GetNormalVectorXZ()
        {
            return new Vector2(X, Z);
        }

        /// <summary>
        /// 向量加法
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vector3 ADD(Vector3 vec)
        {
            return new Vector3(X + vec.X, Y + vec.Y, Z + vec.Z);
        }

        /// <summary>
        /// 向量减法
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vector3 RED(Vector3 vec)
        {
            return new Vector3(X - vec.X, Y - vec.Y, Z - vec.Z);
        }
        
        /// <summary>
        /// 向量数乘
        /// </summary>
        /// <param name="vecdx"></param>
        /// <returns></returns>
        public Vector3 MUL(float vecdx)
        {
            return new Vector3(vecdx * X, Y * vecdx, Z * vecdx);
        }

        /// <summary>
        /// 向量点乘 (内积) 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public float DOT(Vector3 vec)
        {
            return X * vec.X + Y * vec.Y + Z * vec.Z;
        }

        /// <summary>
        /// 向量叉乘 (外积) 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vector3 FMUL(Vector3 vec)
        {
            return new Vector3(
                Y * vec.Z - vec.Y * Z,
                -1 * (X * vec.Z - vec.X * Z),
                X * vec.Y - vec.X * Y);
        }

        /// <summary>
        /// 如果向量表示点的坐标，计算这个点和指定参数点之间的距离
        /// </summary>
        /// <param name="vec">指定点</param>
        /// <returns></returns>
        public float DistanceOf(Vector3 vec)
        {
            float x1 = (float)System.Math.Pow(X - vec.X, 2);
            float x2 = (float)System.Math.Pow(Y - vec.Y, 2);
            float x3 = (float)System.Math.Pow(Z - vec.Z, 2);
            return (float)System.Math.Pow(x1 + x2 + x3, 0.5d);
        }

        /// <summary>
        /// 如果向量表示点的坐标，计算这个点和指定参数点之间的以x, z为平面的距离
        /// </summary>
        /// <param name="vec">指定点</param>
        /// <returns></returns>
        public float DistanceOf2D(Vector3 vec)
        {
            float x1 = (float)System.Math.Pow(X - vec.X, 2);
            float x3 = (float)System.Math.Pow(Z - vec.Z, 2);
            return (float)System.Math.Pow(x1 + x3, 0.5d);
        }

        public Vector3 ToCameraRotation()
        {
            Vector3 vec = new Vector3();
            vec.X = X;
            vec.Z = Z;
            vec.Y = Y + 180f;
            while (vec.Y > 360f)
                vec.Y -= 360f;
            return vec;
        }

        public static Vector3 Random(float xyzrange)
        {
            return Random(xyzrange, xyzrange, xyzrange);
        }

        public static Vector3 Random(float xrange, float yrange, float zrange)
        {
            Random rd = new Random();
            float fx = (float)(rd.NextDouble() - 0.5) * xrange * 2f,
                fy = (float)(rd.NextDouble() - 0.5) * yrange * 2f,
                fz = (float)(rd.NextDouble() - 0.5) * zrange * 2f;
            return new Vector3(fx, fy, fz);
        }

        public string ToXYZ()
        {
            return X + ":" + Y + ":" + Z;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }

        public bool IsMathNull
        {
            get
            {
                return X < 0 || Y < 0 || Z < 0;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3)) return false;
            Vector3 d = (Vector3)obj;
            return X == d.X && Y == d.Y && Z == d.Z;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(X);
            data.Write(Y);
            data.Write(Z);
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out float x);
            data.Read(out float y);
            data.Read(out float z);
            X = x;
            Y = y;
            Z = z;
        }

        public static bool operator ==(Vector3 a, Vector3 b) => a.Equals(b);
        public static bool operator !=(Vector3 a, Vector3 b) => !a.Equals(b);
        public static Vector3 operator *(Vector3 a, float b) => a.MUL(b);
        public static Vector3 operator *(float a, Vector3 b) => b.MUL(a);
        public static Vector3 operator +(Vector3 a, Vector3 b) => a.ADD(b);
        public static Vector3 operator -(Vector3 a, Vector3 b) => a.RED(b);
    }
}
