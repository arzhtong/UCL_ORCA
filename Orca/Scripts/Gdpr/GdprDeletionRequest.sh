# 1. To run, first execute command 'chmod +x GdprDeletionRequest.sh'
# 2. Execute script by running './GdprDeletionRequest.sh [user_id]' where [user_id] is the student ID of the student the report is being generated for.
# 3. Change localhost to url of remote server if exporting from remote server.


#!/bin/bash

MYSQL="/usr/local/mysql/bin/mysql"

read -p "Enter IP address of the MySQL server: " HOSTNAME

read -p "Enter the port:" PORT

read -p "Enter MySQL username: " USERNAME

read -s -p "Enter password for MySQL user: " PASSWORD

DBNAME="ORCA_DB"

TABLE="event"

delete_events="delete from ${TABLE} where student_id = '$1'"

TABLE="student"
delete_details="delete from ${TABLE} where id = '$1'"

${MYSQL} -h${HOSTNAME} -P${PORT} -u${USERNAME} -p${PASSWORD} ${DBNAME} -e "${delete_events}"
${MYSQL} -h${HOSTNAME} -P${PORT} -u${USERNAME} -p${PASSWORD} ${DBNAME} -e "${delete_details}"

echo "Data for Student with ID $1 has been deleted from the ORCA Database."