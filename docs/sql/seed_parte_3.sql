USE db_IF;
GO

/* 
    Seed inicial da Parte 3
    Gismar Pereira Barbosa

    Objetivo:
    Inserir dados mínimos para funcionamento do trabalho final,
    sem alterar a estrutura do banco de dados.
*/

------------------------------------------------------------
-- 1. Roles do ASP.NET Core Identity
------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'MEDICO')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Medico', 'MEDICO', NEWID());
END
GO

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'NUTRICIONISTA')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Nutricionista', 'NUTRICIONISTA', NEWID());
END
GO

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'GERENTEMEDICO')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'GerenteMedico', 'GERENTEMEDICO', NEWID());
END
GO

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'GERENTENUTRICIONISTA')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'GerenteNutricionista', 'GERENTENUTRICIONISTA', NEWID());
END
GO

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'GERENTEGERAL')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'GerenteGeral', 'GERENTEGERAL', NEWID());
END
GO

------------------------------------------------------------
-- 2. Tipos de profissional
------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM tbTipoProfissional WHERE Nome = 'Médico')
BEGIN
    INSERT INTO tbTipoProfissional (Nome)
    VALUES ('Médico');
END
GO

IF NOT EXISTS (SELECT 1 FROM tbTipoProfissional WHERE Nome = 'Nutricionista')
BEGIN
    INSERT INTO tbTipoProfissional (Nome)
    VALUES ('Nutricionista');
END
GO

------------------------------------------------------------
-- 3. Planos
-- Validade em dias; valores definidos para ambiente acadêmico
------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM tbPlano WHERE Nome = 'Médico Total')
BEGIN
    INSERT INTO tbPlano (Nome, Validade, Valor)
    VALUES ('Médico Total', 365, 199.90);
END
GO

IF NOT EXISTS (SELECT 1 FROM tbPlano WHERE Nome = 'Médico Parcial')
BEGIN
    INSERT INTO tbPlano (Nome, Validade, Valor)
    VALUES ('Médico Parcial', 180, 99.90);
END
GO

IF NOT EXISTS (SELECT 1 FROM tbPlano WHERE Nome = 'Nutricionista Total')
BEGIN
    INSERT INTO tbPlano (Nome, Validade, Valor)
    VALUES ('Nutricionista Total', 365, 149.90);
END
GO

IF NOT EXISTS (SELECT 1 FROM tbPlano WHERE Nome = 'Nutricionista Parcial')
BEGIN
    INSERT INTO tbPlano (Nome, Validade, Valor)
    VALUES ('Nutricionista Parcial', 180, 79.90);
END
GO

------------------------------------------------------------
-- 4. Tipos de acesso
------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM tbTipoAcesso WHERE Nome = 'Profissional')
BEGIN
    INSERT INTO tbTipoAcesso (Nome, FlagAtivo)
    VALUES ('Profissional', 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM tbTipoAcesso WHERE Nome = 'Gerente')
BEGIN
    INSERT INTO tbTipoAcesso (Nome, FlagAtivo)
    VALUES ('Gerente', 1);
END
GO

------------------------------------------------------------
-- 5. Conferência dos dados inseridos
------------------------------------------------------------

SELECT Id, Name, NormalizedName
FROM AspNetRoles
ORDER BY Name;
GO

SELECT *
FROM tbTipoProfissional
ORDER BY Nome;
GO

SELECT *
FROM tbPlano
ORDER BY Nome;
GO

SELECT *
FROM tbTipoAcesso
ORDER BY Nome;
GO