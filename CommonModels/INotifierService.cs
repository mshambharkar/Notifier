using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommonModels
{
    [ServiceContract(Namespace = "http://FrustratedItians.Discovery", SessionMode = SessionMode.Required,
                     CallbackContract = typeof(INotifierServiceCallback))]
    public interface INotifierService
    {
        //Join and disconnect
        [OperationContract(IsOneWay = true)]
        void Register(string userId);
        [OperationContract(IsOneWay = true)]
        void DeRegister(string userId);
        //Keep session active
        [OperationContract(IsOneWay = true)]
        void HearBeat();
        //User
        [OperationContract(IsOneWay = false)]
        IList<User> GetUsers();
        [OperationContract(IsOneWay = false)]
        IList<UserDetail> GetUserDetails();
        [OperationContract(IsOneWay = false)]
        IList<Tasks> GetTasks(string userId);
        //Task
        [OperationContract(IsOneWay = true)]
        void UpdateTask(Tasks task);
        [OperationContract(IsOneWay = true)]
        void InsertTask(Tasks task);
        [OperationContract(IsOneWay = true)]
        void DeleteTask(Tasks task);
        //User
        [OperationContract(IsOneWay = true)]
        void UpdateUserAndDetails(User user, UserDetail userDetails);
        [OperationContract(IsOneWay = true)]
        void InsertUserAndDetails(User user, UserDetail userDetails);
        [OperationContract(IsOneWay = true)]
        void DeleteUserAndDetails(User user, UserDetail userDetails);

        [OperationContract(IsOneWay = true)]
        void NotifyTaskCompletion(Tasks task);
    }

    public enum ChnageTypeCallbackEnum
    {
        Add,
        Update,
        Delete
    }

    // The callback interface is used to send messages from service back to client.
    // The Result operation will return the current result after each operation.
    // The Equation opertion will return the complete equation after Clear() is called.
    public interface INotifierServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyTaskCompleted(Tasks task);
        [OperationContract(IsOneWay = true)]
        void NotifyTaskChanges(ChnageTypeCallbackEnum change,Tasks task);
        [OperationContract(IsOneWay = true)]
        void NotifyUserChanges(ChnageTypeCallbackEnum change,User user,UserDetail userdetail);
    }
}
