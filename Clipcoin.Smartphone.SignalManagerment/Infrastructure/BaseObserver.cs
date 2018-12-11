using System;
using System.Collections.Generic;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Infrastructure
{
    public abstract class BaseObserver<T> : IObserver<T>
    {
        public virtual void OnCompleted()
        {
            
        }

        public virtual void OnError(Exception error)
        {
            
        }

        public virtual void OnNext(T value)
        {
            
        }
    }
}
