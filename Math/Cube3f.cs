using Island.StandardLib.Storage;
using System;

namespace Island.StandardLib.Math
{
    public struct Cube3f : IStorable
    {
        float _XStart, _XEnd, _YStart, _YEnd, _ZStart, _ZEnd;

        public float XStart
        {
            get
            {
                return _XStart;
            }
            set
            {
                _XStart = value;
                if (_XEnd < _XStart)
                {
                    float f = _XEnd;
                    _XEnd = _XStart;
                    _XStart = f;
                }
            }
        }

        public float YStart
        {
            get
            {
                return _YStart;
            }
            set
            {
                _YStart = value;
                if (_YEnd < _YStart)
                {
                    float f = _YEnd;
                    _YEnd = _YStart;
                    _YStart = f;
                }
            }
        }

        public float ZStart
        {
            get
            {
                return _ZStart;
            }
            set
            {
                _ZStart = value;
                if (_ZEnd < _ZStart)
                {
                    float f = _ZEnd;
                    _ZEnd = _ZStart;
                    _ZStart = f;
                }
            }
        }

        public float XEnd
        {
            get
            {
                return _XEnd;
            }
            set
            {
                _XEnd = value;
                if (_XEnd < _XStart)
                {
                    float f = _XEnd;
                    _XEnd = _XStart;
                    _XStart = f;
                }
            }
        }

        public float YEnd
        {
            get
            {
                return _YEnd;
            }
            set
            {
                _YEnd = value;
                if (_YEnd < _YStart)
                {
                    float f = _YEnd;
                    _YEnd = _YStart;
                    _YStart = f;
                }
            }
        }

        public float ZEnd
        {
            get
            {
                return _ZEnd;
            }
            set
            {
                _ZEnd = value;
                if (_ZEnd < _ZStart)
                {
                    float f = _ZEnd;
                    _ZEnd = _ZStart;
                    _ZStart = f;
                }
            }
        }

        /// <summary>
        /// 以完整格式初始化<see cref="Cube3f"/>
        /// </summary>
        /// <param name="xStart">X起始点</param>
        /// <param name="xEnd">X终点</param>
        /// <param name="yStart">Y起始点</param>
        /// <param name="yEnd">Y终点</param>
        /// <param name="zStart">Z起始点</param>
        /// <param name="zEnd">Z终点</param>
        public Cube3f(float xStart, float xEnd, float yStart, float yEnd, float zStart, float zEnd)
        {
            _XStart = xStart;
            _XEnd = xEnd;
            _YStart = yStart;
            _YEnd = yEnd;
            _ZStart = zStart;
            _ZEnd = zEnd;
            Trim();
        }

        /// <summary>
        /// 以三边长度初始化<see cref="Cube3f"/>，并将起始点设为<see cref="Vector3.Zero"/>
        /// </summary>
        /// <param name="xlen">X长度</param>
        /// <param name="ylen">Y长度</param>
        /// <param name="zlen">Z长度</param>
        public Cube3f(float xlen, float ylen, float zlen)
        {
            _XStart = _YStart = _ZStart = 0f;
            _XEnd = xlen;
            _YEnd = ylen;
            _ZEnd = zlen;
        }

        /// <summary>
        /// 以三边长度初始化<see cref="Cube3f"/>，并将起始点设为<see cref="Vector3.Zero"/>
        /// </summary>
        /// <param name="size">三边长度</param>
        public Cube3f(Vector3 size)
        {
            _XStart = _YStart = _ZStart = 0f;
            _XEnd = size.X;
            _YEnd = size.Y;
            _ZEnd = size.Z;
        }

        void Trim()
        {
            if (_XEnd < _XStart)
            {
                float f = _XEnd;
                _XEnd = _XStart;
                _XStart = f;
            }
            if (_YEnd < _YStart)
            {
                float f = _YEnd;
                _YEnd = _YStart;
                _YStart = f;
            }
            if (_ZEnd < _ZStart)
            {
                float f = _ZEnd;
                _ZEnd = _ZStart;
                _ZStart = f;
            }
        }

        /// <summary>
        /// 获取三边长度
        /// </summary>
        public Vector3 Length
        {
            get
            {
                return new Vector3(_XEnd - _XStart, _YEnd - _YStart, _ZEnd - _ZStart);
            }
        }

        /// <summary>
        /// 获取或设置起始点
        /// </summary>
        public Vector3 Start
        {
            get
            {
                return new Vector3(_XStart, _YStart, _ZStart);
            }
            set
            {
                Vector3 delta = value.RED(Start);
                _XEnd += delta.X;
                _YEnd += delta.Y;
                _ZEnd += delta.Z;
                _XStart = value.X;
                _YStart = value.Y;
                _ZStart = value.Z;
            }
        }

        /// <summary>
        /// 获取和设置终点
        /// </summary>
        public Vector3 End
        {
            get
            {
                return new Vector3(_XEnd, _YEnd, _ZEnd);
            }
            set
            {
                Vector3 delta = value.RED(End);
                _XStart += delta.X;
                _YStart += delta.Y;
                _ZStart += delta.Z;
                _XEnd = value.X;
                _YEnd = value.Y;
                _ZEnd = value.Z;
            }
        }

        /// <summary>
        /// 获取和设置底面中心点
        /// </summary>
        public Vector3 BottomCenter
        {
            get
            {
                Vector3 hfsize = Length.MUL(0.5f);
                return new Vector3(_XStart + hfsize.X, _YStart, _ZStart + hfsize.Z);
            }
            set
            {
                Vector3 hfsize = Length.MUL(0.5f);
                hfsize.Y = 0;
                Start = value.RED(hfsize);
            }
        }

        /// <summary>
        /// 测试此<see cref="Cube3f"/>是否包含指定的点
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool ContainPoint(Vector3 pt)
        {
            return pt.X >= _XStart && pt.X <= _XEnd && pt.Y >= _YStart && pt.Y <= _YEnd && pt.Z >= _ZStart && pt.Z <= _ZEnd;
        }

        /// <summary>
        /// 测试此<see cref="Cube3f"/>是否与指定的<see cref="Cube3f"/>相交
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        public bool ContainCube(Cube3f cb)
        {
            if (cb.XStart > XEnd)
                return false;
            if (cb.XEnd < XStart)
                return false;
            if (cb.YStart > YEnd)
                return false;
            if (cb.YEnd < YStart)
                return false;
            if (cb.ZStart > ZEnd)
                return false;
            if (cb.ZEnd < ZStart)
                return false;
            return true;
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(_XStart);
            data.Write(_XEnd);
            data.Write(_YStart);
            data.Write(_YEnd);
            data.Write(_ZStart);
            data.Write(_ZEnd);
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out _XStart);
            data.Read(out _XEnd);
            data.Read(out _YStart);
            data.Read(out _YEnd);
            data.Read(out _ZStart);
            data.Read(out _ZEnd);
        }

        public static Cube3f operator +(Cube3f cube, Vector3 vec)
        {
            return new Cube3f(
                cube.XStart + vec.X, cube.XEnd + vec.X,
                cube.YStart + vec.Y, cube.YEnd + vec.Y,
                cube.ZStart + vec.Z, cube.ZEnd + vec.Z);
        }

        public static Cube3f operator -(Cube3f cube, Vector3 vec)
        {
            return new Cube3f(
                cube.XStart - vec.X, cube.XEnd - vec.X,
                cube.YStart - vec.Y, cube.YEnd - vec.Y,
                cube.ZStart - vec.Z, cube.ZEnd - vec.Z);
        }
    }
}
