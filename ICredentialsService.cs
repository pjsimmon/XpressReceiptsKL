using System;
using System.Collections.Generic;
using System.Text;

namespace XpressReceipt
{
    public interface ICredentialsService
    {
      String UserName { get; }

      String Password { get; }

      void SaveCredentials(string userName, string password);

      void DeleteCredentials();

      bool DoCredentialsExist();

    }
}
