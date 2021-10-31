using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Console_log_test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var sw1 = new Stopwatch();
            //var sw2 = new Stopwatch();

            //sw1.Start();
            //for (int i = 0; i < 1000; i++)
            //{
            //    //Thread.Sleep(1000);
            //    var logobj = new logObj { time = DateTime.Now, Status = "NORMAL", message = $"count:{i}" };
            //    var sb = new StringBuilder();
            //    sb.Append(logobj.time.ToString("yy/MM/dd hh:mm:ss"));
            //    sb.Append("[");
            //    sb.Append(logobj.Status);
            //    sb.Append("]");
            //    sb.Append(logobj.message);
            //    Console.WriteLine(sb.ToString());
            //}
            //sw1.Stop();

            //sw2.Start();
            //var log = new logger(Console.Out);
            //log.StartOutput();
            //for (int i = 0; i < 1000; i++)
            //{
            //    //Thread.Sleep(1000);
            //    var logobj = new logObj { time = DateTime.Now, Status = "NORMAL", message = $"count:{i}" };
            //    log.log(logobj);
            //}
            //log.Dispose();
            //sw2.Stop();
            //Console.WriteLine($"time:{sw1.ElapsedMilliseconds}");
            //Console.WriteLine($"time:{sw2.ElapsedMilliseconds}");

            
            for (int i = 1; i <= 100; i++)
            {
                Thread.Sleep(20);
                var logobj = new logObj { time = DateTime.Now, Status = "NORMAL", message = $"count:{i}" };
                var sb = new StringBuilder();
                sb.Append("\r");
                sb.Append(logobj.time.ToString("yy/MM/dd hh:mm:ss"));
                sb.Append("[");
                sb.Append(logobj.Status);
                sb.Append("]");
                sb.Append("[");
                var coo = Console.BufferWidth - sb.Length - 5;
                var length = coo * ((float)i / 100f);
                for (int j = 0; j <= coo; j++)
                {
                    if (j < (int)Math.Ceiling(length))
                    {
                        sb.Append("=");
                        continue;
                    }
                    if (j == (int)Math.Ceiling(length))
                    {
                        sb.Append(">");
                        continue;
                    }
                    sb.Append(" ");
                }
                sb.Append("]");
                sb.Append($"{i}%");


                Console.Write(sb.ToString());
            }

        }
    }

    struct logObj
    {
        public DateTime time;
        public string Status;
        public string message;
    }
    class logger : IDisposable
    {
        static Queue<logObj> logs;
        static Task task;
        static CancellationTokenSource cts = new CancellationTokenSource();
        static CancellationToken Token = cts.Token;
        static System.IO.TextWriter stream;
        public logger(System.IO.TextWriter _stream)
        {
            stream = _stream;
            logs = new Queue<logObj>();
            task = new Task(() => { loggerProc(Token); });//Task.Factory(() => { loggerProc(Token); });
        }
        public void Dispose()
        {
            while (logs.Count > 0) ;
            StopOutput();
            while (!task.IsCompleted) ;
            task.Dispose();
            return;
        }

        public void log(object msg)
        {

        }

        public void log(logObj log)
        {
            logs.Enqueue(log);
        }

        public void StartOutput()
        {
            task.Start();
        }

        public void StopOutput()
        {
            cts.Cancel();
        }

        private void loggerProc(CancellationToken token)
        {
            while (true)
            {
                if (logs.Count <= 0) continue;
                var log = logs.Dequeue();
                var sb = new StringBuilder();
                sb.Append(log.time.ToString("yy/MM/dd hh:mm:ss"));
                sb.Append("[");
                sb.Append(log.Status);
                sb.Append("]");
                sb.Append(log.message);
                stream.WriteLine(sb.ToString());
                if (token.IsCancellationRequested) return;



            }

        }

    }
}
