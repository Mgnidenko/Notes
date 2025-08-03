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
            /// break завершает лишь часть switch в то время как return полностью останавливает программу.
            /// </summary>
            while (true)
            {
                Console.WriteLine("Введите 1 чтобы создать заметку.\nВведите 2 чтобы вывести список заметок. \n" +
                    "Введите 3 чтобы прочитать заметку. \nВведите 4 для изменения заметки.\nВведите 5 для удаления заметки." +
                    "\nВведите 6 для выхода из приложения.");
                var userNumber = GetNumberFromUser("");


                switch (userNumber)
                {
                    case (int)Lop.CreateNote:
                        CreateFile();
                        break;

                    case (int)Lop.ShowListNotes:
                        ShowNotes(PATH);
                        break;

                    case (int)Lop.ReadingNote:
                        ReadNote();
                        break;

                    case (int)Lop.EditingNote:
                        ChangeNote(PATH);
                        break;

                    case (int)Lop.DeleteNote:
                        DeleteNote();
                        break;

                    case (int)Lop.Exit:
                        return;
                }
            }
        }

        /// <summary>
        /// Функция создает корневой каталог если его нет при запуске программы.
        /// </summary>
        static private void CreateDirectory()
        {
            if (!File.Exists(PATH))
            {
                Directory.CreateDirectory(PATH);
            }

        }

        /// <summary>
        /// Создает новый текстовый файл в папке "C:\\Notes" с уникальным именем, введенным  пользователем.
        /// Если файл с таким именем уже существует(без учета регистра и расширения), пользователь должен ввести новое имя.
        /// После создания файла пользователь вводит заголовок, который записывается в файл как его содержимое.
        /// </summary>
        /// <returns></returns>
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
        /// Функция показывает все файлы в каталоге Notes
        /// </summary>
        /// <param name="path"></param>
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
        /// Функция чтения заметки
        /// </summary>
        /// <param name="nameNote"></param>
        /// <param name="path"></param>
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
        /// Функция выводит содержимое файла и если пользователь хочет то может изменить содержимое
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
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
        /// Функция на недопустимость символов в названии заметки.
        /// </summary>
        /// <returns></returns>
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

        enum Lop
        {
            CreateNote = 1,
            ShowListNotes,
            ReadingNote,
            EditingNote,
            DeleteNote,
            Exit
        }

        private static int GetNumberFromUser(string message)
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
