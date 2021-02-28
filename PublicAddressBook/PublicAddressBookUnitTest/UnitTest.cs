using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PublicAddressBook.Dal;
using PublicAddressBook.Handlers.impl;
using PublicAddressBook.Handlers.intf;
using PublicAddressBook.Hubs;
using PublicAddressBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PublicAddressBookUnitTest
{
    [TestClass]
    public class UnitTest
    {
        private PublicAddressBookContext _db;
        private Mock<IHubContext<SignalRHub>> _hub;
        private Mock<IClientProxy> _mockClientProxy;
        private Mock<IHubClients> _mockClients;
        [TestInitialize()]
        public void InitTest()
        {
            _db = new PublicAddressBookContext(new DbContextOptionsBuilder<PublicAddressBookContext>().UseInMemoryDatabase("TestDb")
                .Options);
            DbInitializer.Initialize(_db);
            _hub = new Mock<IHubContext<SignalRHub>>();
            _mockClientProxy = new Mock<IClientProxy>();

            _mockClients = new Mock<IHubClients>();
            _mockClients.Setup(clients => clients.All).Returns(_mockClientProxy.Object);

            _hub.Setup(x => x.Clients).Returns(() => _mockClients.Object);
        }

        [TestCleanup()]
        public void CleanupTest()
        {
            _db.Dispose();
        }

        [TestMethod]
        public async Task GetContact()
        {
            IContactHandler contactHandler = new ContactHandler(_db, _hub.Object);
            var contacts = await contactHandler.GetContact(7);

            Assert.IsTrue(contacts.Count > 0);
        }

        [TestMethod]
        public async Task GetContactById()
        {
            IContactHandler contactHandler = new ContactHandler(_db, _hub.Object);
            var contact = await contactHandler.GetContactById(15);

            Assert.AreEqual(contact.Id, 15);
        }

        [TestMethod]
        public async Task AddContact()
        {
            IContactHandler contactHandler = new ContactHandler(_db, _hub.Object);
            var testName = "Mile_" + Guid.NewGuid().ToString();
            var testAddress = "Ulica_" + Guid.NewGuid().ToString();
            var testContact = new ContactViewModel()
            {
                Name = testName,
                DateOfBirth = DateTime.Now,
                Address = testAddress,
                PhoneNumbers = new List<PhoneNumberViewModel>() { new PhoneNumberViewModel() { Number = "0999" } }
            };
            await contactHandler.AddContact(testContact);

            var contacts = _db.Contacts.Where(c => c.Name == testName && c.Address == testAddress).ToList();
            Assert.AreEqual(contacts.Count, 1);

            _mockClients.Verify(c => c.All, Times.Once);
        }

        [TestMethod]
        public async Task AddDuplicateContact()
        {
            IContactHandler contactHandler = new ContactHandler(_db, _hub.Object);
            var testName = "Mile_" + Guid.NewGuid().ToString();
            var testAddress = "Ulica_" + Guid.NewGuid().ToString();
            var testContact = new ContactViewModel()
            {
                Name = testName,
                DateOfBirth = DateTime.Now,
                Address = testAddress,
                PhoneNumbers = new List<PhoneNumberViewModel>() { new PhoneNumberViewModel() { Number = "0999" } }
            };
            await contactHandler.AddContact(testContact);
            try
            {
                await contactHandler.AddContact(testContact);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Exception during creation of new contact");
            }
        }
        [TestMethod]
        public async Task UpdateContact()
        {
            IContactHandler contactHandler = new ContactHandler(_db, _hub.Object);

            var testContact = await contactHandler.GetContactById(20);
            var testName = "Mile_" + Guid.NewGuid().ToString();
            var testAddress = "Ulica_" + Guid.NewGuid().ToString();
            testContact.Name = testName;
            testContact.Address = testAddress;
            await contactHandler.UpdateContact(testContact);

            var contacts = _db.Contacts.Where(c => c.Name == testName && c.Address == testAddress).ToList();
            Assert.AreEqual(contacts.Count, 1);

            _mockClients.Verify(c => c.All, Times.Once);
        }

        [TestMethod]
        public async Task DeleteContact()
        {
            IContactHandler contactHandler = new ContactHandler(_db, _hub.Object);

            var testContact = await contactHandler.GetContactById(12);
            Assert.IsNotNull(testContact);
            await contactHandler.DeleteContact(12);

            var contacts = _db.Contacts.Where(c => c.Id == 12).ToList();
            Assert.AreEqual(contacts.Count, 0);

            _mockClients.Verify(c => c.All, Times.Once);
        }

        [TestMethod]
        public async Task GetPhoneNumber()
        {
            IContactHandler contactHandler = new ContactHandler(_db, _hub.Object);
            IPhoneNumberHandler phoneNumberHandler = new PhoneNumberHandler(_db, _hub.Object, contactHandler);

            var numbers = await phoneNumberHandler.GetPhoneNumber(2);

            Assert.IsTrue(numbers.Count > 0);
        }

        [TestMethod]
        public async Task GetPhoneNumberById()
        {
            IContactHandler contactHandler = new ContactHandler(_db, _hub.Object);
            IPhoneNumberHandler phoneNumberHandler = new PhoneNumberHandler(_db, _hub.Object, contactHandler);

            var number = await phoneNumberHandler.GetPhoneNumberById(2);

            Assert.AreEqual(number.Id, 2);
        }

        [TestMethod]
        public async Task AddPhoneNumber()
        {
            IContactHandler contactHandler = new ContactHandler(_db, _hub.Object);
            IPhoneNumberHandler phoneNumberHandler = new PhoneNumberHandler(_db, _hub.Object, contactHandler);
            var newNumber = "091" + Guid.NewGuid().ToString();
            var contact = await contactHandler.GetContactById(2);
            Assert.AreEqual(contact.Id, 2);
            Assert.IsNull(contact.PhoneNumbers.FirstOrDefault(p => p.Number == newNumber));

            await phoneNumberHandler.AddPhoneNumber(2, newNumber);

            _mockClients.Verify(c => c.All, Times.Once);
            contact = await contactHandler.GetContactById(2);
            Assert.AreEqual(contact.Id, 2);
            Assert.IsNotNull(contact.PhoneNumbers.FirstOrDefault(p => p.Number == newNumber));
        }

        [TestMethod]
        public async Task UpdatePhoneNumber()
        {
            IContactHandler contactHandler = new ContactHandler(_db, _hub.Object);
            IPhoneNumberHandler phoneNumberHandler = new PhoneNumberHandler(_db, _hub.Object, contactHandler);

            var phoneNumber = await phoneNumberHandler.GetPhoneNumberById(26);
            Assert.IsNotNull(phoneNumber);
            var phoneNumberNew = "2354" + Guid.NewGuid().ToString();
            phoneNumber.Number = phoneNumberNew;
            await phoneNumberHandler.UpdatePhoneNumber(phoneNumber);

            var phoneNumberResult = _db.PhoneNumbers.FirstOrDefault(c => c.Id == 26);
            Assert.AreEqual(phoneNumberResult.Number, phoneNumberNew);

            _mockClients.Verify(c => c.All, Times.Once);
        }

        [TestMethod]
        public async Task DeletePhoneNumber()
        {
            IContactHandler contactHandler = new ContactHandler(_db, _hub.Object);
            IPhoneNumberHandler phoneNumberHandler = new PhoneNumberHandler(_db, _hub.Object, contactHandler);

            var phoneNumber = await phoneNumberHandler.GetPhoneNumberById(12);
            Assert.IsNotNull(phoneNumber);
            await phoneNumberHandler.DeletePhoneNumber(12);

            var phoneNumbers = _db.PhoneNumbers.Where(c => c.Id == 12).ToList();
            Assert.AreEqual(phoneNumbers.Count, 0);

            _mockClients.Verify(c => c.All, Times.Once);
        }
    }
}
