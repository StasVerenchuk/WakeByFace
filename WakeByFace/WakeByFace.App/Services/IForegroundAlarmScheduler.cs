using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeByFace.App.Services
{
    public interface IForegroundAlarmScheduler
    {
        void Start();
        void Stop();
    }
}
