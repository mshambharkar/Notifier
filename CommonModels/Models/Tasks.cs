using CommonModels.Models;
using System;

namespace Common.Models
{
    public class Tasks : BaseModel
    {

        private string _CreatorId;

        public string CreatorId
        {
            get { return _CreatorId; }
            set
            {
                _CreatorId = value;
                OnPropertyChanged(() => CreatorId);
            }
        }

        private string _AssignedToId;

        public string AssignedToId
        {
            get { return _AssignedToId; }
            set
            {
                _AssignedToId = value;
                OnPropertyChanged(() => AssignedToId);
            }
        }


        private DateTime _CreationDateTime;

        public DateTime CreationDateTime
        {
            get { return _CreationDateTime; }
            set
            {
                _CreationDateTime = value;
                OnPropertyChanged(() => CreationDateTime);
            }
        }

        private DateTime _EndDateTime;

        public DateTime EndDateTime
        {
            get { return _EndDateTime; }
            set
            {
                _EndDateTime = value;
                OnPropertyChanged(() => EndDateTime);
            }
        }


        private bool _NotifyCompletion;

        public bool NotifyCompletion
        {
            get { return _NotifyCompletion; }
            set
            {
                _NotifyCompletion = value;
                OnPropertyChanged(() => NotifyCompletion);
            }
        }
        private bool _IsCompleted;

        public bool IsCompleted
        {
            get { return _IsCompleted; }
            set
            {
                _IsCompleted = value;
                OnPropertyChanged(() => IsCompleted);
            }
        }

        private string _SentCompletion;

        public string SentCompletion
        {
            get { return _SentCompletion; }
            set
            {
                _SentCompletion = value;
                OnPropertyChanged(() => SentCompletion);
            }
        }

        private string _TaskDescription;

        public string TaskDescription
        {
            get { return _TaskDescription; }
            set
            {
                _TaskDescription = value;
                OnPropertyChanged(() => TaskDescription);
            }
        }

    }
}