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

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;



namespace XpressReceipt
{
    
    public partial class MainPage : ContentPage
    {
        //
        //Image image = new Image();
        //ICredentialsService storeService;
        String file_location = null;
        MediaFile photo;

        public MainPage()
        {
            InitializeComponent();

            /*
            storeService = DependencyService.Get<ICredentialsService>();
            bool doCredentialsExist = DependencyService.Get<ICredentialsService>().DoCredentialsExist();
            if (!doCredentialsExist)
            {
                storeService.SaveCredentials("jasmine@telaeris.com", "password");
            }
            String pw = storeService.Password;
            */
        } //end MainPage()


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
           

            try
            {
                
                var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);

				//////Perform OCR on the image//////
                 /*
				OcrResults text;
                double total = 0.0;

				var client = new VisionServiceClient("1c9ce69ee64a4d10998e3683da0d8071", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");
                using (var photoStream = file.GetStream())
                {
                    //text = await client.RecognizeTextAsync(photoStream);
                    text = client.RecognizeTextAsync(photoStream, "eng", "true");
                }


				foreach (var region in text.Regions)
				{
					foreach (var line in region.Lines)
					{
						foreach (var word in line.Words)
						{
                            Console.WriteLine("A word is: " + word);
                            Console.WriteLine("A word_string is: " + word.ToString());
							if (word.Text.Contains("$"))
							{
								Console.WriteLine("The word is: " + word);
								var number = Double.Parse(word.Text.Replace("$", ""));

								total = (number > total) ? number : total;
							}
						}
					}
				}

				String string_total = total.ToString();
			

				await DisplayAlert("Alert", "The total is:" + string_total, "OK");
                */


                /////////////////////////////////


                file_location = file.Path;


                 //Shows picture on main screen.
                ImageTaken.Source = ImageSource.FromStream(() => 
				{
                    var stream = file.GetStream();
                    file.Dispose();
                    return stream;
                });

                /////////get the photo to perform OCR on /////////
                //photo = file;

            }
            catch (System.NullReferenceException) //when user cancels picture
            {
                await Navigation.PushAsync(new MainPage());
            }


        }


        private async void BtnUpload_Clicked(object sender, EventArgs e)
        {
	        await Navigation.PushAsync(new EmailPage(file_location));

        }

       

		private void BtnSettings_Clicked(object sender, EventArgs e)
		{
			SettingsPage();
		}
            

        public void SettingsPage()
        {
             Navigation.PushAsync(new SettingsPage());
            //Navigation.PushAsync(new LoginPage());
        }



        //Perform OCR on the photo


    }
}
	  
	
