# 🚀 Web API Clean Architecture Template (.NET)

This repository provides a **ready-to-use .NET Web API solution template** designed to help you **bootstrap new backend projects in seconds** using the `dotnet` CLI.

The template follows **Clean Architecture**, is **microservice-friendly**, and is ideal for building scalable backend systems.

---

## ✨ What’s Included

✅ Clean Architecture structure
* API
* Application
* Infrastructure
* Domain

✅ ASP.NET Web API (.NET 8 / 9 / 10 ready)\
✅ Entity Framework Core\
✅ ASP.NET Identity (ready for JWT-based authentication)\
✅ Dependency Injection per layer\
✅ Designed for microservices & enterprise systems

---

## 📁 Solution Structure

```text
src/
 ├─ <ProjectName>.Api
 ├─ <ProjectName>.Application
 ├─ <ProjectName>.Infrastructure
 └─ <ProjectName>.Domain
```

Each layer has a clear responsibility and minimal coupling, making the solution easy to maintain, test, and scale.

---

## ⚙️ Prerequisites

* .NET SDK installed (8.0 or later recommended)
* `dotnet` CLI available in your terminal

Check your version:

```sh
dotnet --version
```

---

## 📦 Install the Template

From the **root folder of this repository**, run:

### On macOS / Linux (bash, zsh)

```sh
dotnet new install ./
```

### On Windows (Command Prompt / PowerShell)

```sh
dotnet new install .\
```

After installation, the template will be available globally on your machine.

---

## 🚀 Create a New Project

Use the following command to generate a brand-new solution:

```sh
dotnet new webapi-template --ProjectName MyAwesomeProject
```

🎉 **That’s it!**
A fully structured Clean Architecture Web API solution will be created with the project name you choose.

---

## 🧠 How It Works

* `webapi-template` is the **template short name**
* `--ProjectName` defines:

  * Solution name
  * Project names
  * Namespaces
* No manual renaming required

Perfect for:

* Starting new microservices
* Backend-focused CV projects
* Internal company templates
* Rapid prototyping

---

## 🔄 Uninstall the Template (Optional)

If you ever want to remove the template:

```sh
dotnet new uninstall <template-folder-path>
```

---

## 📌 Notes

* This template is framework-agnostic enough to support:

  * JWT authentication
  * Role / permission-based authorization
  * Multi-tenancy
* You can easily extend it with:

  * Message brokers (Kafka, RabbitMQ)
  * API Gateway
  * Observability (OpenTelemetry, Serilog)

---

## ⭐ Final Words

This template is built to **save setup time**, **enforce good architecture**, and let you **focus on business logic immediately**.

Clone once.
Generate forever.
Happy coding! 🚀
