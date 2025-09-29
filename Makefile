dbup:
	docker compose up -d
.PHONY: dbup

dbdown:
	docker compose down
.PHONY: dbdown

mup:
	dotnet ef database update
.PHONY: mup

mdown:
	dotnet ef migrations remove
.PHONY: mdown

migration:
	@if [ -z "${name}" ]; then echo "Usage: make migration name=name_of_migration_file"; exit 1; fi
	 dotnet ef migrations add ${name}
.PHONY: migration