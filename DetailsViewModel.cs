using System;

using System.ComponentModel; //for INotifyPropertyChanged

namespace XpressReceipt
{
    public class DetailsViewModel : INotifyPropertyChanged
    {
        string senderEmail, receiverEmail;

		public string SenderEmail
		{
			get
			{
				return senderEmail;
			}
			set
			{
				if (senderEmail != value)
				{
					senderEmail = value;
					OnPropertyChanged("SenderEmail");
				}
			}
		}

		public string ReceiverEmail
		{
			get
			{
				return receiverEmail;
			}
			set
			{
				if (receiverEmail != value)
				{
					receiverEmail = value;
					OnPropertyChanged("ReceiverEmail");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			var changed = PropertyChanged;
			if (changed != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
    }
}
