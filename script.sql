/* =====================================================================
   SANTA MASSA - GESTÃO DE RESERVAS
   Script de criação do banco gestaoReserva (SQL Server).
   ATENÇÃO: apaga e recria o banco gestaoReserva (todos os dados são perdidos).
   ===================================================================== */

use master;
go

-- Se o banco já existir, desconecta os usuários e apaga
if db_id('gestaoReserva') is not null
begin
    alter database gestaoReserva set single_user with rollback immediate;
    drop database gestaoReserva;
end
go

-- Cria o banco vazio
create database gestaoReserva;
go

use gestaoReserva;
go

/* ---------------------------------------------------------------------
   PESSOAS: dados comuns de clientes e funcionários.
   A coluna senha não é mais usada no login (fica vazia nos novos cadastros);
   cada papel tem sua própria senha_hash em clientes/funcionarios.
   --------------------------------------------------------------------- */
create table pessoas
(
    id    int          not null identity(1,1),  -- código gerado automaticamente
    nome  varchar(50)  not null,                 -- nome completo
    cpf   varchar(20)  not null,                 -- CPF (único)
    senha varchar(256) not null,                 -- legado: não usada no login
    constraint pk_pessoas     primary key (id),
    constraint uq_pessoas_cpf unique (cpf)
);
go

/* ---------------------------------------------------------------------
   CLIENTES: pessoa que faz reservas. Senha própria (hash do PasswordHasher).
   --------------------------------------------------------------------- */
create table clientes
(
    pessoa_id     int          not null,         -- mesma chave da pessoa
    email_pessoal varchar(100) not null,         -- e-mail (único)
    telefone      varchar(20)  not null,         -- telefone (único)
    senha_hash    varchar(256) null,             -- senha do cliente (hash)
    constraint pk_clientes          primary key (pessoa_id),
    constraint uq_clientes_email    unique (email_pessoal),
    constraint uq_clientes_telefone unique (telefone),
    constraint fk_cliente_id        foreign key (pessoa_id) references pessoas(id)
);
go

/* ---------------------------------------------------------------------
   FUNCIONARIOS: pessoa que acessa a área do restaurante. Senha própria (hash).
   --------------------------------------------------------------------- */
create table funcionarios
(
    pessoa_id           int          not null,   -- mesma chave da pessoa
    email_institucional varchar(50)  not null,   -- e-mail institucional (único)
    senha_hash          varchar(256) null,       -- senha do funcionário (hash)
    constraint pk_funcionarios       primary key (pessoa_id),
    constraint uq_funcionarios_email unique (email_institucional),
    constraint fk_funcionario_id     foreign key (pessoa_id) references pessoas(id)
);
go

/* ---------------------------------------------------------------------
   AMBIENTES: áreas do restaurante (Interno, Externo, Sacada...).
   O código é gerado pela aplicação (MAX + 1).
   --------------------------------------------------------------------- */
create table ambientes
(
    codigo     int         not null,             -- código do ambiente
    nome       varchar(50) not null,             -- nome exibido no mapa
    status     varchar(20) null,                 -- situação do ambiente
    capacidade int         not null,             -- capacidade de pessoas
    constraint pk_ambientes primary key (codigo)
);
go

/* ---------------------------------------------------------------------
   MESAS: nr é a chave (igual ao número na criação); status 'Disponivel'
   no banco aparece como 'Livre' nas telas.
   --------------------------------------------------------------------- */
create table mesas
(
    nr               int         not null,       -- chave da mesa
    numero           int         not null,       -- número exibido
    status           varchar(20) not null,       -- Disponivel / Ocupada / Reservada
    qtd_lugares      int         not null constraint df_mesas_qtd_lugares default 0, -- lugares
    codigo_ambientes int         not null,       -- ambiente da mesa
    constraint pk_mesas            primary key (nr),
    constraint ck_mesas_status     check (status in ('Disponivel', 'Ocupada', 'Reservada')),
    constraint fk_codigo_ambientes foreign key (codigo_ambientes) references ambientes(codigo)
);
go

