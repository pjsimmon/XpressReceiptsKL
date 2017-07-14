using System;

using Xamarin.Forms;

namespace XpressReceipt
{
    public class EmailPage : ContentPage
    {
        public EmailPage()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

