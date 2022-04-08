namespace xcite.nio.tests {
    [NetService]
    public interface ITestService {

        bool IsAvailable();

        AuthResult Authenticate(string userName, string pass);

        string GetLocation(IAuthInfo authNfo);
    }
}