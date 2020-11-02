using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Math
{
    public static class StaticMath
    {
        static Random random = new Random();

        public static int RandI(int min, int max) => random.Next(min, max);
        public static float RandF(float min, float max) => (min + max) * 0.5f + (float)(random.NextDouble() * 2d - 1d) * (max - min) / 2;

        public static long RandL(long min, long max)
        {
            long Max = max, Min = min;
            if (min > max)
            {
                Max = min;
                Min = max;
            }
            double Key = random.NextDouble();
            long myResult = Min + (long)((Max - Min) * Key);
            return myResult;
        }

        public static float Disturb(this float input, float range_percentage = 0.01f) => input + input * (float)(random.NextDouble() * 2d - 1d) * range_percentage;
        public static int Disturb(this int input, float range_percentage = 0.01f) => random.Next((int)(input * (1f - range_percentage)), (int)(input * (1f + range_percentage)));
        public static long Disturb(this long input, float range_percentage = 0.01f) => RandL((long)(input * (1f - range_percentage)), (long)(input * (1f + range_percentage)));
    }
}
