using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Clipcoin.Application.Models
{
    public abstract class BaseObservable<T> : IObservable<T>
    {
        private class Subscriber : IDisposable
        {
            private IObserver<T> _observer;
            private ICollection<IObserver<T>> _observers;

            public Subscriber(ICollection<IObserver<T>> observers, IObserver<T> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if(_observer!= null && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }

        private ICollection<IObserver<T>> _observers = new List<IObserver<T>>();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if(!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            return new Subscriber(_observers, observer);
        }
    }
}