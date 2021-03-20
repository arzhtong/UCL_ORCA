# 1. To run first execute commands 'chmod +x ExportEventData.sh', 'chmod +x ExportStudentDetails.sh' and 'chmod +x GdprComplianceExport.sh'.
# 2. Execute script by running './GdprComplianceExport.sh [user_id]' where [user_id] is the student ID of the student the report is being generated for.
# 3. Change localhost to url of remote server if exporting from remote server.


#!/bin/bash
MYSQL="/usr/local/mysql/bin/mysql"

read -p "Enter IP address of the MySQL server: " HOSTNAME

read -p "Enter the port:" PORT

read -p "Enter MySQL username: " USERNAME

read -s -p "Enter password for MySQL user: " PASSWORD

DBNAME="ORCA_DB"

./ExportStudentDetails.sh $1 ${MYSQL} ${HOSTNAME} ${PORT} ${USERNAME} ${PASSWORD} ${DBNAME} && ./ExportEventData.sh $1 ${MYSQL} ${HOSTNAME} ${PORT} ${USERNAME} ${PASSWORD} ${DBNAME}
