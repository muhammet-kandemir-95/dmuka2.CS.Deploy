﻿IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'__migrations')
BEGIN
	CREATE TABLE __migrations (
	  migration_id bigint PRIMARY KEY IDENTITY(1,1) NOT NULL,
	  file_name varchar(1000) NOT NULL
	)
END
