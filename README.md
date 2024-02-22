# Part One: Blog Platform
Represents RESTful API for simple Blog Post platform with Swagger UI and authentication.</br>
Open the project in your preferred IDE</br>
Modify connection strings in `appsettings.Development.json` and `appsettings.Production.json` based on your configuration.</br>
```json
"ConnectionStrings": {
    "Default": "YourConnectionString"
}
```
Also you can change some other configuration such as token lifetime and JWT signing parameters in those specified `appsettings.json` files.</br>
Then rebuild project.</br>
If everything went well, time to apply database migration. For that open Package manager console of Visual Studio and run command:</br>
`Update-Database`</br>
That's all you can run project, Swagger UI will appear:)
Additionally you can test Registration and Login logic via running XUnit tests.</br>
# Part Two: GetAmortizationSchedule
Stored procedure generates monthly payment report (amortization table) for loan.</br>
To test the procedure run command in MS Sql Server via Management Studio:</br>
```
EXEC [dbo].[GetAmortizationSchedule]
```
