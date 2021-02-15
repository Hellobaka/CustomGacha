using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicInfos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicInfos.Tests
{
    [TestClass()]
    public class SQLHelperTests
    {
        [TestMethod()]
        public void CreateDBTest()
        {
            Debug.WriteLine(Environment.CurrentDirectory);
            SQLHelper.CreateDB();
        }

        [TestMethod()]
        public void IDExistsTest()
        {
            Assert.AreEqual(SQLHelper.IDExists(863450594), false);
        }

        [TestMethod()]
        public void RegisterTest()
        {
            var c = SQLHelper.Register(863450594);
            Debug.WriteLine(c.ToString());
        }

        [TestMethod()]
        public void InsertGachaItemTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetUserTest()
        {
            var c = SQLHelper.GetUser(863450594);
            Debug.WriteLine(c.ToString());
        }

        [TestMethod()]
        public void GetMoneyTest()
        {
            var c = SQLHelper.GetMoney(863450594);
            Assert.AreEqual(c, 3200);
        }

        [TestMethod()]
        public void LoadConfigTest()
        {
            SQLHelper.LoadConfig();
            Assert.AreEqual(MainSave.SignFloor, 1600);
        }

        [TestMethod()]
        public void SignTest()
        {
            SQLHelper.LoadConfig();
            var c = SQLHelper.Sign(863450594);
            Debug.WriteLine(SQLHelper.GetUser(863450594).ToString());
            Assert.IsTrue(c > 0);
        }
    }
}