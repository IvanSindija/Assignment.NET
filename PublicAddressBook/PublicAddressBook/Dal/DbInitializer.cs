using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAddressBook.Dal
{
    public static class DbInitializer
    {
        public static void Initialize(PublicAddressBookContext context)
        {
            //context.Database.EnsureCreated();

            // Look for any students.
            if (context.Contacts.Any())
            {
                return;   // DB has been seeded
            }
            int contactsSize = 1000;
            var contacts = new Contact[contactsSize];

            for (int i = 0; i < contactsSize; i++)
            {
                contacts[i] = new Contact { Name = "Carson_" + i, Address = "Ulica_" + i, DateOfBirth = DateTime.Parse("2005-09-01") };

                var rng = new Random();
                int phoneNumbersSize = rng.Next(0, 20);
                if (phoneNumbersSize > 0)
                {
                    contacts[i].PhoneNumbers = new List<PhoneNumber>();
                    for (int j = 0; j < phoneNumbersSize; j++)
                    {
                        contacts[i].PhoneNumbers.Add(new PhoneNumber() { Number = Guid.NewGuid().ToString() });
                    }
                }
            }

            foreach (Contact c in contacts)
            {
                context.Contacts.Add(c);
            }
            context.SaveChanges();
        }
    }
}
