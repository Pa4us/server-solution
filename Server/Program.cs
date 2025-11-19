using DataLibrary.Repositories;
using System.Net.Sockets;
using System.Net;
using DataLibrary.Models;
using Newtonsoft.Json;
using System.Text;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            var repository = new GazetteerRepository("data.txt");
            var server = new TcpListener(IPAddress.Any, 8888);

            server.Start();
            Console.WriteLine("Сервер запущен на порту 8888");

            while (true)
            {
                var client = server.AcceptTcpClient();
                Task.Run(() => HandleClient(client, repository));
            }
        }

        static void HandleClient(TcpClient client, GazetteerRepository repository)
        {
            try
            {
                var stream = client.GetStream();
                var buffer = new byte[4096];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine($"Получен запрос: {request}");

                var response = ProcessRequest(request, repository);
                var responseBytes = Encoding.UTF8.GetBytes(response);

                stream.Write(responseBytes, 0, responseBytes.Length);
                stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        static string ProcessRequest(string request, GazetteerRepository repository)
        {
            try
            {
                var parts = request.Split('|');
                var command = parts[0];
                var data = parts.Length > 1 ? parts[1] : null;

                switch (command)
                {
                    case "GET_ALL":
                        return JsonConvert.SerializeObject(repository.GetAll());

                    case "GET_BY_ID":
                        return JsonConvert.SerializeObject(repository.GetById(int.Parse(data)));

                    case "ADD":
                        var newItem = JsonConvert.DeserializeObject<Gazetteer>(data);
                        repository.Add(newItem);
                        return "OK";

                    case "UPDATE":
                        var updateItem = JsonConvert.DeserializeObject<Gazetteer>(data);
                        repository.Update(updateItem);
                        return "OK";

                    case "DELETE":
                        repository.Delete(int.Parse(data));
                        return "OK";

                    case "GET_BY_CONTINENT":
                        return JsonConvert.SerializeObject(repository.GetByContinent(data));

                    case "GET_BY_POPULATION":
                        var range = data.Split('-');
                        return JsonConvert.SerializeObject(repository.GetByPopulationRange(
                            int.Parse(range[0]), int.Parse(range[1])));

                    default:
                        return "ERROR: Unknown command";
                }
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }
    }
}
