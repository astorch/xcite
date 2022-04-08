namespace xcite.clip.tests; 

[Verb("unpack", HelpText = "bla bla bla")]
public class UnpackOptions : BaseOptions {
    [Option('f', "file", Required = true, Default = null,
        HelpText = "Source path")]
    public string File { get; set; }

    [Option('d', "dest", Required = true, Default = null,
        HelpText = "Destination path")]
    public string Dest { get; set; }

    [Option('o', "override",
        HelpText = "Override flag")]
    public bool Override { get; set; }

    [Option('m', "Mode", Required = false, Default = EMode.Normal,
        HelpText = "Unpack mode")]
    public EMode Mode { get; set; }

    /// <inheritdoc />
    public override bool Equals(object obj) {
        if (!(obj is UnpackOptions opt)) return false;

        if (!string.Equals(opt.Dest, Dest, StringComparison.InvariantCultureIgnoreCase)) return false;
        if (!string.Equals(opt.File, File, StringComparison.InvariantCultureIgnoreCase)) return false;
        if (opt.Override != Override) return false;
        if (opt.Mode != Mode) return false;

        if (!string.Equals(opt.Name, Name, StringComparison.InvariantCultureIgnoreCase)) return false;
        if (opt.Port != Port) return false;

        return true;
    }

    /// <inheritdoc />
    public override string ToString() {
        return $"unpack --file {File} --dest {Dest} --override {Override} --name {Name} --port {Port} --mode {Mode}";
    }
}