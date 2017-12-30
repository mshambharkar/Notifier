using CommonModels.Models;

namespace Common.Models
{
    public class UserDetail : BaseModel
    {

        private string _FirstName;

        public string FirstName
        {
            get { return _FirstName; }
            set
            {
                _FirstName = value;
                OnPropertyChanged(() => FirstName);
            }
        }

        private string _LastName;

        public string LastName
        {
            get { return _LastName; }
            set
            {
                _LastName = value;
                OnPropertyChanged(() => LastName);
            }
        }

        private string _SecurityQuestion1;

        public string SecurityQuestion1
        {
            get { return _SecurityQuestion1; }
            set
            {
                _SecurityQuestion1 = value;
                OnPropertyChanged(() => SecurityQuestion1);
            }
        }

        private string _SecurityAnswer1;

        public string SecurityAnswer1
        {
            get { return _SecurityAnswer1; }
            set
            {
                _SecurityAnswer1 = value;
                OnPropertyChanged(() => SecurityAnswer1);
            }
        }




        private string _SecurityQuestion2;

        public string SecurityQuestion2
        {
            get { return _SecurityQuestion2; }
            set
            {
                _SecurityQuestion2 = value;
                OnPropertyChanged(() => SecurityQuestion2);
            }
        }

        private string _SecurityAnswer2;

        public string SecurityAnswer2
        {
            get { return _SecurityAnswer2; }
            set
            {
                _SecurityAnswer2 = value;
                OnPropertyChanged(() => SecurityAnswer2);
            }
        }

        public bool DefaultId
        {
            get;
            set;
        }
    }
}