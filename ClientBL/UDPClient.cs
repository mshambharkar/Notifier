using CommonModels;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using Common.Models;
using DataLayer;
using System.Timers;
using System.Threading.Tasks;

namespace ClientBL
{
    public enum OperationsTypeEnum
    {
        Task,
        User,
        TaskComplete
    }

    public class UDPClient
    {

        private Timer timer;

        DBOperations db = new DBOperations();
        private static UDPClient _Instance;
        public static UDPClient Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new UDPClient();
                return _Instance;
            }
        }

        public void GetAllTasks()
        {
            var lstTask = client.GetTasks(_UserId);
            if (lstTask != null)
            {
                db.BulkInsertOrUpdate<Tasks>(lstTask);
            }
        }

        readonly CallbackHandler handler = new CallbackHandler();
        public CallbackHandler CallBack
        {
            get
            {
                return handler;
            }
        }
        private DuplexChannelFactory<INotifierService> factory;
        private INotifierService client;
        private string _UserId;
        private UDPClient()
        {
            timer = new Timer(60000);//1 min 
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (factory != null && factory.State == CommunicationState.Opened)
                client.HearBeat();
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
        public void FetchAllUsers()
        {
            EndpointAddress endpointAddress = FindNotifierServiceAddress();
            if (endpointAddress != null)
            {
                try
                {
                    InstanceContext instanceContext = new InstanceContext(handler);
                    NetTcpBinding binding = new NetTcpBinding();
                    factory = new DuplexChannelFactory<INotifierService>(instanceContext, binding, endpointAddress);
                    // Create a client
                    client = factory.CreateChannel();
                    var users = client.GetUsers();
                    db.BulkInsertOrUpdate<User>(users);
                    var userDetails = client.GetUserDetails();
                    db.BulkInsertOrUpdate<UserDetail>(userDetails);
                    factory.Close();
                }
                catch (Exception ex)
                {
                    // throw new Exception("Unable to connect to server");
                }
            }
        }
        public void StartClient(string userId)
        {
            this._UserId = userId;
            EndpointAddress endpointAddress = FindNotifierServiceAddress();

            if (endpointAddress != null)
            {
                InvokeService(endpointAddress);
                timer.Start();
            }
            else
            {
                throw new Exception("No Endpoind Found");
            }
        }

        public void SyncUserDetails()
        {
            try
            {
                if (client != null && factory != null && factory.State == CommunicationState.Opened)
                {
                    var users = client.GetUsers();
                    db.BulkInsertOrUpdate<User>(users);
                    var userDetails = client.GetUserDetails();
                    db.BulkInsertOrUpdate<UserDetail>(userDetails);
                }
            }
            catch (Exception ex)
            {
            }

        }

        public void LogoffClient(string userId)
        {
            try
            {
                if (factory != null && factory.State == CommunicationState.Opened)
                    client.DeRegister(userId);
            }
            catch (Exception)
            {

            }
            finally
            {
                timer.Stop();
            }
        }

        public void StopClient()
        {
            try
            {

                if (factory != null && factory.State == CommunicationState.Closing)

                    //Closing the client gracefully closes the connection and cleans up resources
                    factory.Abort();
            }
            catch (Exception)
            {

            }
            finally
            {
                timer.Stop();
            }
        }

        public void SyncTask(string userId)
        {
            try
            {
                if (client != null && factory != null && factory.State == CommunicationState.Opened)
                {
                    var lstTask = client.GetTasks(_UserId);
                    if (lstTask != null)
                    {
                        db.BulkInsertOrUpdate<Tasks>(lstTask);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void InvokeService(EndpointAddress endpointAddress)
        {
            InstanceContext instanceContext = new InstanceContext(handler);
            NetTcpBinding binding = new NetTcpBinding();
            factory = new DuplexChannelFactory<INotifierService>(instanceContext, binding, endpointAddress);

            // Create a client
            client = factory.CreateChannel();

            client.Register(_UserId);
            Task.Factory.StartNew(LoadUserAndTask);
        }

        private void LoadUserAndTask()
        {

        }

        public void NotifyServer(OperationsTypeEnum operation, ChnageTypeCallbackEnum change, params object[] objects)
        {
            switch (operation)
            {
                case OperationsTypeEnum.Task:
                    switch (change)
                    {
                        case ChnageTypeCallbackEnum.Add:
                            client.InsertTask(objects[0] as Tasks);
                            break;
                        case ChnageTypeCallbackEnum.Update:
                            client.UpdateTask(objects[0] as Tasks);
                            break;
                        case ChnageTypeCallbackEnum.Delete:
                            client.DeleteTask(objects[0] as Tasks);
                            break;
                        default:
                            break;
                    }
                    break;
                case OperationsTypeEnum.User:
                    User user = objects[0] as User;
                    UserDetail userDetail = objects[1] as UserDetail;
                    switch (change)
                    {
                        case ChnageTypeCallbackEnum.Add:
                            client.InsertUserAndDetails(user, userDetail);
                            break;
                        case ChnageTypeCallbackEnum.Update:
                            client.UpdateUserAndDetails(user, userDetail);
                            break;
                        case ChnageTypeCallbackEnum.Delete:
                            client.DeleteUserAndDetails(user, userDetail);
                            break;
                        default:
                            break;
                    }
                    break;
                case OperationsTypeEnum.TaskComplete:
                    client.NotifyTaskCompletion(objects[0] as Tasks);
                    break;
                default:
                    break;
            }
        }


    }

    public class CallbackHandler : INotifierServiceCallback
    {
        public delegate void NotifyDelegate(OperationsTypeEnum operation, ChnageTypeCallbackEnum change, object obj);
        public event NotifyDelegate NotifyHandler;


        public void NotifyTaskChanges(ChnageTypeCallbackEnum change, Tasks task)
        {
            switch (change)
            {
                case ChnageTypeCallbackEnum.Add:
                    break;
                case ChnageTypeCallbackEnum.Update:
                    break;
                case ChnageTypeCallbackEnum.Delete:
                    break;
                default:
                    break;
            }
            NotifyHandler?.Invoke(OperationsTypeEnum.Task, change, task);
        }

        public void NotifyUserChanges(ChnageTypeCallbackEnum change, User user, UserDetail details)
        {
            switch (change)
            {
                case ChnageTypeCallbackEnum.Add:
                    break;
                case ChnageTypeCallbackEnum.Update:
                    break;
                case ChnageTypeCallbackEnum.Delete:
                    break;
                default:
                    break;
            }
            NotifyHandler?.Invoke(OperationsTypeEnum.User, change, user);
        }

        public void NotifyTaskCompleted(Tasks task)
        {
            NotifyHandler?.Invoke(OperationsTypeEnum.TaskComplete, ChnageTypeCallbackEnum.Update, task);
        }
    }
}
