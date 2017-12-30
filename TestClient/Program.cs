using ClientBL;
using CommonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientBL.UDPClient.Instance.StartClient("sa");
            //ServerBL.UDPServer.Instance.StartServer();
            Console.WriteLine("client Started");
            Console.ReadKey();
            Console.WriteLine("Enter key to mark task completed");
            var tasks = ClientBL.UDPClient.Instance.GetAllTasks("sa");
            ClientBL.UDPClient.Instance.NotifyServer(OperationsTypeEnum.TaskComplete, ChnageTypeCallbackEnum.Update, tasks[0]);
            ClientBL.UDPClient.Instance.StopClient();
        }
    }
}
