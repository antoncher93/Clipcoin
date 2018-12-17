using System;
using System.Collections.Generic;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Interfaces
{
    public interface ISignalSendingCallback
    {
        void OnOK();
        void OnFail(string msg);

    }
}
