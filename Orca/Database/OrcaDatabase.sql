-- MySQL Script generated by MySQL Workbench
-- Wed Feb 10 11:57:42 2021
-- Model: New Model    Version: 1.0
-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema ORCA_DB
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema ORCA_DB
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `ORCA_DB` DEFAULT CHARACTER SET utf8 ;
USE `ORCA_DB` ;

-- -----------------------------------------------------
-- Table `ORCA_DB`.`event`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ORCA_DB`.`event` (
  `id` int NOT NULL AUTO_INCREMENT,
  `student_id` VARCHAR(45) NOT NULL,
  `course_id` VARCHAR(60)  NULL,
  `timestamp` DATETIME NOT NULL,
  `event_type` VARCHAR(45) NOT NULL,
  `activity_name` VARCHAR(45) NULL,
  `activity_details` VARCHAR(45) NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `FK_event`
    FOREIGN KEY (`student_id`)
    REFERENCES `ORCA_DB`.`student` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ORCA_DB`.`student`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ORCA_DB`.`student` (
  `id` VARCHAR(45) NOT NULL,
  `first_name` VARCHAR(45) NULL,
  `last_name` VARCHAR(45) NULL,
  `email` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
