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
    public partial class EmailPage : ContentPage
    {

		String userEmail = "";
		String userPassword = "";
		String userCard = "";

        String file_location;

        public EmailPage(String picturePath)
        {
            InitializeComponent();
            BindingContext = new DetailsViewModel();

            if (picturePath == null)
            {
                DisplayAlert("Alert", "You must take a picture before sending an e-mail.", "OK");

            }
            //Display the image
            else 
            {
                //If they had already sent an e-mail, should have a recipient stored.
                if (Application.Current.Properties.ContainsKey("receiver_email"))
                {
                    var to_field = Application.Current.Properties["receiver_email"] as String;
                    ToField.Text = to_field;
                }

                file_location = picturePath;

				//ImageTaken = new Image { Aspect = Aspect.AspectFit };
				ImageTaken.Source = ImageSource.FromFile(picturePath);
				

                //Make sure that a user is logged in.
				if (Application.Current.Properties.ContainsKey("user_e"))
				{
					var fromF = Application.Current.Properties["user_e"] as String;
					FromField.Text = "From: " + fromF;
				}
				else
				{
					DisplayAlert("Alert:", "Please make sure you are logged in first.", "OK");

				}

               
                
            }
			

        } //End EmailPage
        

		public void BtnUploadConfirm_Clicked(object sender, EventArgs e)
		{
            if (file_location == null)
            {
                DisplayAlert("Alert", "You must take a picture before sending an e-mail.", "OK");
            }
            else 
            {

                ///// RECEIVER INFORMATION /////
            
                if (ToField.Text == null) 
                {
					DisplayAlert("Alert", "Your e-mail must have a recipient.", "OK");

				}
                else 
                {
					String receiverEmail = ToField.Text;
					Application.Current.Properties["receiver_email"] = ToField.Text;
					//////USER INFORMATION ////////

					//get Persistent info from the login page
					if (Application.Current.Properties.ContainsKey("user_e")
						&& Application.Current.Properties.ContainsKey("user_pw")
						&& Application.Current.Properties.ContainsKey("user_card"))
					{
						var id_e = Application.Current.Properties["user_e"] as String;
						userEmail = id_e;
						var id_pw = Application.Current.Properties["user_pw"] as String;
						userPassword = id_pw;
						var id_c = Application.Current.Properties["user_card"] as String;
						userCard = id_c;
					}


					var emailMessenger = CrossMessaging.Current.EmailMessenger;
                    /*
					if (emailMessenger.CanSendEmail && file_location != null)
					{
						var email = new EmailMessageBuilder()
						.To(receiverEmail) //"reciepts@telaeris.com <---- reciever
						.Subject($"Receipt on {DateTime.UtcNow.ToLocalTime()} ")
						.Body($"Receipt on {DateTime.UtcNow.ToLocalTime()} ")
						.WithAttachment(file_location, "image/jpeg")
						.Build();


						emailMessenger.SendEmail(email);
					}
                    */

					//This gets executed instead:
					SendSMTPMail(userEmail, receiverEmail, MessageField.Text, "Email body");


				}


			}
               
			
		} //end Upload->Confirm Clicked



		public void SendSMTPMail(string from, string to, string subject, string body)
		{
			//Construct the email to be sent
			var message = new MimeMessage();
			var builder = new BodyBuilder();

			message.From.Add(InternetAddress.Parse(from));
			message.To.Add(InternetAddress.Parse(to));
			message.Subject = subject + "-" + userCard; //concatenate credit card #


			builder.TextBody = body;

			message.Body = builder.ToMessageBody();

			//trying new stuff here -- need to attach image //
			try
			{
				var attachment = new MimePart(file_location, "image/jpeg")
				{
					ContentObject = new ContentObject(System.IO.File.OpenRead(file_location), ContentEncoding.Default),
					ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
					ContentTransferEncoding = ContentEncoding.Base64,
					FileName = System.IO.Path.GetFileName(file_location)
				};

				// image attachment
				var multipart = new Multipart("mixed");
				multipart.Add(message.Body);
				multipart.Add(attachment);

				// now set the multipart/mixed as the message body
				message.Body = multipart;

			}
			catch (System.ArgumentNullException anex)
			{
				Console.WriteLine(" ?????? " + anex);
				DisplayAlert("Alert:", "You cannot upload without taking a picture first.", "OK");
			}


			//Connect to server and send e-mail
			using (var client = new SmtpClient())
			{
				client.ServerCertificateValidationCallback = (s, c, h, e) => true;

				client.Connect("smtp.gmail.com", 587, false);
				client.AuthenticationMechanisms.Remove("XOAUTH2");
				try
				{
					client.Authenticate(userEmail, userPassword);
					client.Send(message);
					client.Disconnect(true);
					DisplayAlert("Success!", "Email successfully sent.", "OK");
				}
				catch (MailKit.Security.AuthenticationException ex)
				{
					Console.WriteLine(ex);

					DisplayAlert("Alert:", "You must be logged in to send an email." +
								 " Please go to the settings page and login.", "OK");

					Console.WriteLine("Something went wrong");
					Console.WriteLine("Could not send the e-mail");
				}
			}

		} //end SendSMTPMail

	}
}
