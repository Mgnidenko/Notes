using System.IO;
using System.Text;

namespace Notes
{
    internal class Program
    {
        const string PATH = "C:\\Notes";
        static void Main(string[] args)
        {
            CreateDirectory();

            ///<summary>
            ///  return полностью останавливает цикл, break переходит на новую итерацию.
            /// </summary>а
            while (true)
            {
                Console.WriteLine("Введите 1 чтобы создать заметку.\nВведите 2 чтобы вывести список заметок. \n" +
                    "Введите 3 чтобы прочитать заметку. \nВведите 4 для изменения заметки.\nВведите 5 для удаления заметки." +
                    "\nВведите 6 для выхода из приложения.");

                int userNumber = GetNumberFromUser();
                if (userNumber >= 1 && userNumber <= 6)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Введите число из диапазона от 1 до 6");
                    continue;
                }


                switch (userNumber)
                {
                    case (int)UserAction.CreateNote:
                        CreateFile();
                        break;

                    case (int)UserAction.ShowListNotes:
                        ShowNotes(PATH);
                        break;

                    case (int)UserAction.ReadingNote:
                        ReadNote();
                        break;

                    case (int)UserAction.EditingNote:
                        ChangeNote(PATH);
                        break;

                    case (int)UserAction.DeleteNote:
                        DeleteNote();
                        break;

                    case (int)UserAction.Exit:
                        return;
                }
            }
        }

        /// <summary>
        /// Создает корневой каталог если его нет при запуске программы.
        /// </summary>
        static private void CreateDirectory()
        {
            if (!Directory.Exists(PATH))
            {
                Directory.CreateDirectory(PATH);
            }

        }

        /// <summary>
        /// Создает новый текстовый файл в папке "C:\\Notes" с уникальным именем, введенным  пользователем.
        /// Если файл с таким именем уже существует(без учета регистра и расширения), пользователь должен ввести новое имя.
        /// После создания файла пользователь вводит заголовок, который записывается в файл как его содержимое.
        /// </summary>
        static private void CreateFile()
        {
            Console.Write("Введите название вашего файла: ");
            DirectoryInfo directoryInfo = new DirectoryInfo("C:\\Notes");
            FileInfo[] allNotes = directoryInfo.GetFiles();
            string? nameNote = CheckForbiddenChars();

            while (true)
            {
                bool nameExists = allNotes.Any(oneNote => oneNote.Name.Substring(0, oneNote.Name.Length - 4).ToLower() == nameNote.ToLower());

                if (nameExists)
                {
                    Console.WriteLine("Такой файл уже есть");
                    nameNote = CheckForbiddenChars();
                }
                else
                {
                    Console.WriteLine("Имя уникально, можно продолжать.");
                    break;
                }
            }
            string filePath = PATH + "\\" + nameNote + ".txt";
            File.Create(filePath).Close();
            Console.Write("Введите первый заголовок: ");
            string? title = Console.ReadLine();
            File.WriteAllText(filePath, title);

            Console.WriteLine($"Файл: {nameNote} создан.");
        }




        /// <summary>
        /// Показывает все файлы в каталоге Notes.
        /// </summary>
        /// <param name="path"> Путь к каталогу. </param>
        static private void ShowNotes(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            if (directoryInfo.Exists)
            {
                FileInfo[] allNotes = directoryInfo.GetFiles();

                foreach (var oneNote in allNotes)
                {
                    Console.WriteLine(oneNote.Name);
                }
            }
            else
            {
                Console.WriteLine("Такой папки не существует.");
            }
        }

        /// <summary>
        /// Пользователь вводит название файла который хочет прочитать.
        /// </summary>
        /// <param name="nameNote"> Название файла. </param>
        /// <param name="path"> Путь к файлу. </param>
        static private void ReadNote()
        {
            Console.Write("Введите название файла который хотите прочитать: ");
            string? nameNote = CheckForbiddenChars();
            string? textNote = " ";
            try
            {
                StreamReader readTextFromFile = new StreamReader(PATH + "\\" + nameNote + ".txt");

                while (textNote is not null)
                {
                    Console.Write(textNote + "\n");
                    textNote = readTextFromFile.ReadLine();
                }
                readTextFromFile.Close();
                Console.WriteLine();
            }
            catch (Exception error)
            {
                Console.WriteLine("Exception: " + error.Message);
            }
            finally
            {

            }
        }


