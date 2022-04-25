using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostService.Classes; 
internal class GrpcService {
    private readonly int _port;
    internal GrpcService(int port) => _port = port;

    internal void StartService() {
        var server = new Server() {
            Ports = {
                new ServerPort("0.0.0.0", _port, ServerCredentials.Insecure)
            }
        };
    }

    internal void StopService() {

    }
}
