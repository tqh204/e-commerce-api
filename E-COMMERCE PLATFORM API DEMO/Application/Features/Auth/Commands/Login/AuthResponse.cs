using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands.Login
{
    public record AuthResponse(string AccessToken, string role);
    
    
}
