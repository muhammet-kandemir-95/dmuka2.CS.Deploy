
CREATE TABLE `__migrations` (
  `migration_id` bigint(20) NOT NULL,
  `file_name` varchar(1000) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE `__migrations`
  ADD PRIMARY KEY (`migration_id`);

ALTER TABLE `__migrations`
  MODIFY `migration_id` bigint(20) NOT NULL AUTO_INCREMENT;
