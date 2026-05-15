# 💳 WalletWatch

**Take control of your finances with ease.** WalletWatch is a polished, modern expense tracking application built with .NET 10 and ASP.NET Core MVC. It provides a comprehensive dashboard to monitor your spending habits, set goals, and manage your budget effectively.

---

## 🚀 Live Demo
The application is hosted on Azure. You can view it here:  
👉 **[Live Demo](https://financialhelper.azurewebsites.net/)**  
*(Note: As it is on a free plan, it may take a few seconds to wake up.)*

### 🔑 Test Credentials
Explore the app without creating an account:
- **Email:** `test@testing.com`
- **Password:** `Test123`

---

## ✨ Key Features
- **Interactive Dashboard:** Real-time visualization of your Income vs. Expenses using Syncfusion charts.
- **Transaction Management:** Effortlessly log your daily spending and earnings.
- **Custom Categories:** Organize your finances with custom categories and intuitive icons.
- **Secure Authentication:** Robust user accounts and role-based access control via ASP.NET Core Identity.
- **Data Isolation:** Your financial data is private and tied securely to your account.

---

## 🛠️ Tech Stack
- **Framework:** ASP.NET Core MVC (.NET 10.0)
- **UI Components:** [Syncfusion EJ2](https://www.syncfusion.com/aspnet-core-ui-controls) (Charts, Grids, DatePickers)
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Security:** ASP.NET Core Identity
- **Frontend:** HTML5, CSS3, JavaScript, Bootstrap, FontAwesome

---

## 💻 Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (LocalDB or full instance)

### Installation
1. **Clone the repository:**
   ```bash
   git clone https://github.com/Azeemvirji/WalletWatch.git
   cd WalletWatch/ExpenseTracker
   ```

2. **Configure Database:**
   Update your connection string in `appsettings.json`.

3. **Run Migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Build and Run:**
   ```bash
   dotnet run
   ```

---

## 📄 License
This project is open-source. Feel free to use and modify it for your own needs.

## ✉️ Contact
Have questions or feedback? Visit the [Privacy Page](https://financialhelper.azurewebsites.net/Home/Privacy) on the live site to fill out a contact form.
