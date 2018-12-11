using System;
using System.Collections.Generic;

namespace Clipcoin.Smartphone.SignalManagement.Infrastructure
{
    public abstract class BaseObservable<T> : IObservable<T>
    {
        protected ICollection<IObserver<T>> _observers = new List<IObserver<T>>();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Subscriber(_observers, observer);
        }

        protected void NotifyAll(T args)
        {
            foreach(var obs in _observers)
            {
                obs.OnNext(args);
            }
        }

        private class Subscriber : IDisposable
        {
            private ICollection<IObserver<T>> _observers;
            private IObserver<T> _observer;

            public Subscriber(ICollection<IObserver<T>> observers, IObserver<T> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}
