namespace BaseLibrary.Responses
{
    //Throw Responses in General Methods of API
    public record GeneralResponse(
        bool Flag,
        string Message=null!
        );
}
