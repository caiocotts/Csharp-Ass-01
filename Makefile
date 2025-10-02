# instantiate a new postgres database in a docker container
dbup:
	docker compose up -d
.PHONY: dbup

# down the postgres database
dbdown:
	docker compose down
.PHONY: dbdown

# apply the latest migration to the database
mup:
	dotnet ef database update
.PHONY: mup

# remove the latest migration
mdown:
	dotnet ef migrations remove
.PHONY: mdown

# check the status of the docker container
status:
	docker compose ps
.PHONY: status

# show logs for the database container
logs:
	docker compose logs -f
.PHONY: logs

# create a new migration file
migration:
	@if [ "$(OS)" = "Windows_NT" ]; \
	then \
		if not defined name ( echo Usage: make migration name=name_of_migration_file & exit /b 1 ); \
		dotnet ef migrations add %name%; \
	else \
		if [ -z "${name}" ]; then echo "Usage: make migration name=name_of_migration_file"; exit 1; fi; \
		dotnet ef migrations add ${name}; \
	fi
.PHONY: migration

# clean up the docker container and the database data
clean:
	docker compose down
	@if [ "$(OS)" = "Windows_NT" ]; \
	then rmdir /s /q pgdata; \
	else rm -rf pgdata; \
	fi
.PHONY: clean
