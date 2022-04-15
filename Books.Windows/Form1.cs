using BooksAPI.Models;

namespace Books.Windows
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ApiService apiService = new ApiService();
            dataGridView1.DataSource = apiService.GetBooks();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Book book = new Book()
            {
                Name = name.Text,
                Author = author.Text,
                Price = double.Parse(price.Text)
            };

            ApiService apiService = new ApiService();
            apiService.AddBook(book);
            dataGridView1.DataSource = apiService.GetBooks();
        }
    }
}