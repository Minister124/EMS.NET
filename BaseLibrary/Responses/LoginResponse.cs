namespace BaseLibrary.Responses
{
    //Throw Responses with Tokens in Login
    public record LoginResponse(
        bool Flag,
        string Message=null!,
        string Token=null!,
        string RefreshToken=null!
        );
}
