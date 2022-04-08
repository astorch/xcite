namespace xcite.clip.tests; 

[Verb("detail", HelpText = "This provides many details for a program.")]
public class DetailOptions {
    
    [Option('x', "ex", HelpText = "Might do something. Don't known.")]
    public string OptionOne { get; set; }
    
    [
        Option('y', "ypsilon", HelpText = "This really does something. So it's heavily recommended to activate this option,\n" +
                                          "so that you can see some awesome features that you would have never expected. If you have\n" +
                                          "some further questions, please ask the programmer.",
        Required = true
        )
    ]
    public string OptionTwo { get; set; }
    
}