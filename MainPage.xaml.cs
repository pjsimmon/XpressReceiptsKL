﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Messaging;
using Plugin.Media.Abstractions;
using MailKit.Net.Smtp;
using MimeKit;
using Xamarin.Auth;



namespace XpressReceipt
{


    public partial class MainPage : ContentPage
    {
        //
        //Image image = new Image();
        ICredentialsService storeService;
        String file_location = null;

        public MainPage()
        {
            InitializeComponent();

            storeService = DependencyService.Get<ICredentialsService>();
            bool doCredentialsExist = DependencyService.Get<ICredentialsService>().DoCredentialsExist();
            if (!doCredentialsExist)
            {
                storeService.SaveCredentials("jasmine@telaeris.com", "password");
            }
            String pw = storeService.Password;
        }


        private async void BtnCapture_Clicked(object sender, EventArgs e)
        {

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available.", "OK");
                return;
            }

            var mediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Receipts",
                //SaveToAlbum = true,
                Name = $"{DateTime.UtcNow}.jpg",
                PhotoSize = PhotoSize.Medium
            };

            if (mediaOptions == null)
                return;

            var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);

            //await DisplayAlert("File Location", file.Path, "OK");
            file_location = file.Path;
            ImageTaken.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });

        }


        private async void BtnUpload_Clicked(object sender, EventArgs e)
        {
	         await Navigation.PushAsync(new EmailPage());

	             //SendTheEmail();

				/////OG KELLY:
				
		      var emailMessenger = CrossMessaging.Current.EmailMessenger;
		      if (emailMessenger.CanSendEmail && file_location != null)
		      {
			        var email = new EmailMessageBuilder()
			        .To("jasmine@telaeris.com")
			        .Subject($"Receipt on {DateTime.UtcNow.ToLocalTime()} ")
			        .Body($"Receipt on {DateTime.UtcNow.ToLocalTime()} ")
			        .WithAttachment(file_location, "image/jpeg")
			        .Build();


			        emailMessenger.SendEmail(email);
		      }

		      SendSMTPMail("jasmine@telaeris.com", "jasmine@telaeris.com", "Subject: Email from Xpress Receipts", "Email body");

		}



        public void SendSMTPMail(string from, string to, string subject, string body)
        {
            //Construct the email to be sent
            var message = new MimeMessage();
            var builder = new BodyBuilder();

            message.From.Add(InternetAddress.Parse(from));
            message.To.Add(InternetAddress.Parse(to));
            message.Subject = subject;


            builder.TextBody = body;

			message.Body = builder.ToMessageBody();

			//trying new stuff here -- need to attach image //
			var attachment = new MimePart(file_location, "image/jpeg")
			{
				ContentObject = new ContentObject(System.IO.File.OpenRead(file_location), ContentEncoding.Default),
				ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
				ContentTransferEncoding = ContentEncoding.Base64,
				FileName = System.IO.Path.GetFileName(file_location)
			};

			// now create the multipart/mixed container to hold the message text and the
			// image attachment
			var multipart = new Multipart("mixed");
			multipart.Add(message.Body);
			multipart.Add(attachment);

			// now set the multipart/mixed as the message body
			message.Body = multipart;

            //end of trying new stuff //



            //Connect to server and send e-mail
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect("smtp.gmail.com", 587, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                try
                {
                    client.Authenticate("jasmine@telaeris.com", "*********");
                    client.Send(message);
                    client.Disconnect(true);
                }
                catch (MailKit.Security.AuthenticationException ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Something went wrong");
                    Console.WriteLine("Could not send the e-mail");
                }
            }

        } //end SendSMTPMail

        private void SendTheEmail()
        {

			var emailMessenger = CrossMessaging.Current.EmailMessenger;
			if (emailMessenger.CanSendEmail)
			{
				// Send simple e-mail to single receiver without attachments, bcc, cc etc.
				emailMessenger.SendEmail("jasmine@telaeris.com", "Xamarin Messaging Plugin: EMAIL SENT", "Well hello there from Xam.Messaging.Plugin");

                /*
				// Alternatively use EmailBuilder fluent interface to construct more complex e-mail with multiple recipients, bcc, attachments etc. 
				var email = new EmailMessageBuilder()
				  .To("jasmine@telaeris.com")
				  //.Cc("cc.plugins@xamarin.com")
				  //.Bcc(new[] { "bcc1.plugins@xamarin.com", "bcc2.plugins@xamarin.com" })
				  .Subject("Xamarin Messaging Plugin")
				  .Body("Well hello there from Xam.Messaging.Plugin")
				  .Build();

				emailMessenger.SendEmail(email);
                */
                Console.WriteLine("Sent the e-mail!");
			}
            else {
                Console.WriteLine("Couldn't send email");
            }

		}

		private void BtnSettings_Clicked(object sender, EventArgs e)
		{
			SettingsPage();
		}
            

        public void SettingsPage()
        {
            Navigation.PushAsync(new SettingsPage());
        }

    }
}
	  
	
