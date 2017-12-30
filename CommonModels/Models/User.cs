using CommonModels.Models;

namespace Common.Models
{
    public class User : BaseModel
    {
        private RoleEnum _Role;

        public RoleEnum Role
        {
            get { return _Role; }
            set
            {
                _Role = value;
                OnPropertyChanged(() => Role);
            }
        }

        private string _Password;

        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                OnPropertyChanged(() => Password);
            }
        }

    }
}
