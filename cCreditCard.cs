using System.ComponentModel;
using System.Collections.Generic;
using SQLite;
namespace XpressReceipt
{
    [Table("CreditCards")]
    public class cCreditCard : INotifyPropertyChanged
    {
      private int _id;
      private string _creditCardName;
      private string _creditCardNum;

      [PrimaryKey, AutoIncrement]
      public int Id
      {
         get
         {
            return _id;
         }
         set
         {
            this._id = value;
            OnPropertyChanged(nameof(Id));
         }
      }

      [NotNull]
      public string CreditCardName
      {
         get
         {
            return _creditCardName;
         }
         set
         {
            this._creditCardName = value;
            OnPropertyChanged(nameof(CreditCardName));
         }
      }

      [NotNull, MaxLength(4)]
      public string CreditCardNum
      {
         get
         {
            return _creditCardNum;
         }
         set
         {
            this._creditCardNum = value;
            OnPropertyChanged(nameof(CreditCardNum));
         }
      }
      public event PropertyChangedEventHandler PropertyChanged;
      private void OnPropertyChanged(string propertyName)
      {
         this.PropertyChanged?.Invoke(this,
           new PropertyChangedEventArgs(propertyName));
      }
   }


}
