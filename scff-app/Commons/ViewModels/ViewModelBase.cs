using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace ScffApp.Commons.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged メンバー

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void NotifyPropertyChanged(string propertyName)
        {
            Contract.Requires(GetType().GetProperty(propertyName) != null);

            if (PropertyChanged == null)
            {
                return;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}