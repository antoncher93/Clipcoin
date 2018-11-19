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

        Timer timer = new Timer { Interval = 2000, Enabled = true };
        TriggerEventArgs lastEvent;
        bool timer_started = false;

        public TriggerEventHolder()
        {
            timer.Elapsed += (s, e) =>
            {
                OnEvent?.Invoke(this, lastEvent);
                StopTimer();
                lastEvent = null;
            };
        }

        public void HoldEvent(TriggerEventArgs args)
        {
            if(lastEvent!=null && args.Type != lastEvent.Type)
            {
                if(timer_started)
                {
                    StopTimer();
                }
                else
                {
                    lastEvent = args;
                    StartTimer();
                }
                
                
            }
            else
            {
                if(lastEvent == null)
                {
                    lastEvent = args;
                    timer.Start();
                    
                }
            }
        }

        private void StartTimer()
        {
            timer_started = true;
            timer.Start();
        }

        private void StopTimer()
        {
            timer_started = false;
            timer.Stop();
        }
    }
}