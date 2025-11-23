echo 'Starting sql server..' ;

/opt/mssql/bin/sqlservr &

echo 'SQL server started.';

sleep 30 ;

echo 'Create database..' ;

/opt/mssql-tools/bin/sqlcmd -d master -i /snk_update_master_db_scripts/CreateDatabase.sql -U sa -P Snk@12345 ;
/opt/mssql-tools/bin/sqlcmd -d master -i /snk_update_master_db_scripts/CreateStructure.sql -U sa -P Snk@12345 ;

echo 'Database created' ;

tail -f /dev/null