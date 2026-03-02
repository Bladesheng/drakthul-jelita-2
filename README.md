# drakthul-jelita-2

I made this so i can upload screenshots of cool names from WoW that I collected over the years. Other people can look at them I guess.
This is C# rewrite of my [previous Laravel app](https://github.com/Bladesheng/drakthul-jelita).

- Backend is ASP.NET Core
- Frontend is made with Razor MVC templates, Tailwind, daisyUI and a bit of JS
- Only admin can upload, edit and delete screenshots
- There is tesseract.js OCR in the upload form page, so you don't have to type out the names manually
- Screenshots are stored in S3 (Cloudflare R2)

## Installation

- Set up S3 secrets:
```bash
dotnet user-secrets init

dotnet user-secrets set "S3:AccessKeyId" "xxx"
dotnet user-secrets set "S3:SecretAccessKey" "xxx"
dotnet user-secrets set "S3:Bucket" "xxx"
dotnet user-secrets set "S3:Endpoint" "xxx"
```
