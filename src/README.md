## DUENDE IDENTITY SERVER

- Migration Command
	+ dotnet ef migrations add InitialPersistedGrantMigration -c PersistedGrantDbContext -o Migrations/IdentityServer/PersistedGrantDb
	+ dotnet ef migrations add InititalConfigurationMigration -c ConfigurationDbContext -o Migrations/IdentityServer/ConfigurationDb
	+ dotnet ef database update -c PersistedGrantDbContext
	