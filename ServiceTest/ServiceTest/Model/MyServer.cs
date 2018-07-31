using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTTPServerLib;
using System.Net;

namespace ServiceTest.Model
{
    public class MyServer : HttpServer
    {
        public MyServer(string IPAddress, int Port) : base(IPAddress, Port)
        {

        }
    }
}
