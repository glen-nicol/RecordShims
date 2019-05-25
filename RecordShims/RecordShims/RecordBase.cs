using System;
using System.Collections.Generic;
using System.Text;

namespace RecordShims
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RecordBase<T> : IRecord<T> where T : class
    {
        protected T RecordType { get { return this as T; } }

        T IRecord<T>.ShallowCopy()
        {
            return RecordUtil.MemberwiseClone(RecordType);
        }

        public abstract void ThrowIfConstraintsAreViolated(T record);
    }

    public class Record : RecordBase<Record>
    {
        public override void ThrowIfConstraintsAreViolated(Record record)
        {
            throw new NotImplementedException();
        }
    }
}
