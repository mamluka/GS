using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GemScopeWPF.UI;
namespace GemScopeWPF.UI
{
    public interface IObserverCaptureFlow<T>
    {
        void CameraStateChange(T value);
    }
}
