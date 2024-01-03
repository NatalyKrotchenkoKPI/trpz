using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public partial class Form1 : Form
	{
		private string connectionString = @"Data Source=DESKTOP-C85P3IS\SQLTRPZ; Initial Catalog=MedicalCenter; Integrated Security=True;";

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(connectionString))
				{
					conn.Open();
					string query = "SELECT RoleName FROM Roles";
					SqlCommand cmd = new SqlCommand(query, conn);
					SqlDataAdapter da = new SqlDataAdapter(cmd);
					DataTable dt = new DataTable();
					da.Fill(dt);

					Rol.Items.Clear();

					foreach (DataRow row in dt.Rows)
					{
						Rol.Items.Add(row["RoleName"].ToString());
					}

					if (Rol.Items.Count > 0)
						Rol.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Помилка при завантаженні ролей: " + ex.Message);
			}
		}
		private void LoginButton_Click(object sender, EventArgs e)
		{
			string username = Login.Text;
			string password = Password.Text;
			string selectedRole = Rol.SelectedItem.ToString();

			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(selectedRole))
			{
				MessageBox.Show("Будь ласка, заповніть усі поля.");
				return;
			}

			User user = AuthenticateUser(username, password, selectedRole);
			if (user.Authenticated)
			{
				if (user.Role == "Лікар")
				{
					Form3 form3 = new Form3();
					form3.CurrentUser = username;
					this.Hide();
					form3.ShowDialog(); // Використовуємо ShowDialog для модального вікна
					this.Show(); // Показати Form1 знову після закриття Form3
				}
				else if(user.Role =="Пацієнт")
				{
					Form2 form2 = new Form2();
					form2.CurrentUser = username;
					this.Hide();
					form2.ShowDialog(); // Використовуємо ShowDialog для модального вікна
					this.Show(); // Показати Form1 знову після закриття Form2
				}
				else
				{
					Form4 form4 = new Form4();
					form4.CurrentUser = username;
					this.Hide();
					form4.ShowDialog(); // Використовуємо ShowDialog для модального вікна
					this.Show(); // Показати Form1 знову після закриття Form2
				}
			}
			else
			{
				MessageBox.Show("Неправильний логін або пароль.");
			}
		}

		private User AuthenticateUser(string username, string password, string role)
		{
			User user = new User();
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				try
				{
					conn.Open();
					string query = @"
                SELECT u.Password, r.RoleName 
                FROM Users u
                INNER JOIN Roles r ON u.RoleID = r.RoleID 
                WHERE u.Username = @Username AND r.RoleName = @RoleName";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Username", username);
						cmd.Parameters.AddWithValue("@RoleName", role);

						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								string storedPassword = reader["Password"].ToString();
								user.Authenticated = storedPassword == password;
								user.Role = reader["RoleName"].ToString();
							}
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Сталася помилка при спробі входу: " + ex.Message);
				}
			}
			return user;
		}

		public class User
		{
			public bool Authenticated { get; set; }
			public string Role { get; set; }
		}
	}
	
}