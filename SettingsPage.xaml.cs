using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XpressReceipt
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
        String ReceiverDefaultEmailString;
        String SenderDefaultEmailString;

        String senderEmail;


        /*
         * The settings page allows the user to input their
         * e-mail address and have it be saved as the default
         * sender's e-mail address.
         * They can also 
         */
		public SettingsPage ()
		{
			InitializeComponent ();
            BindingContext = new DetailsViewModel();

            //Default email to send receipts to is: "receipts@telaeris.com"
           //! ReceiverDefaultEmailString = "receipts@telaeris.com";

            //!senderEmail = "jasmine@telaeris.com";

            //Bind the strings to the entry boxes
           // SenderDefaultEmailEntry.BindingContext = SenderDefaultEmailString;
            //SenderDefaultEmailEntry.SetBinding(SenderDefaultEmailEntry.TextProperty, ".", BindingMode.TwoWay);
            /*
            var SenderEntryText = SenderDefaultEmailEntry;
            SenderEntryText.TextChanged += SenderEntry_TextChanged;


            //ReceiverEntryText gets the text from the receiver's default e-mail
            var ReceiverEntryText = ReceiverDefaultEmailEntry;
            ReceiverEntryText.TextChanged += ReceiverEntry_TextChanged;

            //Sender


            //Called when receiver's default e-mail is changed
			void ReceiverEntry_TextChanged(object sender, TextChangedEventArgs e)
			{
				String oldRText = e.OldTextValue;  //save the old value in case 'cancel' clicked?
				ReceiverDefaultEmailString = e.NewTextValue; //set to new value

			}

            void SenderEntry_TextChanged(object sender, TextChangedEventArgs e)
            {
                String oldMText = e.OldTextValue;
                SenderDefaultEmailString = e.NewTextValue;

            }


*/
		} //end SettingsPage



		public String getReceiverEmail()
		{
			return ReceiverDefaultEmailString;
		}

		public String getSenderEmail()
		{
			return SenderDefaultEmailString;
		}

		
	}
}
