using jCommunicator;
using mLogger;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace jCommunicator.Tests
{
    public class CommunicatorTestBase : IDisposable
    {
        // --- Configuration ---
        protected readonly string _hubHost = "Hub1.local";  // Change to your actual Hub IP
        protected readonly string _hubUser = "camcpp";      // Change to your actual Hub User
        protected readonly string _hubPass = "cam";         // Change to your actual Hub _hubPass
        protected readonly string _node1Host = "10.0.0.11"; // Change to your actual Hub IP
        protected readonly string _node1User = "camcpp";    // Change to your actual Hub User
        protected readonly string _node1Pass = "cam";       // Change to your actual Hub _hubPass
        protected readonly string _node2Host = "10.0.0.12"; // Change to your actual Hub IP
        protected readonly string _node2User = "camcpp";    // Change to your actual Hub User
        protected readonly string _node2Pass = "cam";       // Change to your actual Hub _hubPass

        // --- State ---
        protected Communicator? _communicator;
        public Logger logger = Logger.Instance;

        public CommunicatorTestBase() 
        {
            logger.Initialize("CommunicatorTestBase");

            _communicator = new Communicator(_hubHost, _hubUser, _hubPass);
        }
        ~CommunicatorTestBase()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (_communicator != null)
                _communicator.Disconnect();
        }

        protected string CreateHubFile(Communicator com, string path, string contents = "Contents")
        {
            return com.ExecuteHubCommand($"echo '{contents}' > {path}");
        }
        protected string CreateNodeFile(Communicator com, string host, string username, string path, string contents = "Contents")
        {
            return com.ExecuteNodeCommand($"echo '{contents}' > {path}", host, username);
        }
    }
}
