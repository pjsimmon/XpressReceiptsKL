using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Plugin.Media;
using Plugin.Messaging;
using Plugin.Media.Abstractions;
using MailKit.Net.Smtp;
using MimeKit;
using Xamarin.Auth;


namespace XpressReceipt
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
        String user_email, user_password, user_card;

        /*
         * The settings page allows the user to input their e-mail
         * address, password, and last 4 digits of the credit card
         * number and save it.
         */
		public SettingsPage ()
		{
			InitializeComponent ();
            BindingContext = new DetailsViewModel();

			if (Application.Current.Properties.ContainsKey("user_e") 
                && Application.Current.Properties.ContainsKey("user_pw")
                && Application.Current.Properties.ContainsKey("user_card"))
			{
				var id_e = Application.Current.Properties["user_e"] as String;
				SenderEmail.Text = id_e;
				var id_pw = Application.Current.Properties["user_pw"] as String;
				SenderPassword.Text = id_pw;
				var id_c = Application.Current.Properties["user_card"] as String;
				Last4Digits.Text = id_c;
			}
           

		} //end SettingsPage

		//Settings for user configuration
		//Only want this to happen when the "Update Settings" button is clicked
		public void UpdateSettings_Clicked(object sender, EventArgs evento)
		{
			user_email = SenderEmail.Text;
			user_password = SenderPassword.Text;
            user_card = Last4Digits.Text;

            //Check to see if card is length 4:
            if (user_card.Length != 4)
            {
				DisplayAlert("Alert:", "Please make sure you entered 4 digits" +
                             " for your credit card number.", "OK");
			}
			
            //Test to see if the user is valid/entered info correctly
			//Connect to server and send e-mail
            else {
				
				using (var client = new SmtpClient())
				{
					client.ServerCertificateValidationCallback = (s, c, h, e) => true;

					client.Connect("smtp.gmail.com", 587, false);
					client.AuthenticationMechanisms.Remove("XOAUTH2");
					try
					{
						client.Authenticate(user_email, user_password);
						client.Disconnect(true);

						//Will catch authentication failure before setting info
						Application.Current.Properties["user_e"] = user_email;
						Application.Current.Properties["user_pw"] = user_password;
						Application.Current.Properties["user_card"] = user_card;
					    DisplayAlert("Alert:", "You are logged in!", "OK");

						Navigation.PopAsync(); //get back to main page and still have image saved

                    }
					catch (MailKit.Security.AuthenticationException ex)
					{
						Console.WriteLine(ex);

						DisplayAlert("Alert:", "Email and/or password are incorrect.", "OK");

						Console.WriteLine("Could not send the e-mail, settings wrong!");
					}
				}

                
            }

			

		} // end updateSettingsClicked


        public void CancelSettingsBtn_Clicked(object sender, EventArgs event2)
        {

			if (Application.Current.Properties.ContainsKey("user_e")
				&& Application.Current.Properties.ContainsKey("user_pw")
                && Application.Current.Properties.ContainsKey("user_card"))
			{
				var id_e = Application.Current.Properties["user_e"] as String;
				SenderEmail.Text = id_e;
				var id_pw = Application.Current.Properties["user_pw"] as String;
				SenderPassword.Text = id_pw;
				var id_c = Application.Current.Properties["user_card"] as String;
				Last4Digits.Text = id_c;
			}

            Navigation.PushAsync(new MainPage());

        } //end CancelSettingsBtn_Clicked

		
	}
}
