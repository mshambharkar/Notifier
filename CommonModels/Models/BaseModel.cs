using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace CommonModels.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        private string _Id;

        public string Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                OnPropertyChanged(() => Id);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged<T>(Expression<Func<T>> memberExpression)
        {
            var member = (MemberExpression)memberExpression.Body;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(member.Member.Name));
        }
    }
}
