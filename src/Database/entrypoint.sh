echo 'Starting sql server..' ;

/opt/mssql/bin/sqlservr &

echo 'SQL server started.';

sleep 30 ;

echo 'Create database..' ;

/opt/mssql-tools18/bin/sqlcmd -d master -i /snk_update_master_db_scripts/CreateDatabase.sql -S localhost -U sa -P Snk@12345 -No ;
/opt/mssql-tools18/bin/sqlcmd -d master -i /snk_update_master_db_scripts/CreateStructure.sql -S localhost -U sa -P Snk@12345 -No ;

echo 'Database created' ;

tail -f /dev/null