using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMBase.MVVM.Model
{
    public class LogItem
    {
        public LogItem(string message)
        {
            TimeStamp = DateTime.Now;
            Message = message;
        }
        public DateTime TimeStamp { get; private set; }
        public string Message { get; private set; }
        public override string ToString()
        {
            return $"{TimeStamp}:{Message}";
        }
    }
    public class Log
    {
        static string LogPath = "Logs";
        private static readonly Lazy<Log> lazy = new Lazy<Log>(() => new Log());
        private ObservableCollection<LogItem> logItems;
        object locker = new object();
        public static Log GetInstance()
        {
            return lazy.Value;
        }
        private Log()
        {
            logItems = new ObservableCollection<LogItem>();
            ((INotifyCollectionChanged)logItems).CollectionChanged += (s, a) =>
            {
                if (a.NewItems?.Count >= 1)
                {
                    foreach (LogItem item in a.NewItems)
                        appendLog(item);
                }
            };
        }
        void appendLog(LogItem item)
        {
            lock (locker)
            {
                if (!Directory.Exists(LogPath))
                    Directory.CreateDirectory(LogPath);

                string fileName = $"{LogPath}\\{DateTime.Now.ToString("yyyyMMdd")}.txt";
                if (!File.Exists(fileName))
                    File.Create(fileName);

                File.AppendAllText(fileName, $"{item.ToString()}{Environment.NewLine}");
            }
        }
        public static void logThis(LogItem item)
        {
            GetInstance().logItems.Add(item);
        }
        public static void logThis(string msg)
        {
            logThis(new LogItem(msg));
        }
    }
    public class OutputLog
    {
        private static OutputLog instance;
        private static readonly Lazy<OutputLog> lazy = new Lazy<OutputLog>(() => new OutputLog());

        public static OutputLog GetInstance()
        {
            return lazy.Value;
            //if(instance == null)
            //    instance = new OutputLog();
            //return instance;
        }
        private OutputLog()
        {
            logItems = new ObservableCollection<LogItem>();
            LogItems = new ReadOnlyObservableCollection<LogItem>(logItems);
        }

        private ObservableCollection<LogItem> logItems;
        public ReadOnlyObservableCollection<LogItem> LogItems { get; private set; }
        public static void That(string message)
        {
            LogItem item = new LogItem(message);
            GetInstance().logItems.Add(item);
            try
            {
                Log.logThis(item);
            }
            catch (Exception ex)
            {
                GetInstance().logItems.Add(new LogItem($"Не удалась запись в лог файл: {ex.Message}"));
            }
        }
        public static void DoNothing()
        {

        }
    }
}
