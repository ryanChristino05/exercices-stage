# AI_CONTEXT.md — Contexte technique pour IA

Ce fichier permet à une IA de reprendre le travail sur ce projet sans perdre de contexte.
Mis à jour manuellement par Ryan à chaque session.

---

## Profil du développeur

- Prénom : Ryan
- Étudiant en 3e année d'école d'ingénieur à Madagascar
- Cherche un stage en programmation et développement
- Bon niveau en C# (POO, classes, objets, LINQ, async/await maîtrisés)
- Travaille sur Windows avec Visual Studio
- Utilise WSL2 pour le terminal Linux et Git
- Langage principal : C# / ASP.NET Core
- Langage secondaire : Java

---

## Contexte du projet

On est dans un **guide de préparation au stage**. Ryan apprend les compétences clés
en développement pour arriver en stage avec un niveau solide et un projet portfolio concret.
On avance vite, on comprend avant de faire, et on met à jour les fichiers à chaque étape.

---

## Architecture du projet

Projet : `MonApi`
Emplacement local : `D:\Csharp\ProjetApi\MonApi`
Framework : ASP.NET Core Web API (.NET 10)
Base de données : SQL Server Express (local)
ORM : Entity Framework Core
Port local : `http://localhost:5281`

Projet de tests : `MonApi.Tests`
Emplacement local : `D:\Csharp\ProjetApi\MonApi.Tests`
Framework de test : xUnit + EF Core InMemory

---

## Structure des dossiers

```
D:\Csharp\ProjetApi\
├── MonApi\
│   ├── Controllers\
│   │   ├── TasksController.cs      # CRUD tâches + [Authorize] (complet)
│   │   └── AuthController.cs       # Register + Login JWT (complet)
│   ├── Data\
│   │   └── AppDbContext.cs         # Contexte EF Core
│   ├── Models\
│   │   ├── TaskItem.cs             # Modèle tâche
│   │   └── User.cs                 # Modèle utilisateur
│   ├── Migrations\                 # Init + AjoutUser
│   ├── Dockerfile                  # Multi-stage, contexte parent ProjetApi\
│   ├── appsettings.json
│   ├── Program.cs
│   └── MonApi.csproj
└── MonApi.Tests\
    ├── UnitTest1.cs                # 4 tests écrits et passants
    └── MonApi.Tests.csproj
```

---

## Modèles

### TaskItem (Models/TaskItem.cs)
```csharp
public class TaskItem
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public bool EstTerminée { get; set; } = false;
}
```

### User (Models/User.cs)
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
```

### UserDto (dans AuthController.cs)
```csharp
public class UserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

---

## AppDbContext (Data/AppDbContext.cs)

```csharp
using Microsoft.EntityFrameworkCore;
using MonApi.Models;

namespace MonApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
}
```

---

## Program.cs (configuration complète et finale)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MonApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Ordre critique des middlewares
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

---

## appsettings.json (développement local uniquement)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=MonApiDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "CeciEstUneCléSecrèteTrèsLongueEtSécurisée123!",
    "Issuer": "MonApi",
    "Audience": "MonApiUsers"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> ⚠️ Ce fichier reste pour le développement local (`dotnet run`).
> En Docker, les valeurs sont remplacées par des variables d'environnement.

---

## Dockerfile (multi-stage, contexte parent)

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY MonApi/MonApi.csproj MonApi/
COPY MonApi.Tests/MonApi.Tests.csproj MonApi.Tests/
COPY MonApi/MonApi.slnx MonApi/

RUN dotnet restore MonApi/MonApi.slnx

COPY MonApi/ MonApi/
COPY MonApi.Tests/ MonApi.Tests/

RUN dotnet publish MonApi/MonApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "MonApi.dll"]
```

**Commande de build (depuis `D:\Csharp\ProjetApi`) :**
```powershell
docker build -t monapi -f MonApi\Dockerfile .
```

**Commande de lancement :**
```powershell
docker run -d `
  --name monapi-container `
  -p 5281:8080 `
  -e ASPNETCORE_URLS="http://+:8080" `
  -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal,1433;Database=MonApiDB;User Id=monapi_user;Password=<MOT_DE_PASSE>;TrustServerCertificate=True;" `
  -e "Jwt__Key=CeciEstUneCléSecrèteTrèsLongueEtSécurisée123!" `
  -e "Jwt__Issuer=MonApi" `
  -e "Jwt__Audience=MonApiUsers" `
  monapi
```

---

## Configuration SQL Server (locale)

- Instance : `DESKTOP-UP1CEME\SQLEXPRESS`
- Base de données : `MonApiDB`
- TCP/IP activé, port fixe 1433
- Compte dédié : `monapi_user` (droits `db_datareader` + `db_datawriter`)
- Authentification mode mixte activée

---

## Tests unitaires (MonApi.Tests/UnitTest1.cs)

4 tests écrits et passants :
- `GetAll_ReturneListeTaches` → vérifie GET /tasks retourne 200 + liste
- `GetById_ReturneNotFound_SiInexistant` → vérifie GET /tasks/99 retourne 404
- `Create_AjouteTache` → vérifie POST /tasks retourne 201 et crée la tâche
- `Delete_SupprimeTache` → vérifie DELETE /tasks/1 retourne 204 et supprime

Base de données en mémoire utilisée pour les tests (pas SQL Server).

---

## Packages NuGet installés

### MonApi
- `BCrypt.Net-Next` 4.2.0
- `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.12
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 10.0.12
- `Microsoft.AspNetCore.OpenApi` 10.0.12
- `Microsoft.EntityFrameworkCore.Design` 10.0.12
- `Microsoft.EntityFrameworkCore.SqlServer` 10.0.12
- `System.IdentityModel.Tokens.Jwt` 8.23.0

### MonApi.Tests
- `xunit` 2.9.3
- `Microsoft.AspNetCore.Mvc.Testing` 10.0.12
- `Microsoft.EntityFrameworkCore.InMemory` 10.0.12
- `coverlet.collector` 6.0.4

---

## Décisions techniques

- Pas de Swagger (tests via Postman uniquement)
- Mots de passe hashés avec BCrypt
- Token JWT valide 2 heures, algorithme HS256
- `[Authorize]` sur toute la classe `TasksController`
- Ordre middlewares : UseRouting → UseAuthentication → UseAuthorization → MapControllers
- Données stockées en base SQL Server, pas en mémoire
- Docker : variables d'environnement remplacent appsettings.json au runtime
- `host.docker.internal` utilisé pour joindre SQL Server depuis le conteneur Linux

---

## Prochaine étape exacte

**Déploiement** :
1. Choisir une plateforme (Railway recommandé pour débuter)
2. Pousser l'image Docker sur un registry (Docker Hub ou Railway directement)
3. Configurer les variables d'environnement sur la plateforme
4. Obtenir une URL publique
5. Tester depuis Postman avec l'URL publique
