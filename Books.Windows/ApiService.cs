using BooksAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Windows
{
    public class ApiService
    {
        HttpClient _httpClient = new HttpClient();
        public ApiService()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:7073/api/book/");
        }

        public List<Book> GetBooks()
        {
            var json = _httpClient.GetStringAsync("getall").Result;
            var books = JsonConvert.DeserializeObject<List<Book>>(json);

            return books;
        }

        public void AddBook(Book book)
        {
            var json = JsonConvert.SerializeObject(book);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var res = _httpClient.PostAsync("add", data);
        }
    }
}
