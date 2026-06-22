USE db_IF;
GO

/*
    Associação dos usuários gerentes às suas Roles
    Gismar Pereira Barbosa

    Observação:
    Os usuários foram criados pela tela Register da aplicação,
    para que o hash de senha fosse gerado corretamente pelo ASP.NET Core Identity.
    Aqui, é feito apenas o vínculo User x Role diretamente no banco.
*/

------------------------------------------------------------
-- Gerente Médico
------------------------------------------------------------

INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u
INNER JOIN AspNetRoles r ON r.NormalizedName = 'GERENTEMEDICO'
WHERE u.NormalizedEmail = 'GERENTE.MEDICO@IF.COM'
  AND NOT EXISTS (
      SELECT 1
      FROM AspNetUserRoles ur
      WHERE ur.UserId = u.Id
        AND ur.RoleId = r.Id
  );
GO

------------------------------------------------------------
-- Gerente Nutricionista
------------------------------------------------------------

INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u
INNER JOIN AspNetRoles r ON r.NormalizedName = 'GERENTENUTRICIONISTA'
WHERE u.NormalizedEmail = 'GERENTE.NUTRICIONISTA@IF.COM'
  AND NOT EXISTS (
      SELECT 1
      FROM AspNetUserRoles ur
      WHERE ur.UserId = u.Id
        AND ur.RoleId = r.Id
  );
GO

------------------------------------------------------------
-- Gerente Geral
------------------------------------------------------------

INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u
INNER JOIN AspNetRoles r ON r.NormalizedName = 'GERENTEGERAL'
WHERE u.NormalizedEmail = 'GERENTE.GERAL@IF.COM'
  AND NOT EXISTS (
      SELECT 1
      FROM AspNetUserRoles ur
      WHERE ur.UserId = u.Id
        AND ur.RoleId = r.Id
  );
GO

------------------------------------------------------------
-- Conferência
------------------------------------------------------------

SELECT 
    u.Email,
    r.Name AS Role
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON ur.UserId = u.Id
INNER JOIN AspNetRoles r ON r.Id = ur.RoleId
WHERE u.Email IN (
    'gerente.medico@if.com',
    'gerente.nutricionista@if.com',
    'gerente.geral@if.com'
)
ORDER BY u.Email;
GO