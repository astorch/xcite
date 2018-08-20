using NUnit.Framework;

namespace xcite.csharp.tests {
    [TestFixture]
    public class XEnumTests {
        [Test]
        public void Values() {
            EKinds[] values = EKinds.Values;

            Assert.NotNull(values);
            Assert.AreEqual(3, values.Length);
            Assert.AreEqual(EKinds.Default, values[0]);
            Assert.AreEqual(EKinds.StageOne, values[1]);
            Assert.AreEqual(EKinds.StageTwo, values[2]);
        }

        [Test]
        public void TryParse() {
            bool match = EKinds.TryParse("StageOne", out EKinds matchValue);
            bool noMatch = EKinds.TryParse("stageThree", out EKinds noMatchValue);

            Assert.IsTrue(match);
            Assert.AreEqual(EKinds.StageOne, matchValue);

            Assert.IsFalse(noMatch);
            Assert.AreEqual(EKinds.Default, noMatchValue);
        }

        [Test]
        public void IntOperator() {
            int enumValue = (int) EKinds.StageOne;
            EKinds kind = (EKinds) enumValue;

            Assert.AreEqual(kind, EKinds.StageOne);
        }

        [Test]
        public void ToStringTest() {
            string value = EKinds.StageTwo.ToString();
            Assert.AreEqual("StageTwo", value);
        }
    }

    public class EKinds : XEnum<EKinds> {
        
        public static readonly EKinds Default = new EKinds("default");

        public static readonly EKinds StageOne = new EKinds("stageOne");

        public static readonly EKinds StageTwo = new EKinds("stageTwo");
        
        /// <inheritdoc />
        private EKinds(string uniqueReference) : base(uniqueReference) {
            // Nothing to do
        }
    }
}