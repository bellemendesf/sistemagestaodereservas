create database gestaoReserva
go
use gestaoReserva
go
create table pessoas
(
	id			    int			not null primary key identity,
	nome			varchar(50) not null, 
	cpf				varchar(20) not null unique,
	senha			varchar(30) not null
)
go

create table clientes
(
	pessoa_id	  int		  not null primary key,
	email_pessoal	varchar(100)not null,
	telefone		  varchar(20) not null unique,
	--------Restrições---------
	constraint  fk_cliente_id
	foreign key (pessoa_id) references pessoas(id)
)
go

create table funcionarios
(
	pessoa_id int		  not null primary key,
	email_institucional	      varchar(50) not null
	---------Restrições---------
	constraint  fk_funcionario_id
	foreign key (pessoa_id) references pessoas(id) 
)
go

create table ambientes
(
	codigo		int			not null primary key,
	nome		varchar(50) not null,
	status		varchar(20)		null,
	capacidade	int			not null
)
go

create table mesas
(
	nr						int not null primary key,
	numero					int not null,
	qtd_lugares			    int not null,
	status					varchar(20) not null, 
	codigo_ambientes		int not null,
	---Restrições----------	
	constraint fk_codigo_ambientes
	foreign key(codigo_ambientes) references ambientes(codigo)
	

)
go

create table reservas
(
	numero		int  not null primary key identity,
	qtd_pessoas	int not null,
	data_hora		datetime not null default cast(current_timestamp as time),
	status		int		     check(status in(0,1,2)),
	mesas_nr	int  not null,
	cliente_id int not null,
	----Restrições-----
	foreign key(cliente_id)		references clientes(pessoa_id),
	foreign key(mesas_nr)		references mesas(nr)
)
go

select * from reservas

create table Reservas_Mesas
(	
	numero_reservas int not null, 
	nr_mesas		int not null
	
		-----Restrições------
	constraint pk_reservas_mesas primary key(numero_reservas, nr_mesas)
	constraint fk_numero_reservas 
	foreign key(numero_reservas) references reservas(numero),
	constraint fk_nr_mesas
	foreign key(nr_mesas) references mesas(nr)
)
go
