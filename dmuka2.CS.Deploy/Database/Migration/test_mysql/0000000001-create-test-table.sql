
CREATE TABLE `test_table` (
  `test_id` bigint(20) NOT NULL,
  `test_col` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE `test_table`
  ADD PRIMARY KEY (`test_id`);

ALTER TABLE `test_table`
  MODIFY `test_id` bigint(20) NOT NULL AUTO_INCREMENT;
