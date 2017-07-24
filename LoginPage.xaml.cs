using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace XpressReceipt
{
    public partial class LoginPage : ContentPage
    {
        String user_email, user_password;

        public LoginPage()
        {
            InitializeComponent();


			var label_Title = new Label();
			label_Title.Text = "Login Page";

			var label_email = new Label();
			label_email.Text = "Enter your e-mail:";

			var label_pw = new Label();
			label_pw.Text = "Enter your password";

		}

			

	}
	/*
    private void UpdateLoginBtn_Clicked(object sender, EventArgs e)
	{

		//user_email = FindViewById<Entry>
		//user_password = loginPassword.Text;

	}
*/


}
