DO
$do$
begin
	IF EXISTS (
	   SELECT 1 
	   FROM   pg_catalog.pg_class c
	   JOIN   pg_catalog.pg_namespace n ON n.oid = c.relnamespace
	   WHERE  n.nspname = 'public'
	   AND    c.relname = '__migrations'
	   AND    c.relkind = 'r'    -- only tables
	   ) = false THEN
	   		CREATE TABLE public.__migrations
			(
				migration_id bigserial NOT NULL,
				file_name character varying(1000) NOT NULL,
				PRIMARY KEY (migration_id)
			)
			WITH (
				OIDS = FALSE
			);

			ALTER TABLE public.__migrations
				OWNER to postgres;
			COMMENT ON TABLE public.__migrations
				IS 'This table store migrations to submit last database changes.';
	END IF;
end
$do$