/* ---------------------------------------------------------------------
   RESERVAS: status 0 = Pendente, 1 = Confirmada, 2 = Cancelada.
   mesas_nr aceita nulo (mesa excluída em reserva cancelada).
   data_hora guarda só o dia (meia-noite).
   --------------------------------------------------------------------- */
create table reservas
(
    numero      int      not null identity(1,1), -- número da reserva
    qtd_pessoas int      not null,               -- quantidade de pessoas
    data_hora   datetime not null constraint df_reservas_data_hora default getdate(), -- dia da reserva
    status      int      null,                   -- 0, 1 ou 2
    mesas_nr    int      null,                   -- mesa reservada
    cliente_id  int      not null,               -- cliente da reserva
    constraint pk_reservas             primary key (numero),
    constraint ck_reservas_qtd_pessoas check (qtd_pessoas > 0),
    constraint ck_reservas_status      check (status in (0, 1, 2)),
    constraint fk_reservas_cliente_id  foreign key (cliente_id) references clientes(pessoa_id),
    constraint fk_mesas_nr             foreign key (mesas_nr)   references mesas(nr)
);
go

/* ---------------------------------------------------------------------
   RESERVAS_MESAS: ligação entre reserva e mesa (mantida pela aplicação).
   --------------------------------------------------------------------- */
create table Reservas_Mesas
(
    numero_reservas int not null,                -- reserva
    nr_mesas        int not null,                -- mesa
    constraint pk_reservas_mesas  primary key (numero_reservas, nr_mesas),
    constraint fk_numero_reservas foreign key (numero_reservas) references reservas(numero),
    constraint fk_nr_mesas        foreign key (nr_mesas)        references mesas(nr)
);
go

/* =====================================================================
   DADOS INICIAIS
   Senhas de teste: funcionária Func@123 / cliente Cliente@123
   (hashes gerados com o PasswordHasher do ASP.NET Core)
   ===================================================================== */

-- Funcionária de exemplo (id 1)
insert into pessoas (nome, cpf, senha)
values (N'Ana Cristina', N'111.222.333-44', N'');

insert into funcionarios (pessoa_id, email_institucional, senha_hash)
values (1, N'ana@santamassa.com.br',
        N'AQAAAAIAAYagAAAAEKp7euPr5/KXP4GuDfFACefG4UDZTTiJVKnlvjbzuknMXz7tlW8ez1lw1HlDUXe4Eg==');
go

-- Cliente de exemplo (id 2)
insert into pessoas (nome, cpf, senha)
values (N'Joao Mendes', N'999.888.777-66', N'');

insert into clientes (pessoa_id, email_pessoal, telefone, senha_hash)
values (2, N'joao@email.com', N'(17) 99999-0000',
        N'AQAAAAIAAYagAAAAECaBvHCmvwXt77eqfe2VkO8lgn7MJNVQNVwcdjCrAnnO9TQQzo92HEJpKLCU6i+4jw==');
go

-- Ambientes usados no mapa de mesas
insert into ambientes (codigo, nome, status, capacidade)
values (1, N'Interno', N'Disponivel', 80),
       (2, N'Externo', N'Disponivel', 48),
       (3, N'Sacada',  N'Disponivel', 32);
go

-- Interno: 20 mesas (nr 1 a 20)
;with nums as (select 1 as n union all select n + 1 from nums where n < 20)
insert into mesas (nr, numero, status, qtd_lugares, codigo_ambientes)
select n, n, N'Disponivel', case when n % 2 = 0 then 2 else 4 end, 1
from nums
option (maxrecursion 20);
go

-- Externo: 12 mesas (nr 21 a 32, numeradas 1 a 12)
;with nums as (select 1 as n union all select n + 1 from nums where n < 12)
insert into mesas (nr, numero, status, qtd_lugares, codigo_ambientes)
select n + 20, n, N'Disponivel', 4, 2
from nums
option (maxrecursion 12);
go

-- Sacada: 8 mesas (nr 33 a 40, numeradas 1 a 8)
;with nums as (select 1 as n union all select n + 1 from nums where n < 8)
insert into mesas (nr, numero, status, qtd_lugares, codigo_ambientes)
select n + 32, n, N'Disponivel', 2, 3
from nums
option (maxrecursion 8);
go
