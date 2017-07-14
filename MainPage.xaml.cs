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
		           storeService.SaveCredentials("kelly.lim@telaeris.com", "jayiscool3");
		      } 
		      String pw = storeService.Password;
	    }

	    private async void BtnCapture_Clicked(object sender, EventArgs e)
	    {

		      await CrossMedia.Current.Initialize();

		      if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) 
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



	            /*
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


	      SendSMTPMail("jasmine@telaeris.com", "jasmine@telaeris.com", "hi", "hihihihi");
	      */
	    }


	    private void BtnSettings_Clicked(object sender, EventArgs e)
	    {
	      SettingsPage();
	    }
	    public void SendSMTPMail(string from, string to, string subject, string body)
	    {
	      var message = new MimeMessage();
	      var builder = new BodyBuilder();

	      message.From.Add(InternetAddress.Parse(from));
	      message.To.Add(InternetAddress.Parse(to));
	      message.Subject = subject;

	      builder.TextBody = body;

	      message.Body = builder.ToMessageBody();

	      using (var client = new SmtpClient())
	      {
	        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

	        client.Connect("smtp.gmail.com", 587, false);
	        client.AuthenticationMechanisms.Remove("XOAUTH2");
	        try
	        {
	          client.Authenticate("kelly.lim@telaeris.com", "jayiscool");
	          client.Send(message);
	          client.Disconnect(true);
	        }
	        catch(MailKit.Security.AuthenticationException)
	        {

	        }
	      }
	    }

	    public void SettingsPage()
	    {
	      Navigation.PushAsync(new SettingsPage());
	    }
	  }
	}
