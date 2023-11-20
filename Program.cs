using System;
using System.IO;
using System.Xml;

public class Figure
{
    public string Name { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public Figure(string name, double width, double height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}

public class FileHandler
{
    private string filePath;

    public FileHandler(string path)
    {
        filePath = path;
    }

    public Figure LoadData()
    {
        Figure figure = null;
        string extension = Path.GetExtension(filePath).ToLower();

        try
        {
            string fileContent = File.ReadAllText(filePath);

            switch (extension)
            {
                case ".json":
                    figure = DeserializeJson(fileContent);
                    break;
                case ".xml":
                    figure = DeserializeXml(fileContent);
                    break;
                case ".txt":
                default:
                    string[] lines = fileContent.Split('\n');
                    string name = lines[0].Trim();
                    double width = double.Parse(lines[1].Trim());
                    double height = double.Parse(lines[2].Trim());
                    figure = new Figure(name, width, height);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
        }

        return figure;
    }

    public void SaveData(Figure figure)
    {
        string extension = Path.GetExtension(filePath).ToLower();

        try
        {
            switch (extension)
            {
                case ".json":
                    string jsonContent = SerializeJson(figure);
                    File.WriteAllText(filePath, jsonContent);
                    break;
                case ".xml":
                    string xmlContent = SerializeXml(figure);
                    File.WriteAllText(filePath, xmlContent);
                    break;
                case ".txt":
                default:
                    string txtContent = $"{figure.Name}\n{figure.Width}\n{figure.Height}";
                    File.WriteAllText(filePath, txtContent);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
        }
    }

    private string SerializeJson(Figure figure)
    {
        return $"{{\"Name\":\"{figure.Name}\",\"Width\":{figure.Width},\"Height\":{figure.Height}}}";
    }

    private Figure DeserializeJson(string jsonContent)
    {
        string[] parts = jsonContent
            .Trim('{', '}')
            .Split(new[] { "\",\"", "\":", "," }, StringSplitOptions.None);

        string name = parts[1];
        double width = double.Parse(parts[3]);
        double height = double.Parse(parts[5]);

        return new Figure(name, width, height);
    }

    private string SerializeXml(Figure figure)
    {
        return $"<Figure><Name>{figure.Name}</Name><Width>{figure.Width}</Width><Height>{figure.Height}</Height></Figure>";
    }

    private Figure DeserializeXml(string xmlContent)
    {
        using (var stringReader = new StringReader(xmlContent))
        {
            using (var xmlReader = XmlReader.Create(stringReader))
            {
                xmlReader.ReadToFollowing("Name");
                string name = xmlReader.ReadElementContentAsString();

                xmlReader.ReadToFollowing("Width");
                double width = xmlReader.ReadElementContentAsDouble();

                xmlReader.ReadToFollowing("Height");
                double height = xmlReader.ReadElementContentAsDouble();

                return new Figure(name, width, height);
            }
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Введите путь к файлу:");
        string filePath = Console.ReadLine();
        FileHandler fileHandler = new FileHandler(filePath);

        Figure loadedFigure = fileHandler.LoadData();

        if (loadedFigure != null)
        {
            Console.WriteLine("Данные загружены успешно:");
            Console.WriteLine($"Название: {loadedFigure.Name}");
            Console.WriteLine($"Ширина: {loadedFigure.Width}");
            Console.WriteLine($"Высота: {loadedFigure.Height}");

            Console.WriteLine("Хотите изменить данные? (Да/Нет)");
            string response = Console.ReadLine();

            if (response.ToLower() == "да")
            {
                Console.WriteLine("Введите новую ширину:");
                double newWidth = double.Parse(Console.ReadLine());
                loadedFigure.Width = newWidth;

                fileHandler.SaveData(loadedFigure);
                Console.WriteLine("Данные сохранены успешно.");
            }
        }
        else
        {
            Console.WriteLine("Не удалось загрузить данные.");
        }

        Console.WriteLine("Нажмите Escape для завершения программы.");
        while (Console.ReadKey().Key != ConsoleKey.Escape) ;
    }
}
