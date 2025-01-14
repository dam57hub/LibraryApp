###
to run the project:
make sure you have docker installed
to spawn the mysql database:
'''
docker run --name d-mysql -p 3306:3306 -e MYSQL_ROOT_PASSWORD=damian -d mysql
'''
to run the project:
make sure you have dotnet installed
make sure you have libraries installed
'''
dotnet restore
'''
'''
dontet ef database update
dotnet run
'''
dotnet ef database update runs migrations and updates the database
dotnet run runs the project