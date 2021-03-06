﻿//using Java.IO;

using System;
using System.Collections;
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
        String toEmail = "";

        String receiptTotal;

        String file_location;

        String purpose = "";
        //MediaFile photo_receipt;

        public EmailPage(String photo)
        {
            InitializeComponent();
            BindingContext = new DetailsViewModel();
            this.visionClient = new VisionServiceClient("1c9ce69ee64a4d10998e3683da0d8071", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");

            //Set up the default values for the entries:
            /*
            ToField.Text = "Recipient Email Address";
            VendorField.Text = "Vendor:";
            AmountField.Text = "Total $ Amount:";
            DateField.Text = "Date of Purchase:";
            */


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
                    toEmail = to_field;
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

        //Method for choosing the Purpose/Category for expense from the Picker
        public void onPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("PICKER IS PICKIN!!" );
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
            if (selectedIndex != -1)
            {
                purpose = (string)picker.ItemsSource[selectedIndex];
                Console.WriteLine("Purpose is: " + purpose);
            }

        }
        

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
                    //Check to make sure that there is something in each text field.
                    //(Checked null but should also check empty case).
                    if (String.IsNullOrEmpty(purpose) || String.IsNullOrEmpty(VendorField.Text)||
                        String.IsNullOrEmpty(AmountField.Text) || String.IsNullOrEmpty(DateField.Text) ||
                        String.IsNullOrEmpty(CardField.Text))
                    {
                        DisplayAlert("Alert", "Please make sure that each entry  is filled with information.", "OK");
                        return;
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
							//var id_c = Application.Current.Properties["user_card"] as String;
							//userCard = id_c;
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
						// String subjectLine = (PurposeField.Text + " [" + VendorField.Text + AmountField.Text + DateField.Text + "]");
						String subjectLine = (purpose + " [" + VendorField.Text + " " + AmountField.Text + " " + DateField.Text + "]");

                        String emailBody = BodyField.Text;

						SendSMTPMail(userEmail, receiverEmail, subjectLine, emailBody);

					}

				}
                        
            }
					 	
		} //end Upload->Confirm Clicked


        //When OCR button clicked
        public void Perform_OCR(object sender, EventArgs e)
        {
            OcrGetTotal(file_location);
            OcrGetDate(file_location);
            OcrGetLast4Digits(file_location);

        }



		public void SendSMTPMail(string from, string to, string subject, string body)
		{
			//Construct the email to be sent
			var message = new MimeMessage();
			var builder = new BodyBuilder();

			message.From.Add(InternetAddress.Parse(from));
			message.To.Add(InternetAddress.Parse(to));


            if (CardField.Text != userCard)
            {
                userCard = CardField.Text;
            }

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
            Console.WriteLine("GETTING OCR TOTAL");
            //API Key for OCR access:
            //VisionServiceClient client = new VisionServiceClient("1c9ce69ee64a4d10998e3683da0d8071");

			OcrResults text;
            double total = 0.0;
            //var photo_location = photo.Path;

            String[] decimals = new String[50]; //make an array of all doubles
            int counter = 0;
            double max = 0.0;

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
                //try
                //{
                    text = await visionClient.RecognizeTextAsync(photoStream);  //Causes android to stop working?

                //} catch (Exception e)
                //{
                //    Console.WriteLine("Exception Caught: " + e); //System.Net.WebException: Error: NameResolutionFailure
				//}
            }


			foreach (var region in text.Regions)
			{
				foreach (var line in region.Lines)
				{
					foreach (var word in line.Words)
					{
                        Console.WriteLine("A word is: " + word.Text);
                        if (word.Text.Contains("$"))
                        {
                            try
                            {
                                Console.WriteLine("The word is:" + word.Text);
                                var word_double = word.Text.Substring(1); //Removes the dollar sign from the double
                                Console.WriteLine("The word is:" + word.Text);
                                var number = double.Parse(word_double, System.Globalization.CultureInfo.InvariantCulture);


                                total = (number > total) ? number : total;

                            }
                            //System.FormatException: Input string was not in a correct format.
                            catch (Exception e)
                            {
                                Console.WriteLine("Caught Exception: " + e);
                            }

                        }
                        ///////// if the receipt doesn't use $ signs /////////
                        else if (word.Text.Contains("."))
                        {
                            decimals[counter] = word.Text;
                            counter++;
                            try
                            {
                                Console.WriteLine("Adding the decimal: " + word.Text);
                                double number = double.Parse(word.Text, System.Globalization.CultureInfo.InvariantCulture);
                                if (number > max)
                                {
                                    max = number;
                                }
                            }
                            catch (Exception e) //Invalid format b/c % sign maybe
                            {
                                Console.WriteLine("Caught Exception: " + e);
                            }
                        }
					}
				}
			}

            if (total.Equals(0.0)) //comparing doubles
            {
                total = max;
            }

            //String string_total = total.ToString("F", System.Globalization.CultureInfo.InvariantCulture);
            String string_total = total.ToString();
            receiptTotal = string_total;

			await DisplayAlert("Alert", "The total is:" + receiptTotal, "OK");

            //Maybe put this in its own method and call at end, when completely done with file.
            //photo.Dispose();

            AmountField.Text = "$" + total.ToString();



		} //end performOCR_getTotal


        /* OCR GET DATE */

		public async void OcrGetDate(String photo_loc)
		{
            Console.WriteLine("GETTING OCR DATE");
			//API Key for OCR access:
			//VisionServiceClient client = new VisionServiceClient("1c9ce69ee64a4d10998e3683da0d8071");

			OcrResults text;
			String date = "Unknown_date";
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
				//try
				//{
				text = await visionClient.RecognizeTextAsync(photoStream);  //Causes android to stop working?

				//} catch (Exception e)
				//{
				//    Console.WriteLine("Exception Caught: " + e); //System.Net.WebException: Error: NameResolutionFailure
				//}
			}

            String[] words_in_date_line = new String[100];  //assuming less than 50 words on a line
            //ArrayList words_in_date_line = new ArrayList();
            int counter = 0;

			foreach (var region in text.Regions)
			{
				foreach (var line in region.Lines)
				{
					foreach (var word in line.Words)
					{
						Console.WriteLine("A word is: " + word.Text);
						if (word.Text.Contains("AM") || word.Text.Contains("PM") || word.Text.Contains(":"))
						{
							try
							{
                                //Want to get that line again and extract the date
                               // foreach (var line2 in region.Lines)
                               // {
                                    foreach (var word2 in line.Words)
                                    {
                                        //get all the words and put them in an arrayList
                                       // words_in_date_line.Add(word2);
									   // Console.WriteLine("Adding word: " + word2.Text + " to array.");

										words_in_date_line[counter] = word2.Text;
										Console.WriteLine("Adding word: " + word2.Text + " to array.");
										counter++;
										
								}
                                //}
 
                            }
							//System.FormatException: Input string was not in a correct format.
							catch (Exception e)
							{
								Console.WriteLine("Caught Exception: " + e);
							}

						}
					}
				}
			}

            //Now scan that array to find the most-likely-date
            foreach (var a_string in words_in_date_line)
            {
                if (a_string != null) //need to check for nullException
                {
					//if (a_string.ToString().Contains("/") || a_string.ToString().Contains("-"))
                    if ((a_string.Contains("/") || a_string.Contains("-")) && a_string.Length <= 10) //date will be less than 10, helps not get phone #
					{
						date = a_string.ToString();
					}
                    
                }
				
            }

            /*
            for (int i = 0; i <= counter; i++)
            {
                if (words_in_date_line[i].Contains("/") || words_in_date_line[i].Contains("-"))
                {
                    date = words_in_date_line[i];
                }
            }
            */

			await DisplayAlert("Alert", "The date is:" + date, "OK");

            //Maybe put this in its own method and call at end, when completely done with file.
            //photo.Dispose();

            counter = 0;
            if (date.Equals("Unknown_date"))  //string comparison
            {
                DateField.Text = DateTime.Today.ToString("d"); //get today's date

            }
            else {
                DateField.Text = date;
            }


		} //end OCRGetDate


		public async void OcrGetLast4Digits(String photo_loc)
		{
            Console.WriteLine("GETTING OCR CREDIT CARD ####");
			//API Key for OCR access:
			//VisionServiceClient client = new VisionServiceClient("1c9ce69ee64a4d10998e3683da0d8071");

			OcrResults text;
			String cardNum = "0000";
            String[] cardArray = new String[5];
            int counter = 0;
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
				//try
				//{
				text = await visionClient.RecognizeTextAsync(photoStream);  //Causes android to stop working?

				//} catch (Exception e)
				//{
				//    Console.WriteLine("Exception Caught: " + e); //System.Net.WebException: Error: NameResolutionFailure
				//}
			}


			foreach (var region in text.Regions)
			{
				foreach (var line in region.Lines)
				{
					foreach (var word in line.Words)
					{
						Console.WriteLine("A word is: " + word.Text);
                        if (word.Text.Contains("XXXX") || word.Text.Contains("****") || word.Text.Contains("####")
                           || word.Text.Contains("xxxx"))
						{
							try
							{
								//Want to get that line again and extract the date
								// foreach (var line2 in region.Lines)
								// {
								foreach (var word2 in line.Words)
								{
									//get all the words and put them in an arrayList
									// words_in_date_line.Add(word2);
									// Console.WriteLine("Adding word: " + word2.Text + " to array.");

									cardArray[counter] = word2.Text;
									Console.WriteLine("Adding word: " + word2.Text + " to Card Array.");
									counter++;

								}
								//}

							}
							//System.FormatException: Input string was not in a correct format.
							catch (Exception e)
							{
								Console.WriteLine("Caught Exception: " + e);
							}

						}
					}
				}
			}

			//Now scan that array to find the most-likely-date
			foreach (var a_string in cardArray)
			{
				if (a_string != null) //need to check for nullException
				{
					//if (a_string.ToString().Contains("/") || a_string.ToString().Contains("-"))
					if (a_string.Contains("1") || a_string.Contains("2") || a_string.Contains("3")
                       || a_string.Contains("4") || a_string.Contains("5") || a_string.Contains("6")
                       || a_string.Contains("7") || a_string.Contains("8") || a_string.Contains("9")
                       || a_string.Contains("0"))
					{
                        cardNum = a_string.Substring(Math.Max(0, a_string.Length - 4));
					}

				}

			}

			await DisplayAlert("Alert", "The cardNum is:" + cardNum, "OK");

            if (cardNum != "0000")
            {
                CardField.Text = cardNum;

                //Makes the saved card the new OCR card #, not the login card #.
                userCard = CardField.Text;
            }
            else {
                if (Application.Current.Properties.ContainsKey("user_card"))
                {
					var id_c = Application.Current.Properties["user_card"] as String;
					userCard = id_c;
				}
                CardField.Text = userCard;
            }




			//Maybe put this in its own method and call at end, when completely done with file.
			//photo.Dispose();




		} //end performOCR_getTotal





	}
}
