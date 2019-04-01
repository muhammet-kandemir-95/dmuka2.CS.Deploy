DO $$
DECLARE 
    brow record;
BEGIN
    FOR brow IN (select 'drop table "' || schemaname || '"."' || tablename || '" cascade;' as table_name from pg_tables where schemaname <> 'pg_catalog' AND schemaname <> 'information_schema') LOOP
        EXECUTE brow.table_name;
    END LOOP;
END; $$