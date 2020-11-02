using Island.StandardLib.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Math
{
    public class Transform : IStorable
    {
        Vector3 pos, fwd;

        public Vector3 position
        {
            get => pos; set
            {
                pos = value;
                OnChangePosition?.Invoke(pos);
            }
        }

        public Vector3 forward
        {
            get => fwd; set
            {
                fwd = value;
                OnChangeRotation?.Invoke(fwd);
            }
        }

        public Transform() { }

        readonly Action<Vector3> OnChangePosition;
        readonly Action<Vector3> OnChangeRotation;

        public Transform(Vector3 pos, Vector3 fwd, Action<Vector3> posChanged = null, Action<Vector3> rotChanged = null)
        {
            position = pos;
            forward = fwd;
            OnChangePosition = posChanged;
            OnChangeRotation = rotChanged;
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(pos);
            data.Write(fwd);
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out pos);
            data.Read(out fwd);
        }
    }
}
