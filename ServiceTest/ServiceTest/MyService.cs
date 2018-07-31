using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTest
{
    public partial class MyService : ServiceBase
    {
        public MyService()
        {
            InitializeComponent();            
        }

        private Listener listener;

        protected override void OnStart(string[] args)
        {
            //Task.Factory.StartNew(Handle);
            Task.Factory.StartNew(Monitoring);
        }

        protected override void OnStop()
        {
            listener.Stop();
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("D:\\ServiceLog.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + " Mornitoring Stop And Service Stop");
            }
        }

        /// <summary>
        /// TCP端口监听实例化线程
        /// </summary>
        private void Monitoring()
        {
            try
            {
                listener = new Listener();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 服务测试Log打印线程
        /// </summary>
        private void Handle()
        {
            while (true)
            {
                try
                {
                    var path = AppDomain.CurrentDomain.BaseDirectory + "service.log";
                    var context = "MyWindowsService: Service Stoped " + DateTime.Now + "\n";
                    WriteLogs(path, context);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 服务测试Log打印方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
        public void WriteLogs(string path, string context)
        {
            var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            var sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(context);

            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
