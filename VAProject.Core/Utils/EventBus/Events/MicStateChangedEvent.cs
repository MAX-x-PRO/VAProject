using System;
using System.Collections.Generic;
using System.Text;

namespace VAProject.Core.Utils.EventBus.Events
{
    public class MicStateChangedEvent
    {
        public MicStates State { get; set; }
    }
}
