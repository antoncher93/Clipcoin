using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Trigger;
using Trigger.Enums;
using Trigger.Signal;

namespace Clipcoin.Application.Show
{
    public class TriggerEventHolder
    {
        public event EventHandler<TriggerEventArgs> OnEvent;

        Timer timer = new Timer { Interval = 2000, Enabled = false };
        TriggerEventArgs lastEvent;
        bool timer_started = false;

        public TriggerEventHolder(int interval)
        {
            timer.Interval = interval;
            timer.Elapsed += (s, e) =>
            {
                OnEvent?.Invoke(this, lastEvent);
                StopTimer();
                lastEvent = null;
            };
        }

        public void HoldEvent(TriggerEventArgs args)
        {
            if(lastEvent == null)
            {
                lastEvent = args;
                timer.Start();
            }
            else
            {
                if(lastEvent.Type == args.Type)
                {

                }
                else
                {
                    lastEvent = null;
                    timer.Stop();
                }
            }
        }

        private void RestartTimer()
        {
            timer.Stop();
            timer.Start();
            timer_started = true;
        }

        private void StopTimer()
        {
            timer_started = false;
            timer.Stop();
        }
    }
}