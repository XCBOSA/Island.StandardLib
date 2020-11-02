using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Math
{
    /// <summary>
    /// 提供一个参数化的非线性偶函数拟合
    /// </summary>
    public class NonlinearEvenFunc
    {
        /// <summary>
        /// 峰高度 (x->0)
        /// </summary>
        public float Peak { get; set; }

        /// <summary>
        /// 标准高度 (x->inf)
        /// </summary>
        public float Standard { get; set; }

        /// <summary>
        /// 峰的平均变化速度
        /// </summary>
        public float AvgSpeed { get; set; }

        public NonlinearEvenFunc(float peak, float standard, float avgspeed)
        {
            Peak = peak;
            Standard = standard;
            AvgSpeed = avgspeed;
        }

        public float f(float inputx)
        {
            return (Peak - Standard) / ((inputx * AvgSpeed) * (inputx * AvgSpeed) + 1) + Standard;
        }
    }
}
