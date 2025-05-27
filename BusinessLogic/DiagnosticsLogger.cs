using System.Collections.Concurrent;


namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class DiagnosticsLogger : DiagnosticsLoggerAbstractAPI
    {
        private readonly BlockingCollection<string> _logQueue = new(1000);
        private readonly Thread _logThread;
        private readonly StreamWriter _writer;
        private bool _disposed = false;

        public DiagnosticsLogger(string filePath)
        {
            _writer = new StreamWriter(filePath, append: true, encoding: System.Text.Encoding.ASCII);
            _logThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true
            };
            _logThread.Start();
        }

        public override void Log(String data)
        {
            data = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFFF")}] {data}";
            try
            {
                if (!_logQueue.TryAdd(data, 5))
                {
                    _logQueue.TryTake(out _);
                    _logQueue.TryAdd(data);
                }
            }
            catch { }
        }

        private void ProcessLogQueue()
        {
            foreach (var entry in _logQueue.GetConsumingEnumerable())
            {
                try
                {
                    _writer.WriteLine(entry);
                    _writer.Flush();
                }
                catch (IOException)
                {
                    Thread.Sleep(50);
                }
            }
        }

        public override void Dispose()
        {
            _disposed = true;
            _logQueue.CompleteAdding();
            _logThread.Join();
            _writer.Dispose();
        }
    }
}
