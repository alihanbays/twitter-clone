# Twitter Clone

A Twitter-like social media web application built with **C# and .NET**, featuring user authentication, tweet creation, and a dynamic timeline. This project was created as a personal learning exercise to strengthen backend and frontend development skills using .NET technologies.

---

## 🚀 Features

- **User Authentication**: Sign up, log in, and manage sessions.
- **Tweeting**: Create, view, and delete tweets.
- **Timeline**: Displays posts from users you follow.
- **Follow System**: Follow and unfollow other users.
- **Responsive UI**: Clean and simple interface for easy interaction.
- **Unit Tests**: Basic test coverage for core functionality.

---

## 🛠️ Tech Stack

- **Backend**: C# (.NET 8 or higher)
- **Frontend**: Razor Pages (or MVC Views) + HTML/CSS
- **Database**: SQLite
- **Testing**: xUnit (or your chosen test framework)

---

## 📂 Project Structure

```
.
├── Backend/         # API and server-side logic
├── Frontend/        # UI components, Razor pages, or Blazor files
├── Tests/           # Automated tests
├── twitter-clone.sln
├── README.md
└── .gitignore
```

---

## ⚙️ Getting Started

### ✅ Prerequisites
- [.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download)

### ▶️ Run Locally

```bash
# Clone the repository
git clone https://github.com/alihanbays/twitter-clone.git
cd twitter-clone

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project Backend/YourBackendProjectName.csproj
```

Then, open the app in your browser at:

```
http://localhost:5000
```

---

## 🧪 Running Tests

```bash
dotnet test Tests/
```

---

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. Fork this repo
2. Create your feature branch:  
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. Commit your changes:  
   ```bash
   git commit -m "Add new feature"
   ```
4. Push to the branch:  
   ```bash
   git push origin feature/your-feature-name
   ```
5. Open a Pull Request

---

## 📜 License

This project is open source and available under the **MIT License**.

---

### 🙌 Acknowledgements
Thanks to the .NET community and open-source contributors for making awesome tools and libraries!
