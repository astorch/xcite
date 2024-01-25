using System.Collections;
using System.Text;
using NUnit.Framework;

namespace xcite.clip.tests {
    [TestFixture]
    public class ParserTests {
        [OneTimeSetUp]
        public void OnSetUp() {
            Parser.MarvelMode = false;
        }

        [Test]
        public void PrintUsageNoVerb() {
            string usage;
            using (StringWriter strWtr = new(new StringBuilder(1000))) {
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
            
            Assert.That(usage, Is.Not.Null);
            Assert.That(usage, Is.Not.Empty);
            
            // Assert.IsTrue(usage.Contains("xcite.clip.tests 1.0.0.0"));
            Assert.That(usage, Contains.Substring("ReSharperTestRunner 2.11.1.102"));
            // Assert.IsTrue(usage.Contains("Copyright ©  2019"));
            Assert.That(usage, Contains.Substring("ERROR(S)"));
            Assert.That(usage, Contains.Substring("No verb selected."));
            Assert.That(usage, Contains.Substring("ARGUMENT(S)"));
            Assert.That(usage, Contains.Substring("  unpack     bla bla bla"));
            Assert.That(usage, Contains.Substring("    copy     uhuhu"));
            Assert.That(usage, Contains.Substring("Unknown action. Check help!"));
        }

        [Test]
        public void PrintUsageWithVerb() {
            string usage;
            using (StringWriter strWtr = new(new StringBuilder(1000))) {
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

            Assert.That(usage, Is.Not.Null);
            Assert.That(usage, Is.Not.Empty);
            
            // Assert.IsTrue(usage.Contains("xcite.clip.tests 1.0.0.0"));
            Assert.That(usage, Contains.Substring("ReSharperTestRunner 2.11.1.102"));
            // Assert.IsTrue(usage.Contains("Copyright ©  2019"));
            Assert.That(usage, Contains.Substring("ERROR(S)"));
            Assert.That(usage, Contains.Substring("Missing required argument."));
            Assert.That(usage, Contains.Substring("ARGUMENT(S)"));
            Assert.That(usage, Contains.Substring("      -f, --file     (Required) Source path"));
            Assert.That(usage, Contains.Substring("      -d, --dest     (Required) Destination path"));
            Assert.That(usage, Contains.Substring("  -o, --override     Override flag"));
            Assert.That(usage, Contains.Substring("      -m, --Mode     (Default Normal) Unpack mode"));
            Assert.That(usage, Contains.Substring("      -n, --name     (Required) Name of the machine to address"));
            Assert.That(usage, Contains.Substring("      -p, --port     (Default 9898) Port of the machine to address"));
            Assert.That(usage, Contains.Substring("Missing arguments. Check help!"));
        }

        [Test]
        public void PrintUsageWithVerbAndLongDescription() {
            string usage;
            using (StringWriter strWtr = new(new StringBuilder(1000))) {
                VerbInfo verbNfo = (VerbInfo) ReflectionKit.InvokeStaticMethod(typeof(Parser), "GetVerb",
                    new[] {typeof(string), typeof(Type[])},
                    new object[] {
                        "detail",
                        new[] {typeof(DetailOptions), typeof(UnpackOptions)}
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

            Assert.That(usage, Is.Not.Null);
            Assert.That(usage, Is.Not.Empty);

            // Assert.IsTrue(usage.Contains("xcite.clip.tests 1.0.0.0"));
            Assert.That(usage, Contains.Substring("ReSharperTestRunner 2.11.1.102"));
            // Assert.IsTrue(usage.Contains("Copyright ©  2019"));
            Assert.That(usage, Contains.Substring("ERROR(S)"));
            Assert.That(usage, Contains.Substring("Missing required argument."));
            Assert.That(usage, Contains.Substring("ARGUMENT(S)"));
            Assert.That(usage, Contains.Substring("       -x, --ex     Might do something. Don't known."));
            Assert.That(usage, Contains.Substring("  -y, --ypsilon     (Required) This really does something. So it's heavily recommended to activate this option,"));
            Assert.That(usage, Contains.Substring("                    so that you can see some awesome features that you would have never expected. If you have"));
            Assert.That(usage, Contains.Substring("                    some further questions, please ask the programmer."));
            Assert.That(usage, Contains.Substring("Missing arguments. Check help!"));
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

            Assert.That(result, Is.True);
            Assert.That(verb, Is.Null);
            
            Assert.That(flipArgs, Is.Not.Null);
            Assert.That(flipArgs.Flip, Is.True);
            Assert.That(flipArgs.Value, Is.EqualTo(250));
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
            Assert.That(result, Is.True);
            Assert.That(verb, Is.Not.Null);
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

//                yield return new TestCaseData("unpack")
//                    .Returns(null);

                yield return new TestCaseData("unpack --file \"c:\\\" --dest \"c:\\\" -n localhost")
                    .Returns(new UnpackOptions {
                        File = "c:\\", Dest = "c:\\", Name = "localhost",
                        Port = 9898
                    });
            }
        }
    }
}
