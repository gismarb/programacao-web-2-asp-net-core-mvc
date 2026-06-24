USE db_IF;
GO

/*
    Gismar Pereira Barbosa
    
    Consultas de apoio para testes funcionais da Parte 3

    Objetivo:
    Disponibilizar consultas SQL auxiliares para conferir os dados
    gerados durante os testes do Trabalho Final de Programação Web II.

    Observação:
    Este script não altera dados no banco. Ele apenas consulta informações
    relacionadas a usuários, Roles, profissionais, contratos, pacientes
    e vínculos em tbMedico_Paciente.
*/

------------------------------------------------------------
-- 1. Conferir Roles cadastradas no ASP.NET Core Identity
------------------------------------------------------------

SELECT
    Id,
    Name,
    NormalizedName
FROM AspNetRoles
ORDER BY Name;
GO

------------------------------------------------------------
-- 2. Conferir usuários gerenciais e suas Roles
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

------------------------------------------------------------
-- 3. Conferir profissionais cadastrados, tipo, plano e Role
------------------------------------------------------------

SELECT
    p.IdProfissional,
    p.Nome,
    p.CPF,
    p.IdTipoProfissional,
    CASE
        WHEN p.IdTipoProfissional = 1 THEN 'Médico'
        WHEN p.IdTipoProfissional = 2 THEN 'Nutricionista'
        ELSE 'Não identificado'
    END AS TipoProfissional,
    p.IdUser,
    u.Email,
    r.Name AS Role,
    p.IdContrato,
    pl.Nome AS Plano,
    p.IdTipoAcesso
FROM tbProfissional p
INNER JOIN AspNetUsers u ON u.Id = p.IdUser
LEFT JOIN AspNetUserRoles ur ON ur.UserId = u.Id
LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
INNER JOIN tbContrato c ON c.IdContrato = p.IdContrato
INNER JOIN tbPlano pl ON pl.IdPlano = c.IdPlano
ORDER BY p.IdProfissional;
GO

------------------------------------------------------------
-- 4. Conferir um profissional específico pelo e-mail de login
--    Ajuste o valor da variável conforme o teste executado.
------------------------------------------------------------

DECLARE @EmailProfissional varchar(256) = 'medico.final@if.com';

SELECT
    p.IdProfissional,
    p.Nome,
    p.CPF,
    p.IdTipoProfissional,
    CASE
        WHEN p.IdTipoProfissional = 1 THEN 'Médico'
        WHEN p.IdTipoProfissional = 2 THEN 'Nutricionista'
        ELSE 'Não identificado'
    END AS TipoProfissional,
    u.Email,
    r.Name AS Role,
    pl.Nome AS Plano,
    c.DataInicio,
    c.DataFim
FROM tbProfissional p
INNER JOIN AspNetUsers u ON u.Id = p.IdUser
LEFT JOIN AspNetUserRoles ur ON ur.UserId = u.Id
LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
INNER JOIN tbContrato c ON c.IdContrato = p.IdContrato
INNER JOIN tbPlano pl ON pl.IdPlano = c.IdPlano
WHERE u.Email = @EmailProfissional;
GO

------------------------------------------------------------
-- 5. Conferir pacientes existentes no banco
------------------------------------------------------------

SELECT
    IdPaciente,
    Nome,
    CPF,
    RG,
    DataNascimento,
    Sexo,
    IdCidade
FROM tbPaciente
ORDER BY IdPaciente DESC;
GO

------------------------------------------------------------
-- 6. Conferir vínculos entre profissional e paciente
------------------------------------------------------------

SELECT
    mp.IdMedico_Paciente,
    mp.IdPaciente,
    pac.Nome AS NomePaciente,
    mp.IdProfissional,
    prof.Nome AS NomeProfissional,
    u.Email AS EmailProfissional,
    mp.InformacaoResumida
FROM tbMedico_Paciente mp
INNER JOIN tbPaciente pac ON pac.IdPaciente = mp.IdPaciente
INNER JOIN tbProfissional prof ON prof.IdProfissional = mp.IdProfissional
INNER JOIN AspNetUsers u ON u.Id = prof.IdUser
ORDER BY mp.IdMedico_Paciente DESC;
GO

------------------------------------------------------------
-- 7. Conferir pacientes vinculados a um profissional específico
--    Ajuste o e-mail conforme o usuário profissional testado.
------------------------------------------------------------

DECLARE @EmailProfissionalPacientes varchar(256) = 'medico2.teste@if.com';

SELECT
    mp.IdMedico_Paciente,
    pac.IdPaciente,
    pac.Nome AS NomePaciente,
    pac.CPF,
    pac.RG,
    pac.DataNascimento,
    pac.Sexo,
    cid.Nome AS Cidade,
    pac.TelCelular,
    mp.InformacaoResumida,
    prof.IdProfissional,
    prof.Nome AS NomeProfissional,
    u.Email AS EmailProfissional
FROM tbMedico_Paciente mp
INNER JOIN tbPaciente pac ON pac.IdPaciente = mp.IdPaciente
INNER JOIN tbProfissional prof ON prof.IdProfissional = mp.IdProfissional
INNER JOIN AspNetUsers u ON u.Id = prof.IdUser
LEFT JOIN tbCidade cid ON cid.IdCidade = pac.IdCidade
WHERE u.Email = @EmailProfissionalPacientes
ORDER BY pac.Nome;
GO

------------------------------------------------------------
-- 8. Conferir profissionais sem vínculo com pacientes
--    Útil para testar exclusão gerencial permitida.
------------------------------------------------------------

SELECT
    p.IdProfissional,
    p.Nome,
    u.Email,
    CASE
        WHEN p.IdTipoProfissional = 1 THEN 'Médico'
        WHEN p.IdTipoProfissional = 2 THEN 'Nutricionista'
        ELSE 'Não identificado'
    END AS TipoProfissional
FROM tbProfissional p
INNER JOIN AspNetUsers u ON u.Id = p.IdUser
WHERE NOT EXISTS (
    SELECT 1
    FROM tbMedico_Paciente mp
    WHERE mp.IdProfissional = p.IdProfissional
)
ORDER BY p.IdProfissional;
GO

------------------------------------------------------------
-- 9. Conferir profissionais com vínculo com pacientes
--    Útil para testar bloqueio de exclusão gerencial.
------------------------------------------------------------

SELECT
    p.IdProfissional,
    p.Nome,
    u.Email,
    CASE
        WHEN p.IdTipoProfissional = 1 THEN 'Médico'
        WHEN p.IdTipoProfissional = 2 THEN 'Nutricionista'
        ELSE 'Não identificado'
    END AS TipoProfissional,
    COUNT(mp.IdMedico_Paciente) AS TotalVinculosPaciente
FROM tbProfissional p
INNER JOIN AspNetUsers u ON u.Id = p.IdUser
INNER JOIN tbMedico_Paciente mp ON mp.IdProfissional = p.IdProfissional
GROUP BY
    p.IdProfissional,
    p.Nome,
    u.Email,
    p.IdTipoProfissional
ORDER BY p.IdProfissional;
GO

------------------------------------------------------------
-- 10. Conferir se um paciente removido da lista do profissional
--     continua preservado em tbPaciente.
--     Ajuste o nome conforme o paciente usado no teste.
------------------------------------------------------------

DECLARE @NomePaciente varchar(100) = '%Pablo%';

SELECT
    IdPaciente,
    Nome,
    CPF,
    RG
FROM tbPaciente
WHERE Nome LIKE @NomePaciente
ORDER BY IdPaciente DESC;
GO
