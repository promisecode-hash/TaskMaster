using TaskMaster.Api.Security;

namespace TaskMaster.Api.Services;

public interface IJwtTokenService
{
    string CreateToken(string username, string email, string name);
}
