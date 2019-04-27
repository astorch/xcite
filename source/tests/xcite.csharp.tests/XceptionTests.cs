using System;
using NUnit.Framework;
using xcite.csharp.exceptions;

namespace xcite.csharp.tests {
    [TestFixture]
    public class XceptionTests {
        [Test]
        public void ThrowFormatted() {
            Assert.Throws<UTXception>(
                () => throw new UTXception(UTErrorReason.Hard),
                "UTE-1 [Hard]: "
            );
        }
    }

    class UTXception : Xception<UTErrorReason> {
        public UTXception(UTErrorReason errorReason) 
            : base(errorReason) {
            
        }

        public UTXception(UTErrorReason errorReason, string message) 
            : base(errorReason, message) {
            
        }

        public UTXception(UTErrorReason errorReason, string message, Exception innerException) 
            : base(errorReason, message, innerException) {
            
        }
    }
    
    class UTErrorReason : EErrorReason<UTErrorReason> {

        public static readonly UTErrorReason None = new UTErrorReason("None", 0);
        public static readonly UTErrorReason Hard = new UTErrorReason("Hard", 1);
        
        private UTErrorReason(string name, int code) : base(name, code) {
            
        }

        public override string TLC { get; } = "UTE";
    }
}