using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Auth;
using Xamarin.Forms;
using XpressReceipt.Droid;

[assembly: Dependency(typeof(CredentialsService))]
namespace XpressReceipt.Droid
{
  public class CredentialsService : ICredentialsService
  {
    public string UserName
    {
      get
      {
        var account = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
        return (account != null) ? account.Username : null;
      }
    }

    public string Password
    {
      get
      {
        var account = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
        return (account != null) ? account.Properties["Password"] : null;
      }
    }

    public void SaveCredentials(string userName, string password)
    {
      if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
      {
        Account account = new Account
        {
          Username = userName
        };
        account.Properties.Add("Password", password);
        AccountStore.Create(Forms.Context).Save(account, App.AppName);
      }
    }

    public void DeleteCredentials()
    {
      var account = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
      if (account != null)
      {
        AccountStore.Create(Forms.Context).Delete(account, App.AppName);
      }
    }


    public bool DoCredentialsExist()
    {
      return AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).Any() ? true : false;
    }
  }
}