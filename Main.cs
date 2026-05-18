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

            ShowDeadlineWarnings(currentUser); //виводить інформацію про прострочений дедлайн якщо юзер робітник або менеджер
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
    Console.WriteLine("5. Редагувати завдання (Edit Task)");
    Console.WriteLine("6. Редагувати етап (Edit Stage)");
    Console.WriteLine("7. Видалити етап (Delete Stage)");
    Console.WriteLine("8. Загальний витрачений час (Calculate Total Time)");
    Console.WriteLine("9. Додати коментар до завдання (Create Comment)");
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

        if (task.Stages.Any())
        {
            task.Stages.Last().StartTime = DateTime.Now;
        }

        Console.WriteLine("Етап додано! Відлік часу для цього етапу розпочато.");
        Console.ReadLine();
    }
    else if (choice == "4")
    {
        foreach (var t in globalTasks)
        {
            string deadlineStr = t.deadLine.HasValue ? t.deadLine.Value.ToString("dd.MM.yyyy HH:mm") : "Немає";
            string warning = (t.deadLine.HasValue && t.deadLine.Value < DateTime.Now && t.CalulateProgress() < 100)
                             ? " [ПРОСТРОЧЕНО]" : "";

            Console.WriteLine($"- Завдання: {t.Name} (Прогрес: {t.CalulateProgress()}%){warning} | Дедлайн: {deadlineStr}");

            foreach (var s in t.Stages)
            {
                string status = s.EndTime.HasValue ? "Виконано" : "В процесі";
                Console.WriteLine($"   -> Етап: {s.Name} (Вага: {s.Weight}) [{status}]");
            }
            if (t.Comments.Any())
            {
                Console.WriteLine("   -> Коментарі:");
                foreach (var c in t.Comments)
                    Console.WriteLine($"      [{c.Author.Name}]: {c.Message}");
            }
        }
        Console.ReadLine();
    }
    else if (choice == "5")
    {
        Console.Write("Введіть точну назву завдання для редагування: ");
        string inputName = Console.ReadLine() ?? "";
        var task = globalTasks.FirstOrDefault(t => t.Name == inputName);
        if (task == null) throw new Exception("Завдання не знайдено");

        Console.Write("Нова назва (натисніть Enter, щоб не змінювати): ");
        string nName = Console.ReadLine() ?? "";
        Console.Write("Новий опис (натисніть Enter, щоб не змінювати): ");
        string nDesc = Console.ReadLine() ?? "";

        manager.EditTask(task,
            string.IsNullOrWhiteSpace(nName) ? null : nName,
            string.IsNullOrWhiteSpace(nDesc) ? null : nDesc);
        Console.WriteLine("Завдання успішно відредаговано!");
        Console.ReadLine();
    }
    else if (choice == "6")
    {
        Console.Write("Введіть назву завдання: ");
        string taskName = Console.ReadLine() ?? "";
        var task = globalTasks.FirstOrDefault(t => t.Name == taskName);
        if (task == null) throw new Exception("Завдання не знайдено");

        Console.Write("Введіть назву етапу: ");
        string stageName = Console.ReadLine() ?? "";
        var stage = task.Stages.FirstOrDefault(s => s.Name == stageName);
        if (stage == null) throw new Exception("Етап не знайдено");

        Console.Write("Нова назва етапу (натисніть Enter, щоб не змінювати): ");
        string nName = Console.ReadLine() ?? "";
        Console.Write("Нова вага етапу (натисніть Enter, щоб не змінювати): ");
        string wStr = Console.ReadLine() ?? "";
        int? nWeight = string.IsNullOrWhiteSpace(wStr) ? null : int.Parse(wStr);

        manager.EditStage(stage, string.IsNullOrWhiteSpace(nName) ? null : nName, nWeight);
        Console.WriteLine("Етап успішно відредаговано!");
        Console.ReadLine();
    }
    else if (choice == "7")
    {
        Console.Write("Введіть назву завдання: ");
        string taskName = Console.ReadLine() ?? "";
        var task = globalTasks.FirstOrDefault(t => t.Name == taskName);
        if (task == null) throw new Exception("Завдання не знайдено");

        Console.Write("Введіть назву етапу для видалення: ");
        string stageName = Console.ReadLine() ?? "";
        var stage = task.Stages.FirstOrDefault(s => s.Name == stageName);
        if (stage == null) throw new Exception("Етап не знайдено");

        manager.DeleteStage(task, stage);
        Console.WriteLine("Етап видалено!");
        Console.ReadLine();
    }
    else if (choice == "8")
    {
        Console.Write("Введіть назву завдання: ");
        string taskName = Console.ReadLine() ?? "";
        var task = globalTasks.FirstOrDefault(t => t.Name == taskName);
        if (task == null) throw new Exception("Завдання не знайдено");

        TimeSpan total = task.CalculateTotalTime();
        Console.WriteLine($"Загальний зафіксований час: {total.Days} дн, {total.Hours} год, {total.Minutes} хв, {total.Seconds} сек.");
        Console.ReadLine();
    }
    else if (choice == "9")
    {
        Console.Write("Введіть назву завдання: ");
        string taskName = Console.ReadLine() ?? "";
        var task = globalTasks.FirstOrDefault(t => t.Name == taskName);
        if (task == null) throw new Exception("Завдання не знайдено");

        Console.Write("Введіть текст коментаря: ");
        string msg = Console.ReadLine() ?? "";
        manager.CreateComment(msg, task);
        Console.WriteLine("Коментар додано!");
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
    Console.WriteLine("1. Переглянути завдання, етапи та коментарі");
    Console.WriteLine("2. Завершити етап");
    Console.WriteLine("3. Додати коментар до завдання (Create Comment)");
    Console.WriteLine("0. Вийти з акаунта");
    Console.Write("Вибір: ");

    string choice = Console.ReadLine() ?? "";

    if (choice == "0") { employee.LogOut(); currentUser = null; }
    else if (choice == "1")
    {
        foreach (var t in globalTasks)
        {
            string deadlineStr = t.deadLine.HasValue ? t.deadLine.Value.ToString("dd.MM.yyyy HH:mm") : "Немає";
            string warning = (t.deadLine.HasValue && t.deadLine.Value < DateTime.Now && t.CalulateProgress() < 100)
                             ? " [ПРОСТРОЧЕНО]" : "";

            Console.WriteLine($"Завдання: {t.Name}{warning} | Дедлайн: {deadlineStr}");
            for (int i = 0; i < t.Stages.Count; i++)
            {
                string status = t.Stages[i].EndTime.HasValue ? "Виконано" : "Очікує";
                Console.WriteLine($"  {i}. {t.Stages[i].Name} - {status}");
            }
            if (t.Comments.Any())
            {
                Console.WriteLine("  Коментарі:");
                foreach (var c in t.Comments)
                    Console.WriteLine($"    [{c.Author.Name}]: {c.Message}");
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
            Console.WriteLine("Етап успішно завершено! Час зафіксовано.");
        }
        else throw new Exception("Невірний номер етапу");

        Console.ReadLine();
    }
    else if (choice == "3")
    {
        Console.Write("Введіть назву завдання: ");
        string taskName = Console.ReadLine() ?? "";
        var task = globalTasks.FirstOrDefault(t => t.Name == taskName);
        if (task == null) throw new Exception("Завдання не знайдено");

        Console.Write("Введіть текст коментаря: ");
        string msg = Console.ReadLine() ?? "";
        employee.CreateComment(msg, task);
        Console.WriteLine("Коментар відправлено!");
        Console.ReadLine();
    }
}

// ==========================================
// ФУНКЦІЯ ПОПЕРЕДЖЕННЯ ПРО ДЕДЛАЙНИ
// ==========================================
void ShowDeadlineWarnings(User user)
{
    bool hasWarnings = false;

    foreach (var t in globalTasks)
    {
        // Перевіряємо, чи встановлений дедлайн, чи пройшов час і чи завдання не завершене
        if (t.deadLine.HasValue && t.deadLine.Value < DateTime.Now && t.CalulateProgress() < 100)
        {
            if (!hasWarnings)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
            }

            if (user is Employee)
            {
                Console.WriteLine($"[ПОПЕРЕДЖЕННЯ] Завдання '{t.Name}' прострочено! Дедлайн був: {t.deadLine.Value}");
            }
            else if (user is Manager)
            {
                Console.WriteLine($"[ПОВІДОМЛЕННЯ ПРО ЗАТРИМКУ] Завдання '{t.Name}' не виконано вчасно! Дедлайн: {t.deadLine.Value}");
            }

            hasWarnings = true;
        }
    }

    if (hasWarnings)
    {
        Console.ResetColor();
        Console.WriteLine("\nНатисніть Enter для переходу до панелі керування...");
        Console.ReadLine();
    }
}