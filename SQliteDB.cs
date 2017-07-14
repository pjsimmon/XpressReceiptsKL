using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;
namespace XpressReceipt
{
    class SQliteDB
    {
        private SQLiteConnection database;
        private static object collisionLock = new object();

        public ObservableCollection<cCreditCard> CreditCards { get; set; }


        public SQliteDB()
        {
            database =
              DependencyService.Get<ISQLiteDB>().
              DbConnection();
            database.CreateTable<cCreditCard>();
            this.CreditCards =
              new ObservableCollection<cCreditCard>(database.Table<cCreditCard>());
            // If the table is empty, initialize the collection
            if (!database.Table<cCreditCard>().Any())
            {
                AddNewCreditCard();
            }
        }

		public void AddNewCreditCard()
		{
			this.CreditCards.
			  Add(new cCreditCard()
			  {
				  CreditCardName = "Credit card name...",
				  CreditCardNum = "Credit card number..."
			  });
		}
        /*
		public IEnumerable<Customer> GetCreditCards()
		{
			lock (collisionLock)
			{
				var query = from creditcards in database.Table<cCreditCard>()
							select creditcards;
				return query.AsEnumerable();
			}
		}

		public Customer GetCreditCard(int id)
		{
			lock (collisionLock)
			{
				return database.Table<cCreditCard>().
				  FirstOrDefault(ccreditcard => ccreditcard.Id == id);
			}
		}

		public int CreateorUpdateCreditCard(cCreditCard creditCardInstance)
		{
			lock (collisionLock)
			{
				if (customerInstance.Id != 0)
				{
					database.Update(customerInstance);
					return customerInstance.Id;
				}
				else
				{
					database.Insert(customerInstance);
					return customerInstance.Id;
				}
			}
		}
		*/


    }

}