using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Math
{
    /// <summary>
    /// 提供一个参数化的非线性周期函数拟合
    /// </summary>
    public class NonlinearPeriodicFunc
    {
        const double PI2 = 6.2831853071795862;

        /// <summary>
        /// 最低值 (x=周期点)
        /// </summary>
        public float Low { get; set; }

        /// <summary>
        /// 最高值 (x=周期中点)
        /// </summary>
        public float High { get; set; }

        /// <summary>
        /// 周期长度
        /// </summary>
        public float Length { get; set; }

        public NonlinearPeriodicFunc(float low, float high, float length)
        {
            Low = low;
            High = high;
            Length = length;
        }

        public float f(float inputx)
        {
            return (float)(-System.Math.Cos(PI2 * inputx / Length) + 1) * ((High - Low) * 0.5f) + Low;
        }
    }
}
