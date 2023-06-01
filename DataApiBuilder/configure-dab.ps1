## uncomment code below to install or update the DataApiBuilder tool

#echo "Installing or updating Microsoft.DataApiBuilder..."	
# dotnet tool install --global Microsoft.DataApiBuilder 
#dotnet tool update --global Microsoft.DataApiBuilder

## script below relies on local SQL Server instance and Northwind database
## Northwind database can be downloaded from here: https://github.com/Microsoft/sql-server-samples/tree/master/samples/databases/northwind-pubs

echo "Initiating and configuring DataApiBuilder configuration file..."

dab init --database-type "mssql" --connection-string "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False" --host-mode "Development"

dab add Order --source dbo.Orders --permissions "anonymous:*"

dab add Customer --source dbo.Customers --permissions "anonymous:*"
dab update Customer --relationship "orders" --cardinality "many" --target.entity "Order"

dab add Employee --source dbo.Employees --permissions "anonymous:*"
dab update Employee --relationship "orders" --cardinality "many" --target.entity "Order"



