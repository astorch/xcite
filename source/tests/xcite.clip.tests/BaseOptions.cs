namespace xcite.clip.tests; 

public abstract class BaseOptions {
    [Option('n', "name", Required = true, Default = null,
        HelpText = "Name of the machine to address")]
    public string Name { get; set; }

    [Option('p', "port", Required = false, Default = 9898,
        HelpText = "Port of the machine to address")]
    public int Port { get; set; }
}