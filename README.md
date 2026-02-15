\# 🎮 Game Localization REST API (Backend for Games)



As a Game Developer, I usually live inside the game engine (Unity). However, I wanted to step out of my comfort zone and understand how the "dark side" (Backend) works. 



This project is a fully functional, production-ready RESTful API built with \*\*.NET Core\*\* and \*\*Entity Framework Core\*\*. It is designed to act as a centralized localization (multi-language) server for video games.



\## 🎯 Architectural Decisions \& Features (Why I built it this way)



Instead of just performing basic CRUD operations, I designed this API considering the different needs of the end-users (The Game Client vs. The Admin Panel).



\* \*\*Client-Optimized Endpoints (Data Shaping):\*\* Games shouldn't download heavy database models. The `/{isoCode}/export` endpoint returns a pure, flat JSON object (`Dictionary<string, string>`). Unity can download and parse this into RAM instantly during a loading screen without wasting bandwidth on IDs or metadata.

\* \*\*Admin Pagination:\*\* When a translator uses the admin panel, loading 50,000 strings at once would crash the browser. I implemented `Skip()` and `Take()` pagination for the admin endpoints to ensure scalable performance.

\* \*\*DTO (Data Transfer Object) Pattern:\*\* Completely decoupled the database entities (`GameLanguage`, `GameString`) from the client payload. This prevents over-posting vulnerabilities and improves the Developer Experience (DX) by hiding internal IDs and Foreign Keys.

\* \*\*Relational DB Architecture:\*\* Built a solid 1-to-Many relationship using SQLite, enforcing cascade deletions so that removing a language automatically cleans up orphaned translation strings.



\## 🛠️ Tech Stack

\* \*\*C# \& .NET Core\*\* (Web API)

\* \*\*Entity Framework Core\*\* (Code-First Approach \& ORM)

\* \*\*SQLite\*\* (Lightweight, perfect for small/medium indie games)

\* \*\*Scalar / OpenAPI\*\* (Modern, interactive API documentation \& testing UI)



\## 🚀 How to Run It



1\. Clone the repository.

2\. Open the project in Visual Studio or Rider.

3\. Build and Run the project (`F5`).

4\. The system will automatically generate the `game.db` SQLite file.

5\. You will be greeted by the \*\*Scalar\*\* UI in your browser, where you can instantly test endpoints without needing Postman.



---

\*Note for recruiters: This project was built to demonstrate my understanding of backend architecture, RESTful principles, and client-server communication in game development.\*

