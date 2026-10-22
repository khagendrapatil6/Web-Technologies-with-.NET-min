-- College Fest database (MySQL 8+). The app also runs this automatically on startup,
-- so running it by hand is optional (useful to show in MySQL Workbench / for your report).

CREATE DATABASE IF NOT EXISTS college_fest CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE college_fest;

CREATE TABLE IF NOT EXISTS events (
    event_id             INT            NOT NULL PRIMARY KEY,
    event_name           VARCHAR(80)    NOT NULL UNIQUE,
    maximum_participants INT            NOT NULL,
    registration_fee     DECIMAL(10,2)  NOT NULL DEFAULT 0
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS participants (
    participant_id       INT            NOT NULL AUTO_INCREMENT PRIMARY KEY,
    participant_name     VARCHAR(80)    NOT NULL,
    email                VARCHAR(120)   NOT NULL,
    department           VARCHAR(80)    NOT NULL,
    academic_year        TINYINT        NOT NULL,
    event_id             INT            NOT NULL,
    is_team_event        TINYINT(1)     NOT NULL DEFAULT 0,
    registration_fee     DECIMAL(10,2)  NOT NULL,
    is_completed         TINYINT(1)     NOT NULL DEFAULT 0,
    registered_on        DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_participant_event FOREIGN KEY (event_id) REFERENCES events(event_id),
    CONSTRAINT uq_event_email UNIQUE (event_id, email)
) ENGINE=InnoDB AUTO_INCREMENT=1001;

-- Handy queries for your viva / report:
-- SELECT p.participant_id, p.participant_name, e.event_name, p.is_completed FROM participants p JOIN events e USING(event_id);
-- SELECT e.event_name, COUNT(p.participant_id) AS registered, e.maximum_participants FROM events e LEFT JOIN participants p USING(event_id) GROUP BY e.event_id;
