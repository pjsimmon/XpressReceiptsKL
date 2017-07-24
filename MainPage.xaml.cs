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
        //ICredentialsService storeService;
        String file_location = null;

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

                file_location = file.Path;
                ImageTaken.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    file.Dispose();
                    return stream;
                });

            }
            catch (System.NullReferenceException nrex) //when user cancels picture
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

    }
}
	  
	
