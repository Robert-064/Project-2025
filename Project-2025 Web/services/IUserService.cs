namespace Project_2025_Web.Services
{
    public interface IUserService
    {
        int? GetUserId();
        string GetUserEmail();
        bool IsAuthenticated();
    }
}
