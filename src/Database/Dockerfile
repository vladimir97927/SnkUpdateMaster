FROM mcr.microsoft.com/mssql/server:2025-latest

COPY ./SnkUpdateMasterDb/scripts/CreateDatabase.sql /snk_update_master_db_scripts/
COPY ./SnkUpdateMasterDb/scripts/CreateStructure.sql /snk_update_master_db_scripts/

ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=Snk@12345
ENV MSSQL_TCP_PORT=1433

ADD entrypoint.sh /

ENTRYPOINT ["/bin/bash", "/entrypoint.sh"]