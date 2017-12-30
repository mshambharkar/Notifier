using CommonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;

namespace ServerBL
{
    public class UDPServer
    {
        ServiceHost serviceHost;
        private static UDPServer _Instance;
        public static UDPServer Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new UDPServer();
                return _Instance;
            }
        }

        private UDPServer()
        {
        }
        public void StopServer()
        {
            try
            {
                if (serviceHost == null)
                    return;

                serviceHost.Close();
                if (serviceHost.State != CommunicationState.Closed)
                {
                    serviceHost.Abort();
                }
            }
            catch (Exception)
            {
            }
        }
        public void StartServer()
        {
            EndpointAddress endpointAddress = FindNotifierServiceAddress();
            if (endpointAddress != null)
            {
                return;//Someone Else has already started the server
            }

            Uri baseAddress = new Uri(String.Format("net.tcp://{0}:8001/TaskAssign/", Utils.GetIpV4()));
            serviceHost = new ServiceHost(typeof(NotifierService), baseAddress);
            try
            {
                // Add an endpoint to the service
                ServiceEndpoint discoverableCalculatorEndpoint = serviceHost.AddServiceEndpoint(
                    typeof(INotifierService),
                    new NetTcpBinding(),
                    "/DiscoverableEndpoint");

                // Add a Scope to the endpoint
                EndpointDiscoveryBehavior discoverableEndpointBehavior = new EndpointDiscoveryBehavior();
                discoverableEndpointBehavior.Scopes.Add(new Uri("ldap:///ou=frustratedItian,o=frustratedItian,c=IN"));
                discoverableCalculatorEndpoint.Behaviors.Add(discoverableEndpointBehavior);

                // Add an endpoint to the service
                ServiceEndpoint nonDiscoverableCalculatorEndpoint = serviceHost.AddServiceEndpoint
                    (typeof(INotifierService),
                    new NetTcpBinding(),
                    "/NonDiscoverableEndpoint");

                // Disable discoverability of the endpoint
                EndpointDiscoveryBehavior nonDiscoverableEndpointBehavior = new EndpointDiscoveryBehavior();
                nonDiscoverableEndpointBehavior.Scopes.Add(new Uri("ldap:///ou=frustratedItian,o=frustratedItian,c=IN"));
                nonDiscoverableEndpointBehavior.Enabled = false;
                nonDiscoverableCalculatorEndpoint.Behaviors.Add(nonDiscoverableEndpointBehavior);

                // Make the service discoverable over UDP multicast
                serviceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                serviceHost.Open();
            }
            catch (CommunicationException e)
            {
                throw e;
            }
            catch (TimeoutException e)
            {
                throw e;
            }
        }
        private EndpointAddress FindNotifierServiceAddress()
        {
            // Create DiscoveryClient
            DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());

            // Find ICalculatorService endpoints in the specified scope            
            Uri scope = new Uri("ldap:///ou=frustratedItian,o=frustratedItian,c=IN");
            FindCriteria findCriteria = new FindCriteria(typeof(INotifierService));
            findCriteria.Scopes.Add(scope);
            findCriteria.MaxResults = 1;
            FindResponse findResponse = discoveryClient.Find(findCriteria);
            if (findResponse.Endpoints.Count > 0)
            {
                return findResponse.Endpoints[0].Address;
            }
            else
            {
                return null;
            }
        }
    }
}
