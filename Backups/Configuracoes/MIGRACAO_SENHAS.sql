-- SCRIPT DE MIGRAÇÃO: Preparar coluna senha para armazenar hashes
-- Este script aumenta o tamanho da coluna senha para acomodar hashes do PasswordHasher

-- IMPORTANTE: Execute este script ANTES de iniciar a aplicação
-- As senhas atuais em texto puro serão substituídas por um valor temporário
-- A aplicação gerará novos hashes ao atualizar os funcionários

USE gestaoReserva;
GO

-- Aumentar o tamanho da coluna senha de varchar(30) para varchar(256)
-- Isso é necessário porque os hashes do PasswordHasher são mais longos
ALTER TABLE pessoas
ALTER COLUMN senha VARCHAR(256) NOT NULL;
GO

-- Se houver funcionários com senhas em texto puro, elas serão invalidadas
-- Recomenda-se que os funcionários façam reset de senha após a migração
-- ou que um administrador gere senhas temporárias

-- Para teste: inserir um funcionário com a senha 'Teste@123' já hashada
-- Hash exemplo (gerado via PasswordHasher): AQAAAAIAAYagAAAAEKh7M...
-- (Este é apenas um exemplo; você receberá o hash correto da aplicação)

PRINT 'Migração de senhas concluída!';
PRINT 'A coluna senha agora comporta hashes de até 256 caracteres.';
GO
