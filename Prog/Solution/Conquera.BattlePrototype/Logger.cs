using System;
using System.Windows.Controls;

namespace Conquera.BattlePrototype
{
    public static class Logger
    {
        internal class LogEventArgs : EventArgs
        {
            public object Record;
            public ListBoxItem Item;
        }

        public delegate void RecordsDoubleClickHandler(object record);

        internal static event EventHandler<LogEventArgs> Logged;

        public static void Log(object record)
        {
            Log(record, null);
        }

        public static void Log(object record, RecordsDoubleClickHandler onDoubleClick)
        {
            if (Logged != null)
            {
                LogEventArgs args = new LogEventArgs { Record = record };
                Logged(null, args);

                if (onDoubleClick != null)
                {
                    args.Item.MouseDoubleClick += (sender, e) => onDoubleClick(record);
                }
            }
        }
    }
}
