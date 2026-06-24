USE db_IF;
GO

/*
    Gismar Pereira Barbosa

    Remoção opcional de vínculo de teste entre profissional e paciente

    Objetivo:
    Remover um vínculo criado manualmente em tbMedico_Paciente durante
    os testes funcionais da Parte 3.

    Cenário de uso:
    Este script pode ser utilizado após validar a regra que bloqueia
    a exclusão gerencial de profissional com paciente vinculado.

    Observação:
    Este script remove apenas o vínculo entre profissional e paciente.
    O cadastro do paciente em tbPaciente e o cadastro do profissional
    em tbProfissional são preservados.

    Atenção:
    Ajuste os valores de @IdProfissional e @IdPaciente antes de executar.
*/

------------------------------------------------------------
-- 1. Parâmetros do vínculo de teste
------------------------------------------------------------

DECLARE @IdProfissional int = 2;
DECLARE @IdPaciente int = 1;

------------------------------------------------------------
-- 2. Conferência antes da remoção
------------------------------------------------------------

SELECT
    mp.IdMedico_Paciente,
    mp.IdPaciente,
    pac.Nome AS NomePaciente,
    mp.IdProfissional,
    prof.Nome AS NomeProfissional,
    mp.InformacaoResumida
FROM tbMedico_Paciente mp
INNER JOIN tbPaciente pac ON pac.IdPaciente = mp.IdPaciente
INNER JOIN tbProfissional prof ON prof.IdProfissional = mp.IdProfissional
WHERE mp.IdProfissional = @IdProfissional
  AND mp.IdPaciente = @IdPaciente;
GO

------------------------------------------------------------
-- 3. Remoção do vínculo de teste
------------------------------------------------------------

DECLARE @IdProfissional int = 2;
DECLARE @IdPaciente int = 1;

DELETE FROM tbMedico_Paciente
WHERE IdProfissional = @IdProfissional
  AND IdPaciente = @IdPaciente;
GO

------------------------------------------------------------
-- 4. Conferência após a remoção
------------------------------------------------------------

DECLARE @IdProfissional int = 2;
DECLARE @IdPaciente int = 1;

SELECT
    mp.IdMedico_Paciente,
    mp.IdPaciente,
    pac.Nome AS NomePaciente,
    mp.IdProfissional,
    prof.Nome AS NomeProfissional,
    mp.InformacaoResumida
FROM tbMedico_Paciente mp
INNER JOIN tbPaciente pac ON pac.IdPaciente = mp.IdPaciente
INNER JOIN tbProfissional prof ON prof.IdProfissional = mp.IdProfissional
WHERE mp.IdProfissional = @IdProfissional
  AND mp.IdPaciente = @IdPaciente;
GO
