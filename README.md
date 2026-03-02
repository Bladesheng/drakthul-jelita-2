# drakthul-jelita-2

Set up S3 secrets:
```bash
dotnet user-secrets init

dotnet user-secrets set "S3:AccessKeyId" "xxx"
dotnet user-secrets set "S3:SecretAccessKey" "xxx"
dotnet user-secrets set "S3:Bucket" "xxx"
dotnet user-secrets set "S3:Endpoint" "xxx"
```

Set up admin password:
```bash
dotnet user-secrets set "AdminPassword" "xxx"
```
