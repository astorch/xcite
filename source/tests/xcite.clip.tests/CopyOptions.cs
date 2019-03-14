using System;

namespace xcite.clip.tests {
    [Verb("copy", HelpText = "uhuhu")]
    public class CopyOptions : BaseOptions {
        [Option('s', "source", Required = true)]
        public string Source { get; set; }

        [Option('t', "target", Required = true)]
        public string Target { get; set; }

        [Option('o', "override", Required = false, Default = false)]
        public bool Override { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if (!(obj is CopyOptions opt)) return false;

            if (!string.Equals(opt.Source, Source, StringComparison.InvariantCultureIgnoreCase)) return false;
            if (!string.Equals(opt.Target, Target, StringComparison.InvariantCultureIgnoreCase)) return false;
            if (opt.Override != Override) return false;

            if (!string.Equals(opt.Name, Name, StringComparison.InvariantCultureIgnoreCase)) return false;
            if (opt.Port != Port) return false;

            return true;
        }

        /// <inheritdoc />
        public override string ToString() {
            return $"copy --source {Source} --target {Target} --override {Override} --name {Name} --port {Port}";
        }
    }
}