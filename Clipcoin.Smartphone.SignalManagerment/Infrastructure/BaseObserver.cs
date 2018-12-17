using System;
using System.Collections.Generic;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Infrastructure
{
    public abstract class BaseObserver<T> : IObserver<T>
    {
        protected IDisposable unsubscriber;

        public virtual void OnCompleted()
        {
            unsubscriber.Dispose();
        }

        public virtual void OnError(Exception error) { }

        public virtual void OnNext(T value) { }

        public void Subscribe(IObservable<T> provider)
        {
            unsubscriber = provider.Subscribe(this);
        }
    }
}
