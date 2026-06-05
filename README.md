# 🎞️ Custom React Disney Library combining the Data from disneyapi.dev and themoviedb.org

Custom build Library SPA that combines the character data from disneyapi.dev and the film, tv show and short film data from themoviedb.org

## 🚀 Features

- **Backend written in ASP.NET Core:** The backend crawls the disneyapi.dev data daily and crawls the themoviedb data if it finds new films, tv shows or short films
- **Frontend build in React:** The Frontend is a React SPA displaying only the important data

---

## 💻 Tech Stack
Languages: TypeScript (Strict Mode), React, ASP.NET Core, PostgeSQL

Bundler/Build-Tool: Webpack

Styling: CSS3 / TailwindCSS 4

---

## 🔧 Installation & starting locally
1. Clone Repo:

```Bash
- git clone https://github.com/ChristianReinsberg/disneyClient.git
```

2. Move into directory:

```Bash
cd disneyClient
```
3. Install dependencies:

```Bash
docker compose build
```

4. Start development server:
```bash
docker compose up
```
