using System.Text;
using TimeAccountingSystem;
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Глобальні "бази даних" нашої програми
List<User> globalUsers = new List<User>();
List<TimeAccountingSystem.Task> globalTasks = new List<TimeAccountingSystem.Task>();
User? currentUser = null;

// Головний цикл програми
while (true)
{
    try
    {
        if (currentUser == null)
        {
            ShowAuthMenu();
        }
        else if (currentUser is Admin admin)
        {
            ShowAdminMenu(admin);
        }
        else if (currentUser is Manager manager)
        {
            ShowManagerMenu(manager);
        }
        else if (currentUser is Employee employee)
        {
            ShowEmployeeMenu(employee);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[ПОМИЛКА]: {ex.Message}");
        Console.WriteLine("Натисніть Enter для продовження...");
        Console.ReadLine();
    }
}

// ==========================================
// МЕНЮ АВТОРИЗАЦІЇ ТА РЕЄСТРАЦІЇ
// ==========================================
void ShowAuthMenu()
{
    Console.Clear();
    Console.WriteLine("=== СИСТЕМА ОБЛІКУ ЧАСУ ===");
    Console.WriteLine("1. Увійти");
    Console.WriteLine("2. Зареєструватися");
    Console.WriteLine("0. Вийти з програми");
    Console.Write("Вибір: ");

    string choice = Console.ReadLine() ?? "";

    if (choice == "0") Environment.Exit(0);
    else if (choice == "1")
    {
        Console.Write("Введіть ID: ");
        string id = Console.ReadLine() ?? "";
        Console.Write("Введіть Ім'я: ");
        string name = Console.ReadLine() ?? "";

        var user = globalUsers.FirstOrDefault(u => u.Id == id && u.Name == name);
        if (user != null)
        {
            user.LogIn(name, id);
            currentUser = user;
            Console.WriteLine("Успішний вхід!");
        }
        else throw new Exception("Користувача не знайдено або дані невірні.");
    }
    else if (choice == "2")
    {
        Console.WriteLine("Оберіть роль (0 - Admin, 1 - Manager, 2 - Employee): ");
        Role role = (Role)int.Parse(Console.ReadLine() ?? "2");

        Console.Write("Введіть ID: "); string id = Console.ReadLine() ?? "";
        Console.Write("Введіть Ім'я: "); string name = Console.ReadLine() ?? "";

        if (globalUsers.Any(u => u.Id == id)) throw new Exception("Користувач з таким ID вже існує!");

        User newUser = role switch
        {
            Role.Admin => new Admin(name, id),
            Role.Manager => new Manager(name, id, 0, 0),
            Role.Employee => new Employee(name, id, 0, 0),
            _ => throw new Exception("Невідома роль")
        };

        globalUsers.Add(newUser);
        Console.WriteLine("Реєстрація успішна! Тепер ви можете увійти.");
        Console.ReadLine();
    }
}

// ==========================================
// МЕНЮ АДМІНІСТРАТОРА
// ==========================================
void ShowAdminMenu(Admin admin)
{
    Console.Clear();
    Console.WriteLine($"=== ПАНЕЛЬ АДМІНА ({admin.Name}) ===");
    Console.WriteLine("1. Додати користувача (Manager/Employee)");
    Console.WriteLine("2. Видалити користувача");
    Console.WriteLine("3. Список користувачів");
    Console.WriteLine("0. Вийти з акаунта");
    Console.Write("Вибір: ");

    string choice = Console.ReadLine() ?? "";

    if (choice == "0") { admin.LogOut(); currentUser = null; }
    else if (choice == "1")
    {
        Console.WriteLine("Роль (1 - Manager, 2 - Employee): ");
        Role role = (Role)int.Parse(Console.ReadLine() ?? "2");
        Console.Write("ID: "); string id = Console.ReadLine() ?? "";
        Console.Write("Ім'я: "); string name = Console.ReadLine() ?? "";
        Console.Write("Зарплата: "); decimal salary = decimal.Parse(Console.ReadLine() ?? "0");

        admin.Create(role, name, id, salary, 0, globalUsers);
        Console.WriteLine("Користувача створено!");
        Console.ReadLine();
    }
    else if (choice == "2")
    {
        Console.Write("Введіть ID користувача для видалення: ");
        string id = Console.ReadLine() ?? "";
        admin.Delete(id, globalUsers);
        Console.WriteLine("Користувача видалено (якщо він існував).");
        Console.ReadLine();
    }
    else if (choice == "3")
    {
        foreach (var u in globalUsers) Console.WriteLine($"- {u.GetType().Name}: {u.Name} (ID: {u.Id})");
        Console.ReadLine();
    }
}

// ==========================================
// МЕНЮ МЕНЕДЖЕРА
// ==========================================
void ShowManagerMenu(Manager manager)
{
    Console.Clear();
    Console.WriteLine($"=== ПАНЕЛЬ МЕНЕДЖЕРА ({manager.Name}) ===");
    Console.WriteLine("1. Створити завдання");
    Console.WriteLine("2. Видалити завдання");
    Console.WriteLine("3. Додати етап (Stage) до завдання");
    Console.WriteLine("4. Список завдань");
    Console.WriteLine("0. Вийти з акаунта");
    Console.Write("Вибір: ");

    string choice = Console.ReadLine() ?? "";

    if (choice == "0") { manager.LogOut(); currentUser = null; }
    else if (choice == "1")
    {
        Console.Write("Назва завдання: "); string name = Console.ReadLine() ?? "";
        Console.Write("Опис: "); string desc = Console.ReadLine() ?? "";
        manager.CreateTask(name, desc, DateTime.Now, null, DateTime.Now.AddDays(7), new List<Stage>(), globalTasks);
        Console.WriteLine("Завдання створено!");
        Console.ReadLine();
    }
    else if (choice == "2")
    {
        Console.Write("Введіть назву завдання для видалення: ");
        string name = Console.ReadLine() ?? "";
        var task = globalTasks.FirstOrDefault(t => t.Name == name);
        if (task != null) manager.DeleteTask(task, globalTasks);
        Console.WriteLine("Виконано.");
        Console.ReadLine();
    }
    else if (choice == "3")
    {
        Console.Write("Введіть назву завдання: ");
        string tName = Console.ReadLine() ?? "";
        var task = globalTasks.FirstOrDefault(t => t.Name == tName);
        if (task == null) throw new Exception("Завдання не знайдено");

        Console.Write("Назва етапу: "); string sName = Console.ReadLine() ?? "";
        Console.Write("Вага етапу (число): "); int weight = int.Parse(Console.ReadLine() ?? "1");

        manager.AddStage(task, sName, weight);
        Console.WriteLine("Етап додано!");
        Console.ReadLine();
    }
    else if (choice == "4")
    {
        foreach (var t in globalTasks)
        {
            Console.WriteLine($"- Завдання: {t.Name} (Прогрес: {t.CalulateProgress()}%)");
            foreach (var s in t.Stages)
            {
                string status = s.EndTime.HasValue ? "Виконано" : "В процесі";
                Console.WriteLine($"   -> Етап: {s.Name} [{status}]");
            }
        }
        Console.ReadLine();
    }
}

// ==========================================
// МЕНЮ РОБІТНИКА
// ==========================================
void ShowEmployeeMenu(Employee employee)
{
    Console.Clear();
    Console.WriteLine($"=== ПАНЕЛЬ РОБІТНИКА ({employee.Name}) ===");
    Console.WriteLine("1. Переглянути завдання та етапи");
    Console.WriteLine("2. Завершити етап");
    Console.WriteLine("0. Вийти з акаунта");
    Console.Write("Вибір: ");

    string choice = Console.ReadLine() ?? "";

    if (choice == "0") { employee.LogOut(); currentUser = null; }
    else if (choice == "1")
    {
        foreach (var t in globalTasks)
        {
            Console.WriteLine($"Завдання: {t.Name}");
            for (int i = 0; i < t.Stages.Count; i++)
            {
                string status = t.Stages[i].EndTime.HasValue ? "Виконано" : "Очікує";
                Console.WriteLine($"  {i}. {t.Stages[i].Name} - {status}");
            }
        }
        Console.ReadLine();
    }
    else if (choice == "2")
    {
        Console.Write("Введіть назву завдання: ");
        string tName = Console.ReadLine() ?? "";
        var task = globalTasks.FirstOrDefault(t => t.Name == tName);
        if (task == null) throw new Exception("Завдання не знайдено");

        Console.Write("Введіть номер етапу (зі списку вище): ");
        int sIndex = int.Parse(Console.ReadLine() ?? "0");

        if (sIndex >= 0 && sIndex < task.Stages.Count)
        {
            employee.CompleteStage(task.Stages[sIndex]);
            Console.WriteLine("Етап успішно завершено!");
        }
        else throw new Exception("Невірний номер етапу");

        Console.ReadLine();
    }
}