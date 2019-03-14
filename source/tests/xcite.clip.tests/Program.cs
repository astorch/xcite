using System;

namespace xcite.clip.tests {
    public class Program {
        public static void Main(string[] args) {
            bool result = Parser.Run(args, OnSuccess, 
                typeof(UnpackOptions), typeof(CopyOptions));
        }

        private static void OnSuccess(string arg1, object arg2) {
            Console.WriteLine();
        }
    }
}