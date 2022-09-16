
using Application.Common.Cryptography;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using System.Text.Json;

namespace Application.Common.Functions;

internal static class Functions
{
   
    public static bool ValidarClave ( string claveUsuario, string claveBase )
    {
        return BCrypt.Net.BCrypt.Verify(claveUsuario, claveBase);
    }


}