        /// <summary>
        /// Выводит содержимое файла и если пользователь хочет то может изменить содержимое.
        /// </summary>
        /// <param name="name"> Название файла. </param>
        /// <param name="path"> Путь к файлу. </param>
        static private void ChangeNote(string path)
        {

            Console.Write("Введите название вашего файла: ");
            string? name = CheckForbiddenChars();
            string notePath = path + "\\" + name + ".txt";

            TextReader reader = new StreamReader(notePath);
            string contents = reader.ReadToEnd();
            reader.Close();
            Console.Write("Содержимое файла: ");
            Console.WriteLine(contents);


            while (true)
            {
                Console.WriteLine("Если вы хотите ввести новый текст то введите да если хотите оставить старый текст то введите нет: ");
                string? keyWord = Console.ReadLine();

                if (keyWord == "да" || keyWord == "Да")
                {
                    // Полностью перезаписываю выбранный файл.
                    try
                    {
                        StreamWriter writeFile = new StreamWriter(notePath, false, Encoding.UTF8);
                        Console.Write("Введите ваш текст: ");
                        string? userWords = Console.ReadLine();
                        writeFile.WriteLine(userWords);
                        writeFile.Close();
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error.Message);
                    }
                    finally
                    {
                        Console.WriteLine();
                    }
                    break;
                }
                else if (keyWord == "нет" || keyWord == "Нет")
                {
                    // Оставляю содержимое файла как есть.
                    break;
                }
            }
        }


        /// <summary>
        /// Проверка на недопустимость символов в названии файла.
        /// </summary>
        /// <returns> Возвращает проверенное название.</returns>
        static private string? CheckForbiddenChars()
        {
            while (true)
            {
                string? userInput = Console.ReadLine();
                char[] spl_chars = { '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '.', '/', ',', '<', '>', '?', '|', '{', '}', '[', ']', ';', ':', '|', };
                if (String.IsNullOrEmpty(userInput) || userInput.IndexOfAny(spl_chars) >= 0)
                {
                    Console.WriteLine("Вы ввели пустое значение или недопустимый символ введи название еще раз: ");
                    userInput = Console.ReadLine();

                    continue;
                }
                else if (userInput == null)
                {
                    Console.Write("Введите название файла заново: ");
                    continue;
                }
                else
                {
                    return userInput;
                }
            }
        }

        /// <summary>
        /// Полностью удаляет выбранный файл.
        /// </summary>
        static private void DeleteNote()
        {
            while (true)
            {
                Console.Write("Введите название файла: ");
                string? nameNote = CheckForbiddenChars();
                string path = PATH + "\\" + nameNote + ".txt";
                FileInfo fileInfo = new FileInfo(path);

                if (fileInfo.Exists)
                {
                    Console.WriteLine("Файл удален");
                    File.Delete(path);
                    break;

                }
                else
                {
                    Console.WriteLine("Такого файла нет");
                    continue;
                }
            }
        }

        enum UserAction
        {
            CreateNote = 1,
            ShowListNotes,
            ReadingNote,
            EditingNote,
            DeleteNote,
            Exit
        }

        /// <summary>
        /// Получает пользовательский ввод числа. Преобразует его в int и возвращает.
        /// Если пользователь ввёл не число, то цикл продолжит запрашивать ввод.
        /// </summary>
        /// <returns>Возвращает преобразованное из пользовательского ввода целое число.</returns>
        private static int GetNumberFromUser()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (int.TryParse(input, out var number))
                {
                    return number;
                }
                PrintError("Введите число");
            }
        }

        private static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

    }
}
