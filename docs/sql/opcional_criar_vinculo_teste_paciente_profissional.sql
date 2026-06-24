USE db_IF;
GO

/*
    Gismar Pereira Barbosa

    Vínculo de teste entre profissional e paciente.

    Objetivo:
    validar a regra do Trabalho Final que impede a exclusão
    de profissional com pacientes cadastrados.

    Observação:
    Apesar do nome da tabela ser tbMedico_Paciente, ela será usada
    como tabela de vínculo entre profissional e paciente, conforme
    estrutura existente no banco da disciplina.
*/

IF NOT EXISTS (
    SELECT 1
    FROM tbMedico_Paciente
    WHERE IdProfissional = 2
      AND IdPaciente = 1
)
BEGIN
    INSERT INTO tbMedico_Paciente
        (IdPaciente, IdProfissional, InformacaoResumida)
    VALUES
        (1, 2, 'Vínculo de teste para validar bloqueio de exclusão gerencial.');
END
GO

SELECT
    mp.IdMedico_Paciente,
    mp.IdPaciente,
    p.Nome AS NomePaciente,
    mp.IdProfissional,
    pr.Nome AS NomeProfissional,
    mp.InformacaoResumida
FROM tbMedico_Paciente mp
INNER JOIN tbPaciente p ON p.IdPaciente = mp.IdPaciente
INNER JOIN tbProfissional pr ON pr.IdProfissional = mp.IdProfissional
WHERE mp.IdProfissional = 2;
GO
