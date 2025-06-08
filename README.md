# SteamCurrency

```{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MongoDbSettings": {
    "ConnectionString": "mongoDB/connectionString",
    "DatabaseName": "SteamCurrency",
    "CollectionName": "Currencies"
  },
  "SteamAPI": {
    "URL": "https://steamcommunity.com/market/priceoverview/?appid={appId}&market_hash_name={product}&currency={currencyId}"
  }
}```
