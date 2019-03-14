using System;
using System.Collections;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace xcite.clip.tests {
    [TestFixture]
    public class ParserTests {
        [Test]
        public void PrintUsageNoVerb() {
            string usage;
            using (StringWriter strWtr = new StringWriter(new StringBuilder(1000))) {
                ReflectionKit.InvokeStaticMethod(typeof(Parser), "PrintUsageWithError",
                    new[] {
                        typeof(TextWriter),
                        typeof(string),
                        typeof(Type[]),
                        typeof(VerbInfo)
                    }, 
                    new object[] {
                        strWtr,
                        "No verb selected.",
                        new[] {typeof(UnpackOptions), typeof(CopyOptions)},
                        null
                    });
                usage = strWtr.ToString();
            }
            
            Assert.IsNotNull(usage);
            Assert.IsNotEmpty(usage);

            Assert.IsTrue(usage.Contains("xcite.clip.tests 1.0.0.0"));
            Assert.IsTrue(usage.Contains("Copyright ©  2019"));
            Assert.IsTrue(usage.Contains("ERROR(S)"));
            Assert.IsTrue(usage.Contains("No verb selected."));
            Assert.IsTrue(usage.Contains("ARGUMENT(S)"));
            Assert.IsTrue(usage.Contains("  unpack     bla bla bla"));
            Assert.IsTrue(usage.Contains("    copy     uhuhu"));
            Assert.IsTrue(usage.Contains("Unknown action. Check help!"));
        }

        [Test]
        public void PrintUsageWithVerb() {
            string usage;
            using (StringWriter strWtr = new StringWriter(new StringBuilder(1000))) {
                VerbInfo verbNfo = (VerbInfo) ReflectionKit.InvokeStaticMethod(typeof(Parser), "GetVerb",
                    new[] {typeof(string), typeof(Type[])},
                    new object[] {
                        "unpack",
                        new[] {typeof(UnpackOptions), typeof(CopyOptions)}
                    });

                ReflectionKit.InvokeStaticMethod(typeof(Parser), "PrintUsageWithError",
                    new[] {
                        typeof(TextWriter),
                        typeof(string),
                        typeof(Type[]),
                        typeof(VerbInfo)
                    },
                    new object[] {
                        strWtr,
                        "Missing required argument.",
                        new[] {typeof(UnpackOptions), typeof(CopyOptions)},
                        verbNfo, 
                    });
                usage = strWtr.ToString();
            }

            Assert.IsNotNull(usage);
            Assert.IsNotEmpty(usage);

            Assert.IsTrue(usage.Contains("xcite.clip.tests 1.0.0.0"));
            Assert.IsTrue(usage.Contains("Copyright ©  2019"));
            Assert.IsTrue(usage.Contains("ERROR(S)"));
            Assert.IsTrue(usage.Contains("Missing required argument."));
            Assert.IsTrue(usage.Contains("ARGUMENT(S)"));
            Assert.IsTrue(usage.Contains("      -f, --file     (Required) Source path"));
            Assert.IsTrue(usage.Contains("      -d, --dest     (Required) Destination path"));
            Assert.IsTrue(usage.Contains("  -o, --override     Override flag"));
            Assert.IsTrue(usage.Contains("      -m, --Mode     (Default Normal) Unpack mode"));
            Assert.IsTrue(usage.Contains("      -n, --name     (Required) Name of the machine to address"));
            Assert.IsTrue(usage.Contains("      -p, --port     (Default 9898) Port of the machine to address"));
            Assert.IsTrue(usage.Contains("Missing arguments. Check help!"));
        }

        [Test]
        public void ParseNoVerb() {
            string argLine = "-f -v 250";
            string[] args = argLine.Split(' ');
            string verb = null;
            FlipArguments flipArgs = null;
            bool result = Parser.Run(args, (s, o) => {
                verb = s;
                flipArgs = (FlipArguments) o;
            }, typeof(FlipArguments));

            Assert.IsTrue(result);
            Assert.IsNull(verb);

            Assert.IsNotNull(flipArgs);
            Assert.AreEqual(true, flipArgs.Flip);
            Assert.AreEqual(250, flipArgs.Value);
        }

        [Test, TestCaseSource(typeof(ParseTestCaseSource))]
        public BaseOptions ParseVerbs(string argLine) {
            string[] args = argLine.Split(' ');
            string verb = null;
            BaseOptions opts = null;
            bool result = Parser.Run(args, (s, o) => {
                verb = s;
                opts = (BaseOptions) o;
            }, typeof(UnpackOptions), typeof(CopyOptions));
            Assert.IsTrue(result);
            Assert.IsNotNull(verb);
            return opts;
        }

        class ParseTestCaseSource : IEnumerable {
            /// <inheritdoc />
            public IEnumerator GetEnumerator() {
                yield return new TestCaseData("unpack -m fast --file \"c:\\staging\\zip\\EGUB_Suite20190312.zip\" --dest \"c:\\EGUB-Suite\\\" --override -n localhost")
                    .Returns(new UnpackOptions {
                        File = "c:\\staging\\zip\\EGUB_Suite20190312.zip",
                        Dest = "c:\\EGUB-Suite\\",
                        Override = true,
                        Name = "localhost",
                        Port = 9898,
                        Mode = EMode.Fast
                    });

                yield return new TestCaseData("copy --source \"$assets\\EGUB_Suite20190312.zip\" --target \"c:\\staging\\zip\\EGUB_Suite20190312.zip\" -n localhost -p 1010")
                    .Returns(new CopyOptions {
                        Source = "$assets\\EGUB_Suite20190312.zip",
                        Target = "c:\\staging\\zip\\EGUB_Suite20190312.zip",
                        Override = false,
                        Name = "localhost",
                        Port = 1010
                    });
            }
        }
    }
}
