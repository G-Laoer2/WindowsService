using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;

namespace ServiceTest
{
    public class Listener
    {
        private Thread th;
        private TcpListener tcpl;
        public bool listenerRun = true;
        //listenerRun为true，表示可以接受连接请求，false则为结束程序

        private string path = AppDomain.CurrentDomain.BaseDirectory + "service.log"; //服务日志路径

        /// <summary>
        /// 构造及监听初始化
        /// </summary>
        public Listener()
        {
            th = new Thread(new ThreadStart(Listen));//新建一个用于监听的线程
            th.Start();//打开新线程
        }

        /// <summary>
        /// 监听终止并释放
        /// </summary>
        public void Stop()
        {
            tcpl.Stop();
            th.Abort();//终止线程
        }

        /// <summary>
        /// 端口监听主线程
        /// </summary>
        private void Listen()
        {
            try
            {
                tcpl = new TcpListener(IPAddress.Any,8081);//在8081端口新建一个TcpListener对象
                tcpl.Start();
                Console.WriteLine("started listening..");

                while (listenerRun)//开始监听
                {
                    Socket s = tcpl.AcceptSocket();
                    string remote = s.RemoteEndPoint.ToString();
                    Byte[] stream = new Byte[80];
                    int i = s.Receive(stream);//接受连接请求的字节流
                    string msg = "<" +remote + ">" +System.Text.Encoding.UTF8.GetString(stream);
                    //Console.WriteLine(msg);//在控制台显示字符串
                    var context = "MyWindowsService: Service running " + DateTime.Now + "And" + msg + "\n";//日志打印信息
                    WriteLogs(path, context);
                }
            }
            catch (System.Security.SecurityException)
            {
                var context = "firewall says no no to application – application cries..\n";
                WriteLogs(path, context);
            }
            catch (Exception listenwrong)
            {
                var context = "stoped listening..  " + listenwrong.Message + "\n";
                WriteLogs(path, context);
                Listen();
                //Console.WriteLine("stoped listening..");
            }
        }

        /// <summary>
        /// 日志打印方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
        private void WriteLogs(string path, string context)
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
