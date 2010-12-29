using System;
using System.Collections.Generic;
using System.Text;

namespace Conquera
{
    public static class EventHelper
    {
        public static void RaiseEvent(EventHandler handler, object sender)
        {
            RaiseEvent(handler, sender, EventArgs.Empty);
        }

        public static void RaiseEvent(EventHandler handler, object sender, EventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
