﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Application.Show.Ranger;

namespace Clipcoin.Application.Show
{
    public interface IOnEventListener
    {
        void OnEvent(EventType type);
    }
}