# PROGRESS.md — Suivi d'avancement

Mis à jour manuellement par Ryan à chaque session.

---

## Travaux terminés ✅

### Environnement
- [x] WSL2 installé sur Windows
- [x] Git configuré avec clé SSH connectée à GitHub
- [x] Dépôt GitHub créé : `github.com/ryanChristino05/exercices-stage`
- [x] Visual Studio avec .NET 10 installé
- [x] SQL Server Express installé et actif
- [x] Docker Desktop installé (moteur desktop-linux, WSL2 backend)

### Apprentissage
- [x] Terminal Linux : mkdir, echo, cat, grep, chmod, wc -l, >, >>, |
- [x] Git : init, add, commit, push, branches, merge
- [x] LINQ : Where, Select, OrderBy, FirstOrDefault, Sum, Average, Max
- [x] async/await : Task, Task.WhenAll, parallélisme

### Projet MonApi — CRUD
- [x] Projet ASP.NET Core Web API créé (`dotnet new webapi`)
- [x] Modèle `TaskItem` créé
- [x] Modèle `User` créé
- [x] `AppDbContext` configuré avec `Tasks` et `Users`
- [x] `Program.cs` configuré avec EF Core + JWT + UseRouting
- [x] Migrations créées et appliquées (`Init` + `AjoutUser`)
- [x] Base de données `MonApiDB` créée dans SQL Server Express
- [x] `TasksController` complet : GET all, GET by id, POST, PUT, DELETE
- [x] Testé dans Postman : tous les endpoints CRUD fonctionnels

### Projet MonApi — Authentification JWT ✅
- [x] `AuthController` complet : Register + Login
- [x] BCrypt installé et utilisé pour le hashage des mots de passe
- [x] Token JWT généré au Login (validité 2h, signé HS256)
- [x] Test Register dans Postman : ✅ "Compte créé avec succès"
- [x] Test Login dans Postman : ✅ Token JWT reçu et compris sur jwt.io
- [x] `[Authorize]` ajouté sur `TasksController`
- [x] Test sans token : ✅ 401 Unauthorized
- [x] Test avec token Bearer : ✅ 200 OK avec données

### Projet MonApi — Tests unitaires xUnit ✅
- [x] Projet `MonApi.Tests` créé et ajouté à la solution
- [x] Référence vers `MonApi` ajoutée dans `MonApi.Tests`
- [x] Packages installés : xUnit, Mvc.Testing, EF InMemory
- [x] Base de données en mémoire configurée pour les tests
- [x] 4 tests écrits et passants :
  - `GetAll_ReturneListeTaches` → 200 OK ✅
  - `GetById_ReturneNotFound_SiInexistant` → 404 ✅
  - `Create_AjouteTache` → 201 Created ✅
  - `Delete_SupprimeTache` → 204 NoContent ✅

### Projet MonApi — Docker ✅
- [x] SQL Server configuré : TCP/IP activé, port fixe 1433, mode mixte activé
- [x] Compte SQL dédié `monapi_user` créé avec droits sur `MonApiDB`
- [x] Dockerfile multi-stage créé (contexte parent `ProjetApi\`)
- [x] Problème de contexte résolu : `MonApi.Tests` inclus dans le build
- [x] `docker build` réussi → image `monapi:latest` (ID: 9e725d8c3653)
- [x] `docker run` réussi → conteneur `monapi-container` actif
- [x] Test sans token : ✅ 401 Unauthorized depuis le conteneur
- [x] Test Register depuis le conteneur : ✅ compte créé dans SQL Server
- [x] Test Login depuis le conteneur : ✅ token JWT reçu
- [x] Connexion conteneur → SQL Server via `host.docker.internal` validée

---

## En cours 🔄

- [ ] Déploiement : mettre l'API en ligne avec une URL publique

---

## Prochaines étapes (dans l'ordre)

1. **Déploiement**
   - Choisir une plateforme (Railway recommandé)
   - Pousser l'image ou le code sur la plateforme
   - Configurer les variables d'environnement en ligne
   - Obtenir une URL publique
   - Tester depuis Postman avec l'URL publique

2. **Projet portfolio**
   - Choisir un sujet concret (à décider avec Ryan)
   - README complet
   - Déployé et visible sur GitHub
   - CV mis à jour avec le lien

---

## Dernières erreurs rencontrées

| Erreur | Cause | Solution |
|---|---|---|
| `error: remote origin already exists` | Remote déjà configuré en HTTPS | `git remote remove origin` puis rajouter en SSH |
| `MSB3026: Impossible de copier` | API encore en cours d'exécution | Arrêter l'API avec Ctrl+C avant de relancer |
| Login affichait "compte créé avec succès" | URL `/auth/register` utilisée au lieu de `/auth/login` | Vérifier l'URL dans Postman |
| 401 non retourné malgré `[Authorize]` | Ordre des middlewares incorrect | Ajouter `app.UseRouting()` avant `UseAuthentication` |
| Cycle détecté dans les références | MonApi se référençait lui-même dans `.csproj` | Supprimer la ligne `<ProjectReference Include="MonApi.csproj" />` dans MonApi.csproj |
| `docker build` échoue : fichier introuvable | Contexte Docker trop restreint (MonApi\ seulement) | Lancer depuis `ProjetApi\` avec `-f MonApi\Dockerfile .` |

---

## Informations inconnues / à compléter

- Noms exacts des fichiers de migration dans `Migrations/`
- Port exact si différent de 5281 après redémarrage
