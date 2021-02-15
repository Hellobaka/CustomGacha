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
    }
}