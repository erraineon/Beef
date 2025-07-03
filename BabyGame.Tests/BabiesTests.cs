using System.Diagnostics;
using BabyGame.Babies;
using BabyGame.Data;
using BabyGame.Events;

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
                TotalExperience = 0
            };
            var y = x.Handle(new BabyEventArgs<IChuOnKiss, double>());
            Debug.WriteLine(y);
        }
    }
}
