using CommonModels.Models;
using System;

namespace Common.ViewModel
{
    public class CommandViewModel
    {
        public CommandViewModel(Action addCommand, Action editCommand, Action deleteCommand)
        {
            _AddCommand = new DelegateCommand(addCommand);
            _DeleteCommand = new DelegateCommand(deleteCommand);
            _EditCommand = new DelegateCommand(editCommand);
        }

        public CommandViewModel(DelegateCommand addCommand, DelegateCommand editCommand, DelegateCommand deleteCommand)
        {
            _AddCommand = addCommand;
            _DeleteCommand = deleteCommand;
            _EditCommand = editCommand;
        }
        private DelegateCommand _AddCommand;
        public DelegateCommand AddCommand
        {
            get
            {
                return _AddCommand;
            }
        }

        private DelegateCommand _DeleteCommand;
        public DelegateCommand DeleteCommand
        {
            get
            {
                return _DeleteCommand;
            }
        }



        private DelegateCommand _EditCommand;
        public DelegateCommand EditCommand
        {
            get
            {
                return _EditCommand;
            }
        }
    }
}
