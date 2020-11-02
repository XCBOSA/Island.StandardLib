using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Math
{
    /// <summary>
    /// 表示一个二维矩形
    /// </summary>
    public class Rect2
    {
        /// <summary>
        /// 矩形的最小X坐标
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// 矩形的最小Y坐标
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// 矩形的宽度
        /// </summary>
        public float SizeX { get; set; }

        /// <summary>
        /// 矩形的高度
        /// </summary>
        public float SizeY { get; set; }

        public Rect2(Vector2 start, Vector2 size)
        {
            X = start.X;
            Y = start.Y;
            SizeX = size.X;
            SizeY = size.Y;
        }

        public Rect2(float x, float y, float sizeX, float sizeY)
        {
            X = x;
            Y = y;
            SizeX = sizeX;
            SizeY = sizeY;
        }

        /// <summary>
        /// 选定点是否在此矩形内
        /// </summary>
        /// <param name="point">选定的点</param>
        /// <returns></returns>
        public bool Contains(Vector2 point)
        {
            return X < point.X && X + SizeX > point.X && Y < point.Y && Y + SizeY > point.Y;
        }

        /// <summary>
        /// 获取矩形的中间点
        /// </summary>
        /// <returns></returns>
        public Vector2 GetNormalVector()
        {
            return new Vector2(X + (SizeX / 2), Y + (SizeY / 2));
        }
    }
}
