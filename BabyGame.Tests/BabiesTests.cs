using System.Diagnostics;
using BabyGame.Babies;
using BabyGame.Data;

namespace BabyGame.Tests
{
    [TestClass]
    public sealed class BabiesTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var x = new BetterKissesBaby()
            {
                BirthDate = DateTimeOffset.Now,
                Level = 10,
                Marriage = null,
                Name = null,
                TotalExperience = 0,
                MotherId = new Spouse()
            };
            var y = x.GetChu(Enumerable.Repeat(x, 1).OfType<Baby>().ToList());
            Debug.WriteLine(y);
        }
    }
}
