using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Projeto1_IF.Areas.Identity.Pages.Account;

// Gismar Pereira Barbosa
//
// PageModel criado para a página AccessDenied customizada.
//
// Esta página é usada pelo ASP.NET Core Identity quando um usuário
// autenticado tenta acessar uma rota para a qual não possui permissão.
//
// A lógica fica simples porque a finalidade da página é apenas apresentar
// uma mensagem amigável em português.
public class AccessDeniedModel : PageModel
{
    public void OnGet()
    {
    }
}