namespace xcite.clip.tests; 

public class FlipArguments {
    [Option('f')]
    public bool Flip { get; set; }

    [Option('v', Required = true, Default = 100)]
    public int Value { get; set; }
}