#!/bin/bash
TABLE="event"

select_events="select * from ${TABLE} where student_id = $1"

$2 -h$3 -P$4 -u$5 -p$6 $7 -e "${select_events}" > 1.log << EOF
EOF

echo "Downloading student $1 data from ${DBNAME} in mysql..."
sleep 1
echo "now converting it to csv file..."
sleep 1

#cat the 1.log file and convert it to csv file
cat 1.log | while read line
do
echo $line | tr " " ","
done > ../../Record_data/Student_Events/$1"_EVENTS_GDPR_RECORD".csv

sleep 1
#remove the temporal file 1.log
rm -rf 1.log

#echo the infomation
echo "Convert ${DBNAME} into $1_EVENTS_GDPR_RECORD.csv."
sleep 1
echo "Done successfully! Please check the file!"
