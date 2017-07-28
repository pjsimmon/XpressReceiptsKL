//using Java.IO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//For email and picture
using Plugin.Media;
using Plugin.Connectivity;
using Plugin.Media.Abstractions;
using Plugin.Messaging;
using MailKit.Net.Smtp;
using MimeKit;
using Xamarin.Auth;


//For OCR
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;


namespace XpressReceipt
{

   
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmailPage : ContentPage
    {
		private readonly VisionServiceClient visionClient;

		String userEmail = "";
		String userPassword = "";
		String userCard = "";

        String receiptTotal;

        String file_location;
        //MediaFile photo_receipt;

        public EmailPage(String photo)
        {
            InitializeComponent();
            BindingContext = new DetailsViewModel();
            this.visionClient = new VisionServiceClient("1c9ce69ee64a4d10998e3683da0d8071");

            file_location = photo;

            if (photo == null)
            {
                DisplayAlert("Alert", "You must take a picture before sending an e-mail.", "OK");

            }
            //Display the image
            else 
            {
                //photo_receipt = photo;

                //If they had already sent an e-mail, should have a recipient stored.
                if (Application.Current.Properties.ContainsKey("receiver_email"))
                {
                    var to_field = Application.Current.Properties["receiver_email"] as String;
                    ToField.Text = to_field;
                }

                //file_location = photo.Path;

				//ImageTaken = new Image { Aspect = Aspect.AspectFit };
				ImageTaken.Source = ImageSource.FromFile(file_location);
				

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
                    String subjectLine = (PurposeField.Text + " [" + VendorField.Text + AmountField.Text + DateField.Text + "]");
					SendSMTPMail(userEmail, receiverEmail, subjectLine, "Email body");


				}


			}
               
			
		} //end Upload->Confirm Clicked


        //When OCR button clicked
        public void Perform_OCR(object sender, EventArgs e)
        {
            OcrGetTotal(file_location);
        }



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


        public async void OcrGetTotal(String photo_loc) 
        {
            //API Key for OCR access:
            //VisionServiceClient client = new VisionServiceClient("1c9ce69ee64a4d10998e3683da0d8071");

			OcrResults text;
            double total = 0.0;
            //var photo_location = photo.Path;

            String pathName = photo_loc;

            if (pathName == null)
            {
                await DisplayAlert("Alert", "No photo has been taken, cannot perform OCR.", "OK");
                return;
            }

            //Check for network Connectivity
			if (!CrossConnectivity.Current.IsConnected)
			{
				await DisplayAlert("Network Error", "Please reconnect network and retry", "OK");
				return;
			}
			
            //var new_photo = System.IO.File.OpenRead(photo_location);

            using (var photoStream = System.IO.File.OpenRead(photo_loc))
            {
                try
                {
                    text = await visionClient.RecognizeTextAsync(photoStream);  //Causes android to stop working?

                } catch (Exception e)
                {
                    Console.WriteLine("Exception Caught: " + e); //System.Net.WebException: Error: NameResolutionFailure
				}
            }
            		
            /*
			foreach (var region in text.Regions)
			{
				foreach (var line in region.Lines)
				{
					foreach (var word in line.Words)
					{
						if (word.Text.Contains("$"))
						{
							var number = Double.Parse(word.Text.Replace("$", ""));
							total = (number > total) ? number : total;
						}
					}
				}
			} 
               
            String string_total = total.ToString();
            receiptTotal = string_total;

			await DisplayAlert("Alert", "The total is:" + receiptTotal, "OK");

            //Maybe put this in its own method and call at end, when completely done with file.
            photo.Dispose();

            AmountField.Text = total.ToString();
            */


		} //end performOCR_getTotal




	}
}
