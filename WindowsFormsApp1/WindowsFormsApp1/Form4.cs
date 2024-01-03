using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public partial class Form4 : Form
	{

		private string connectionString = @"Data Source=DESKTOP-C85P3IS\SQLTRPZ; Initial Catalog=MedicalCenter; Integrated Security=True;";
		public string CurrentUser { get; set; }

		public Form4()
		{
			InitializeComponent();
			LoadRoles();
			LoadUsersData();
			LoadPatientsData();
			LoadStaffData();
			LoadCabinetsData();
			LoadScheduleData();
			LoadMedicinesData();
			LoadMedicalRecordsData();
			LoadVisitLogsData();
			LoadMedicationsData();
			LoadHealthMonitoringData();

			this.button7.Click += new EventHandler(this.button7_Click);
			this.button4.Click += new EventHandler(this.button4_Click);
			this.button5.Click += new EventHandler(this.button5_Click);
			this.button6.Click += new EventHandler(this.button6_Click);

			this.button8.Click += new EventHandler(this.button8_Click);
			this.button9.Click += new EventHandler(this.button9_Click);
			this.button10.Click += new EventHandler(this.button10_Click);
			this.button11.Click += new EventHandler(this.button11_Click);
			this.button12.Click += new EventHandler(this.button12_Click);
			this.button13.Click += new EventHandler(this.button13_Click);
			this.button14.Click += new EventHandler(this.button14_Click);

			this.button15.Click += new EventHandler(this.button15_Click);
			this.button16.Click += new EventHandler(this.button16_Click);
			this.button17.Click += new EventHandler(this.button17_Click);

			this.button18.Click += new EventHandler(this.button18_Click);
			this.button19.Click += new EventHandler(this.button19_Click);
			this.button20.Click += new EventHandler(this.button20_Click);

			this.button21.Click += new EventHandler(this.button21_Click);
			this.button22.Click += new EventHandler(this.button22_Click);
			this.button23.Click += new EventHandler(this.button23_Click);

			this.button24.Click += new EventHandler(this.button24_Click);
			this.button25.Click += new EventHandler(this.button25_Click);
			this.button26.Click += new EventHandler(this.button26_Click);

			this.button27.Click += new EventHandler(this.button27_Click);
			this.button28.Click += new EventHandler(this.button28_Click);
			this.button29.Click += new EventHandler(this.button29_Click);

			this.button30.Click += new EventHandler(this.button30_Click);
			this.button31.Click += new EventHandler(this.button31_Click);
			this.button32.Click += new EventHandler(this.button32_Click);
		}

		private void Form4_Load(object sender, EventArgs e)
		{

		}
		private void LoadRoles()
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand("SELECT RoleID, RoleName FROM Roles", conn))
				{
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							comboBox1.Items.Add(new { Text = reader["RoleName"].ToString(), Value = reader["RoleID"] });
						}
					}
				}
			}
			comboBox1.DisplayMember = "Text";
			comboBox1.ValueMember = "Value";
		}

		private void LoadUsersData()
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				// SQL запит для отримання даних про всіх користувачів
				string query = @"
            SELECT u.UserID, u.Username, u.Password, r.RoleName
            FROM Users u
            JOIN Roles r ON u.RoleID = r.RoleID";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					SqlDataAdapter da = new SqlDataAdapter(cmd);
					DataTable dt = new DataTable();
					da.Fill(dt);

					// Прив'язка DataTable до DataGridView
					dataGridView1.DataSource = dt;
				}
			}

			// Налаштування видимості стовпців або заголовків, якщо потрібно
			dataGridView1.Columns["UserID"].Visible = false; // Якщо не хочемо показувати ID користувача
			dataGridView1.Columns["Username"].HeaderText = "Ім'я користувача";
			dataGridView1.Columns["Password"].HeaderText = "Пароль";
			dataGridView1.Columns["RoleName"].HeaderText = "Роль";
		}

		private void button1_Click_1(object sender, EventArgs e)
		{

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand("INSERT INTO Users (UserID, Username, Password, RoleID) VALUES (@UserID, @Username, @Password, @RoleID)", conn))
				{
					int userId = int.Parse(textBox3.Text); // textBoxUserID - це текстове поле для вводу ID користувача
					cmd.Parameters.AddWithValue("@UserID", userId);
					cmd.Parameters.AddWithValue("@Username", textBox1.Text);
					cmd.Parameters.AddWithValue("@Password", textBox2.Text);
					cmd.Parameters.AddWithValue("@RoleID", ((dynamic)comboBox1.SelectedItem).Value);

					// Перед вставкою варто перевірити, чи не існує вже користувача з таким UserID
					if (IsUserIdAvailable(userId, conn))
					{
						cmd.ExecuteNonQuery();
						MessageBox.Show("Користувача створено.");
						LoadUsersData(); // Оновлюємо DataGridView
					}
					else
					{
						MessageBox.Show("Користувач з таким UserID вже існує.");
					}
				}
			}
		}

		private bool IsUserIdAvailable(int userId, SqlConnection conn)
		{
			using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE UserID = @UserID", conn))
			{
				cmd.Parameters.AddWithValue("@UserID", userId);
				int userCount = (int)cmd.ExecuteScalar();
				return userCount == 0;
			}
		}

		private void button2_Click_1(object sender, EventArgs e)
		{
			// Припустимо, що в textBoxUserID знаходиться UserID користувача, якого потрібно оновити
			int userIdToUpdate = int.Parse(textBox3.Text);

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand("UPDATE Users SET Username = @Username, Password = @Password, RoleID = @RoleID WHERE UserID = @UserID", conn))
				{
					cmd.Parameters.AddWithValue("@Username", textBox1.Text);
					cmd.Parameters.AddWithValue("@Password", textBox2.Text); // У реальній системі пароль повинен бути захешований
					cmd.Parameters.AddWithValue("@RoleID", ((dynamic)comboBox1.SelectedItem).Value);
					cmd.Parameters.AddWithValue("@UserID", userIdToUpdate);

					int rowsAffected = cmd.ExecuteNonQuery();
					if (rowsAffected > 0)
					{
						MessageBox.Show("Користувача оновлено.");
						LoadUsersData();
					}
					else
					{
						MessageBox.Show("Користувача не знайдено або дані не були змінені.");
					}
				}
			}
		}

		private void button3_Click_1(object sender, EventArgs e)
		{
			// Припустимо, що в textBoxUserID знаходиться UserID користувача, якого потрібно видалити
			int userIdToDelete = int.Parse(textBox3.Text);

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserID = @UserID", conn))
				{
					cmd.Parameters.AddWithValue("@UserID", userIdToDelete);

					int rowsAffected = cmd.ExecuteNonQuery();
					if (rowsAffected > 0)
					{
						MessageBox.Show("Користувача видалено.");
						LoadUsersData();
					}
					else
					{
						MessageBox.Show("Користувача не знайдено або він вже був видалений.");
					}
				}
			}
		}

		private void button7_Click(object sender, EventArgs e)
		{
			// Пошук пацієнта та вивід його даних у поля форми
			FindPatient();
		}

		// Кнопка "Створити" - створення нового пацієнта
		private void button4_Click(object sender, EventArgs e)
		{
			// Створення нового пацієнта
			CreatePatient();
		}

		// Кнопка "Оновити" - оновлення даних існуючого пацієнта
		private void button5_Click(object sender, EventArgs e)
		{
			// Оновлення даних пацієнта
			UpdatePatient();
		}

		// Кнопка "Видалити" - видалення пацієнта
		private void button6_Click(object sender, EventArgs e)
		{
			// Видалення пацієнта
			DeletePatient();
		}

		private void CreatePatient()
		{
			// Збирання даних з форми
			int patientId = int.Parse(textBox4.Text); // Припускаємо, що у вас є TextBox для ID пацієнта
			int userId = int.Parse(textBox11.Text); // Отримуємо UserID
			string firstName = textBox5.Text;
			string lastName = textBox6.Text;
			string middleName = textBox7.Text;
			string gender = textBox8.Text;
			DateTime birthDate = dateTimePicker1.Value;
			string contact = textBox10.Text;

			// Розділяємо адресу на вулицю та місто
			var addressDetails = textBox9.Text.Split(new string[] { ", " }, StringSplitOptions.None);
			string street = addressDetails[0];
			string city = addressDetails.Length > 1 ? addressDetails[1] : "";
			int addressId = InsertAddress(street, city); // Припускаємо, що у вас є метод InsertAddress

			// Створення SQL-запиту для вставки
			string query = "INSERT INTO Patients (PatientID, UserID, PatientName, PatientLastname, PatientSurname, Gender, BirthDate, AddressID, Contact) " +
						   "VALUES (@PatientID, @UserID, @FirstName, @LastName, @MiddleName, @Gender, @BirthDate, @AddressID, @Contact)";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					// Встановлення параметрів для команди
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("userId", userId);
					cmd.Parameters.AddWithValue("@FirstName", firstName);
					cmd.Parameters.AddWithValue("@LastName", lastName);
					cmd.Parameters.AddWithValue("@MiddleName", middleName);
					cmd.Parameters.AddWithValue("@Gender", gender);
					cmd.Parameters.AddWithValue("@BirthDate", birthDate);
					cmd.Parameters.AddWithValue("@AddressID", addressId);
					cmd.Parameters.AddWithValue("@Contact", contact);

					// Виконання команди
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}

			// Оновлення DataGridView
			LoadPatientsData();
		}

		// Метод для оновлення даних існуючого пацієнта
		private void UpdatePatient()
		{
			// Збирання даних з форми
			int patientId = int.Parse(textBox4.Text); // Припускаємо, що у вас є TextBox для ID пацієнта
			string firstName = textBox5.Text;
			string lastName = textBox6.Text;
			string middleName = textBox7.Text;
			string gender = textBox8.Text;
			DateTime birthDate = dateTimePicker1.Value;
			string contact = textBox10.Text;

			// Розділяємо адресу на вулицю та місто
			var addressDetails = textBox9.Text.Split(new string[] { ", " }, StringSplitOptions.None);
			string street = addressDetails[0];
			string city = addressDetails.Length > 1 ? addressDetails[1] : "";
			int addressId = UpdateOrInsertAddress(street, city, patientId); // Припускаємо, що у вас є метод UpdateOrInsertAddress, який оновлює адресу, якщо вона існує, або створює нову

			// Створення SQL-запиту для оновлення
			string query = "UPDATE Patients SET PatientName = @FirstName, PatientLastname = @LastName, " +
						   "PatientSurname = @MiddleName, Gender = @Gender, BirthDate = @BirthDate, " +
						   "AddressID = @AddressID, Contact = @Contact WHERE PatientID = @PatientID";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					// Встановлення параметрів для команди
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("@FirstName", firstName);
					cmd.Parameters.AddWithValue("@LastName", lastName);
					cmd.Parameters.AddWithValue("@MiddleName", middleName);
					cmd.Parameters.AddWithValue("@Gender", gender);
					cmd.Parameters.AddWithValue("@BirthDate", birthDate);
					cmd.Parameters.AddWithValue("@AddressID", addressId);
					cmd.Parameters.AddWithValue("@Contact", contact);

					// Виконання команди
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}

			// Оновлення DataGridView
			LoadPatientsData();
		}

		// Метод для видалення пацієнта
		private void DeletePatient()
		{
			int patientId = int.Parse(textBox4.Text); // Припускаємо, що у вас є TextBox для ID пацієнта

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				// Отримуємо AddressID пацієнта перед видаленням
				int addressId = GetPatientAddressId(patientId, conn);

				// Видаляємо запис пацієнта
				string deletePatientQuery = "DELETE FROM Patients WHERE PatientID = @PatientID";
				using (SqlCommand cmd = new SqlCommand(deletePatientQuery, conn))
				{
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					int result = cmd.ExecuteNonQuery();
					if (result > 0)
					{
						// Якщо пацієнта видалено, видаляємо його адресу
						string deleteAddressQuery = "DELETE FROM Addresses WHERE AddressID = @AddressID";
						using (SqlCommand deleteAddressCmd = new SqlCommand(deleteAddressQuery, conn))
						{
							deleteAddressCmd.Parameters.AddWithValue("@AddressID", addressId);
							deleteAddressCmd.ExecuteNonQuery();
						}
						MessageBox.Show("Пацієнта та його адресу видалено.");
						LoadPatientsData(); // Оновлення DataGridView
					}
					else
					{
						MessageBox.Show("Пацієнта з таким ID не знайдено.");
					}
				}
			}

			// Оновлення DataGridView
			LoadPatientsData();
		}
		private int GetPatientAddressId(int patientId, SqlConnection conn)
		{
			string query = "SELECT AddressID FROM Patients WHERE PatientID = @PatientID";
			using (SqlCommand cmd = new SqlCommand(query, conn))
			{
				cmd.Parameters.AddWithValue("@PatientID", patientId);
				object result = cmd.ExecuteScalar();
				return result != null ? Convert.ToInt32(result) : 0;
			}
		}
		// Метод для завантаження даних пацієнтів у DataGridView
		private void LoadPatientsData()
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				string query = "SELECT PatientID, PatientName, PatientLastname, PatientSurname, Gender, BirthDate, AddressID, Contact FROM Patients";
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					DataTable table = new DataTable();
					adapter.Fill(table);
					dataGridView2.DataSource = table;
				}
			}
		}
		private int InsertAddress(string street, string city)
		{
			int newAddressId = GetNextAddressId(); // Отримуємо наступний AddressID
			string query = "INSERT INTO Addresses (AddressID, Street, City) VALUES (@AddressID, @Street, @City)";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@AddressID", newAddressId);
					cmd.Parameters.AddWithValue("@Street", street);
					cmd.Parameters.AddWithValue("@City", city);

					conn.Open();
					cmd.ExecuteNonQuery();
					return newAddressId; // Повертаємо ID нової адреси
				}
			}
		}

		// Метод для оновлення існуючої адреси або вставки нової, якщо такої немає
		private int UpdateOrInsertAddress(string street, string city, int patientId)
		{
			// Спочатку спробуємо оновити існуючу адресу
			string updateQuery = @"
        UPDATE Addresses 
        SET Street = @Street, City = @City 
        FROM Addresses 
        INNER JOIN Patients ON Addresses.AddressID = Patients.AddressID 
        WHERE Patients.PatientID = @PatientID";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
				{
					cmd.Parameters.AddWithValue("@Street", street);
					cmd.Parameters.AddWithValue("@City", city);
					cmd.Parameters.AddWithValue("@PatientID", patientId);

					conn.Open();
					int rowsAffected = cmd.ExecuteNonQuery();

					// Якщо адресу оновлено, повертаємо існуючий AddressID
					if (rowsAffected > 0)
					{
						string selectQuery = "SELECT AddressID FROM Patients WHERE PatientID = @PatientID";
						using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
						{
							selectCmd.Parameters.AddWithValue("@PatientID", patientId);
							int addressId = (int)selectCmd.ExecuteScalar();
							return addressId;
						}
					}
					// Якщо оновлення не відбулося, можливо адреси ще немає, тому вставимо нову
					else
					{
						return InsertAddress(street, city);
					}
				}
			}
		}
		private void FindPatient()
		{
			int patientId = int.Parse(textBox4.Text); // textBoxPatientId - поле для введення ID пацієнта

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				string query = @"
            SELECT PatientID, PatientName, PatientLastname, PatientSurname, Gender, BirthDate, Contact, AddressID 
            FROM Patients 
            WHERE PatientID = @PatientID";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@PatientID", patientId);

					conn.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							// Заповнення полів форми даними пацієнта
							textBox4.Text = reader["PatientID"].ToString();
							textBox5.Text = reader["PatientName"].ToString();
							textBox6.Text = reader["PatientLastname"].ToString();
							textBox7.Text = reader["PatientSurname"].ToString();
							textBox8.Text = reader["Gender"].ToString();
							dateTimePicker1.Value = Convert.ToDateTime(reader["BirthDate"]);
							textBox10.Text = reader["Contact"].ToString();

							// Отримання адреси пацієнта
							int addressId = Convert.ToInt32(reader["AddressID"]);
							GetPatientAddress(addressId);
						}
						else
						{
							MessageBox.Show("Пацієнта з таким ID не знайдено.");
						}
					}
				}
			}
		}

		private void GetPatientAddress(int addressId)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				string query = "SELECT Street, City FROM Addresses WHERE AddressID = @AddressID";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@AddressID", addressId);

					conn.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							string street = reader["Street"].ToString();
							string city = reader["City"].ToString();
							textBox9.Text = street + ", " + city; // textBoxAddress - поле для введення адреси
						}
					}
				}
			}
		}
		private int GetNextAddressId()
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				string query = "SELECT MAX(AddressID) FROM Addresses";
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					conn.Open();
					object result = cmd.ExecuteScalar();
					// Перевіряємо, чи результат не є DBNull, що означатиме, що в таблиці ще немає записів.
					int lastAddressId = result != DBNull.Value ? Convert.ToInt32(result) : 0;
					return lastAddressId + 1; // Повертаємо наступний ID, який можна використовувати.
				}
			}
		}


		//лікарі

		private void CreateStaffMember()
		{
			// Перед використанням змінних з форми, переконайтеся, що вони існують і правильно ініціалізовані
			int staffId = int.Parse(textBox17.Text); // TextBox для вводу StaffID
			int userId = int.Parse(textBox18.Text); // TextBox для вводу UserID
			string staffName = textBox12.Text; // TextBox для вводу імені лікаря
			string staffLastname = textBox13.Text; // TextBox для прізвища
			string staffSurname = textBox14.Text; // TextBox для по батькові
			string position = textBox16.Text; // TextBox для посади
			DateTime birthDate = dateTimePicker2.Value; // DateTimePicker для дати народження

			// Розділяємо адресу на вулицю та місто
			string[] addressDetails = textBox15.Text.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
			string street = addressDetails.Length > 0 ? addressDetails[0] : string.Empty;
			string city = addressDetails.Length > 1 ? addressDetails[1] : string.Empty;

			int addressId = InsertAddress(street, city); // Метод вставки адреси повинен бути реалізований вище

			// Підготовка SQL-запиту для вставки даних
			string query = @"
        INSERT INTO MedicalStaff (StaffID, UserID, StaffName, StaffLastname, StaffSurname, Position, BirthDate, AddressID) 
        VALUES (@StaffID, @UserID, @StaffName, @StaffLastname, @StaffSurname, @Position, @BirthDate, @AddressID)";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					// Встановлення параметрів для SQL-команди
					cmd.Parameters.AddWithValue("@StaffID", staffId);
					cmd.Parameters.AddWithValue("@UserID", userId);
					cmd.Parameters.AddWithValue("@StaffName", staffName);
					cmd.Parameters.AddWithValue("@StaffLastname", staffLastname);
					cmd.Parameters.AddWithValue("@StaffSurname", staffSurname);
					cmd.Parameters.AddWithValue("@Position", position);
					cmd.Parameters.AddWithValue("@BirthDate", birthDate);
					cmd.Parameters.AddWithValue("@AddressID", addressId);

					// Відкриття з'єднання та виконання запиту
					conn.Open();
					int result = cmd.ExecuteNonQuery();

					// Перевірка результату виконання команди
					if (result > 0)
					{
						MessageBox.Show("Лікаря успішно створено.");
						// Тут можна оновити DataGridView, якщо потрібно
						 LoadStaffData(); // Метод для завантаження даних медперсоналу
					}
					else
					{
						MessageBox.Show("Не вдалося створити лікаря.");
					}
				}
			}
		}
		private void UpdateStaffMember()
		{
			// Отримання даних з текстових полів форми
			int staffId = int.Parse(textBox17.Text);
			string staffName = textBox12.Text;
			string staffLastname = textBox13.Text;
			string staffSurname = textBox14.Text;
			string position = textBox16.Text;
			DateTime birthDate = dateTimePicker2.Value; // Припустимо, що у вас є DateTimePicker для дати народження

			// Отримання адреси з текстового поля, яке може містити вулицю та місто, розділені комою
			var addressParts = textBox15.Text.Split(new[] { ", " }, StringSplitOptions.None);
			string street = addressParts.Length > 0 ? addressParts[0] : "";
			string city = addressParts.Length > 1 ? addressParts[1] : "";

			// Оновлення або вставка адреси
			int addressId = UpdateOrInsertAddress(street, city, staffId);

			// Підготовка SQL-запиту для оновлення даних лікаря
			string query = @"
        UPDATE MedicalStaff
        SET StaffName = @StaffName, StaffLastname = @StaffLastname, StaffSurname = @StaffSurname,
            Position = @Position, BirthDate = @BirthDate, AddressID = @AddressID
        WHERE StaffID = @StaffID";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					// Встановлення параметрів для SQL-команди
					cmd.Parameters.AddWithValue("@StaffID", staffId);
					cmd.Parameters.AddWithValue("@StaffName", staffName);
					cmd.Parameters.AddWithValue("@StaffLastname", staffLastname);
					cmd.Parameters.AddWithValue("@StaffSurname", staffSurname);
					cmd.Parameters.AddWithValue("@Position", position);
					cmd.Parameters.AddWithValue("@BirthDate", birthDate);
					cmd.Parameters.AddWithValue("@AddressID", addressId);

					// Відкриття з'єднання та виконання запиту
					conn.Open();
					int result = cmd.ExecuteNonQuery();

					// Перевірка результату оновлення
					if (result > 0)
					{
						MessageBox.Show("Дані співробітника оновлено.");
					}
					else
					{
						MessageBox.Show("Не вдалося оновити дані співробітника.");
					}
				}
			}

			// Можливо, вам знадобиться метод для оновлення DataGridView для відображення оновлених даних
			LoadStaffData();
		}
		private void DeleteStaffMember()
		{
			int staffId = int.Parse(textBox17.Text); // TextBox для вводу StaffID

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();

				// Отримання AddressID співробітника перед видаленням
				int addressId = GetStaffAddressId(staffId, conn);

				// Спочатку видалення запису співробітника
				string deleteStaffQuery = "DELETE FROM MedicalStaff WHERE StaffID = @StaffID";
				using (SqlCommand cmd = new SqlCommand(deleteStaffQuery, conn))
				{
					cmd.Parameters.AddWithValue("@StaffID", staffId);
					int result = cmd.ExecuteNonQuery();

					if (result > 0)
					{
						// Видалення адреси, якщо співробітника видалено
						string deleteAddressQuery = "DELETE FROM Addresses WHERE AddressID = @AddressID";
						using (SqlCommand deleteAddressCmd = new SqlCommand(deleteAddressQuery, conn))
						{
							deleteAddressCmd.Parameters.AddWithValue("@AddressID", addressId);
							deleteAddressCmd.ExecuteNonQuery();
						}

						MessageBox.Show("Співробітника та його адресу видалено.");
					}
					else
					{
						MessageBox.Show("Співробітника з таким ID не знайдено.");
					}
				}
			}

			// Оновлення DataGridView для відображення актуальних даних
			LoadStaffData();
		}

		private int GetStaffAddressId(int staffId, SqlConnection conn)
		{
			string query = "SELECT AddressID FROM MedicalStaff WHERE StaffID = @StaffID";
			using (SqlCommand cmd = new SqlCommand(query, conn))
			{
				cmd.Parameters.AddWithValue("@StaffID", staffId);
				object result = cmd.ExecuteScalar();
				return result != null ? Convert.ToInt32(result) : 0;
			}
		}

		// Припускаємо, що ви маєте метод LoadStaffData() для оновлення DataGridView
		private void LoadStaffData()
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				// Ваш SQL запит може включати приєднання до інших таблиць, якщо це потрібно
				string query = @"
            SELECT ms.StaffID, ms.StaffName, ms.StaffLastname, ms.StaffSurname, 
                   ms.Position, ms.BirthDate, a.Street + ', ' + a.City AS 'Address', u.Username
            FROM MedicalStaff ms
            LEFT JOIN Addresses a ON ms.AddressID = a.AddressID
            LEFT JOIN Users u ON ms.UserID = u.UserID";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					DataTable table = new DataTable();
					adapter.Fill(table);

					// Встановлення джерела даних для DataGridView
					dataGridView3.DataSource = table;
				}
			}

			// Налаштування вигляду DataGridView, якщо потрібно
			// Наприклад, ви можете встановити заголовки стовпців
			dataGridView3.Columns["StaffID"].HeaderText = "ID";
			dataGridView3.Columns["StaffName"].HeaderText = "Ім'я";
			dataGridView3.Columns["StaffLastname"].HeaderText = "Прізвище";
			dataGridView3.Columns["StaffSurname"].HeaderText = "По батькові";
			dataGridView3.Columns["Position"].HeaderText = "Посада";
			dataGridView3.Columns["BirthDate"].HeaderText = "Дата народження";
			dataGridView3.Columns["Address"].HeaderText = "Адреса";
			dataGridView3.Columns["Username"].HeaderText = "Користувач";
		}
		private void button8_Click(object sender, EventArgs e)
		{
			CreateStaffMember();
		}

		// Обробник події для кнопки "Оновити"
		private void button9_Click(object sender, EventArgs e)
		{
			UpdateStaffMember();
		}

		// Обробник події для кнопки "Видалити"
		private void button10_Click(object sender, EventArgs e)
		{
			DeleteStaffMember();
		}

		private void button11_Click(object sender, EventArgs e)
		{
			FindStaffMember();
		}

		private void FindStaffMember()
		{
			int staffId = int.Parse(textBox17.Text); // TextBox для вводу StaffID

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				string query = @"
        SELECT ms.StaffID, ms.StaffName, ms.StaffLastname, ms.StaffSurname, 
               ms.Position, ms.BirthDate, a.Street + ', ' + a.City AS 'Address', u.Username
        FROM MedicalStaff ms
        LEFT JOIN Addresses a ON ms.AddressID = a.AddressID
        LEFT JOIN Users u ON ms.UserID = u.UserID
        WHERE ms.StaffID = @StaffID";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@StaffID", staffId);

					conn.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							// Заповнення полів форми даними лікаря
							textBox17.Text = reader["StaffID"].ToString();
							textBox12.Text = reader["StaffName"].ToString();
							textBox13.Text = reader["StaffLastname"].ToString();
							textBox14.Text = reader["StaffSurname"].ToString();
							textBox16.Text = reader["Position"].ToString();
							dateTimePicker2.Value = Convert.ToDateTime(reader["BirthDate"]);
							textBox15.Text = reader["Address"].ToString();
							textBox18.Text = reader["Username"].ToString(); // Припускаємо, що у вас є TextBox для Username
						}
						else
						{
							MessageBox.Show("Співробітника з таким ID не знайдено.");
						}
					}
				}
			}
		}
		private void LoadCabinetsData()
		{
			string query = "SELECT PlaceID, Description FROM Places";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					DataTable table = new DataTable();
					adapter.Fill(table);
					dataGridView4.DataSource = table; // Налаштуйте назву вашого DataGridView відповідно
				}
			}
		}

		// Метод для створення нового кабінета
		private void CreateCabinet(int placeID, string cabinetName)
		{
			string query = "INSERT INTO Places (PlaceID, Description) VALUES (@PlaceID, @Description)";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@PlaceID", placeID);
					cmd.Parameters.AddWithValue("@Description", cabinetName);
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
			LoadCabinetsData(); // Оновлюємо DataGridView
		}

		// Метод для оновлення існуючого кабінета
		private void UpdateCabinet(int cabinetId, string cabinetName)
		{
			string query = "UPDATE Places SET Description = @Description WHERE PlaceID = @PlaceID";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@PlaceID", cabinetId);
					cmd.Parameters.AddWithValue("@Description", cabinetName);
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
			LoadCabinetsData(); // Оновлюємо DataGridView
		}

		// Метод для видалення кабінета
		private void DeleteCabinet(int cabinetId)
		{
			string query = "DELETE FROM Places WHERE PlaceID = @PlaceID";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@PlaceID", cabinetId);
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
			LoadCabinetsData(); // Оновлюємо DataGridView
		}

		private void button14_Click(object sender, EventArgs e)
		{
			
			if (int.TryParse(textBox19.Text, out int cabinetId))
			{
				UpdateCabinet(cabinetId, textBox20.Text);
			}
			else
			{
				MessageBox.Show("ID кабінета введено некоректно.");
			}
		}

		private void button12_Click(object sender, EventArgs e)
		{
			if (int.TryParse(textBox19.Text, out int cabinetId))
			{
				DeleteCabinet(cabinetId);
			}
			else
			{
				MessageBox.Show("ID кабінета введено некоректно. Будь ласка, введіть числове значення.");
			}
		}

		private void button13_Click(object sender, EventArgs e)
		{
			try
			{
				if (int.TryParse(textBox19.Text, out int placeID))
				{
					string cabinetName = textBox20.Text;
					CreateCabinet(placeID, cabinetName);
				}
				else
				{
					MessageBox.Show("Перевірте коректність введеного PlaceID.");
				}
			}
			catch (FormatException)
			{
				MessageBox.Show("Перевірте коректність введеного PlaceID.");
			}
			catch (SqlException ex)
			{
				MessageBox.Show($"Помилка бази даних: {ex.Message}");
			}
		}


		private void AddAppointment(string scheduleId, string patientName, string doctorName, DateTime appointmentDate, string roomDescription)
		{
			// Знайти ID пацієнта та лікаря за іменами
			int patientId = FindPatientIdByName(patientName);
			int doctorId = FindDoctorIdByName(doctorName);
			int roomId = FindRoomIdByDescription(roomDescription);

			// Створити запис в базі даних
			string query = "INSERT INTO Schedules (ScheduleID, PatientID, StaffID, Reception, PlaceID) VALUES (@ScheduleID, @PatientID, @StaffID, @Reception, @PlaceID)";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					// Встановлення параметрів для SQL-команди
					cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("@StaffID", doctorId);
					cmd.Parameters.AddWithValue("@Reception", appointmentDate);
					cmd.Parameters.AddWithValue("@PlaceID", roomId);

					// Відкриття з'єднання та виконання запиту
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}

		// Метод для оновлення запису на прийом
		private void UpdateAppointment(string scheduleId, string patientName, string doctorName, DateTime appointmentDate, string roomDescription)
		{
			// Знайти ID пацієнта, лікаря та кабінету
			int patientId = FindPatientIdByName(patientName);
			int doctorId = FindDoctorIdByName(doctorName);
			int roomId = FindRoomIdByDescription(roomDescription);

			// Підготувати SQL-запит для оновлення запису
			string query = @"
        UPDATE Schedules
        SET PatientID = @PatientID, StaffID = @StaffID, Reception = @Reception, PlaceID = @PlaceID
        WHERE ScheduleID = @ScheduleID";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					// Встановити параметри для SQL-команди
					cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("@StaffID", doctorId);
					cmd.Parameters.AddWithValue("@Reception", appointmentDate);
					cmd.Parameters.AddWithValue("@PlaceID", roomId);

					// Відкрити з'єднання і виконати команду
					conn.Open();
					int result = cmd.ExecuteNonQuery();

					// Перевірити результат
					if (result > 0)
					{
						MessageBox.Show("Запис успішно оновлено.");
					}
					else
					{
						MessageBox.Show("Не вдалося оновити запис.");
					}
				}
			}
		}

		// Метод для видалення запису на прийом
		private void DeleteAppointment(string scheduleId)
		{
			// Підготувати SQL-запит для видалення запису
			string query = "DELETE FROM Schedules WHERE ScheduleID = @ScheduleID";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					// Встановити параметр для SQL-команди
					cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);

					// Відкрити з'єднання і виконати команду
					conn.Open();
					int result = cmd.ExecuteNonQuery();

					// Перевірити результат
					if (result > 0)
					{
						MessageBox.Show("Запис на прийом успішно видалено.");
					}
					else
					{
						MessageBox.Show("Не вдалося видалити запис на прийом.");
					}
				}
			}
		}

		// Метод для пошуку ID пацієнта за іменем
		private int FindPatientIdByName(string patientName)
		{
			// Запит для отримання PatientID за іменем пацієнта
			string query = "SELECT PatientID FROM Patients WHERE CONCAT(PatientName, ' ', PatientLastname) = @PatientName";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@PatientName", patientName);

					conn.Open();
					object result = cmd.ExecuteScalar();

					// Перевіряємо, чи отримали ми результат
					if (result != null)
					{
						return Convert.ToInt32(result);
					}
					else
					{
						// Якщо пацієнта з таким іменем немає, повертаємо 0 або можемо кинути виключення залежно від логіки програми
						return 0;
					}
				}
			}
		}

		// Метод для пошуку ID лікаря за іменем
		private int FindDoctorIdByName(string doctorName)
		{
			// Запит для отримання StaffID за іменем лікаря
			string query = "SELECT StaffID FROM MedicalStaff WHERE CONCAT(StaffName, ' ', StaffLastname) = @DoctorName";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@DoctorName", doctorName);

					conn.Open();
					object result = cmd.ExecuteScalar();

					// Перевіряємо, чи отримали ми результат
					if (result != null)
					{
						return Convert.ToInt32(result);
					}
					else
					{
						// Якщо лікаря з таким іменем немає, повертаємо 0 або можемо кинути виключення залежно від логіки програми
						return 0;
					}
				}
			}
		}

		// Метод для пошуку ID кабінету за описом
		private int FindRoomIdByDescription(string roomDescription)
		{
			string query = "SELECT PlaceID FROM Places WHERE Description = @RoomDescription";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@RoomDescription", roomDescription);

					conn.Open();
					object result = cmd.ExecuteScalar();

					if (result != null)
					{
						return Convert.ToInt32(result);
					}
					else
					{
						throw new Exception("Кабінет з таким описом не знайдено.");
					}
				}
			}
		}
		private DateTime GetAppointmentDateTime(DateTimePicker datePicker, TextBox timeTextBox)
		{
			
			if (TimeSpan.TryParse(timeTextBox.Text, out var time))
			{
				return datePicker.Value.Date + time;
			}
			else
			{
				throw new FormatException("Неправильний формат часу.");
			}
		}
		private void button15_Click(object sender, EventArgs e)
		{
			try
			{
				string scheduleId = textBox21.Text;
				string patientName = textBox22.Text;
				string doctorName = textBox23.Text;
				DateTime appointmentDate = GetAppointmentDateTime(dateTimePicker3, textBox24);
				string roomDescription = textBox25.Text;

				AddAppointment(scheduleId, patientName, doctorName, appointmentDate, roomDescription);
			}
			catch (FormatException ex)
			{
				MessageBox.Show("Помилка форматування: " + ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Сталася помилка: " + ex.Message);
			}
			LoadScheduleData();
		}
		private void button17_Click(object sender, EventArgs e)
		{
			try
			{
				string scheduleId = textBox21.Text; // textBoxScheduleId - поле для вводу ID запису
				DeleteAppointment(scheduleId);
			}
			catch (FormatException ex)
			{
				MessageBox.Show("Помилка форматування: " + ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Сталася помилка: " + ex.Message);
			}
			LoadScheduleData();
		}
		private void button16_Click(object sender, EventArgs e)
		{
			try
			{
				string scheduleId = textBox21.Text; // Припустимо, що ви маєте TextBox для вводу ScheduleID
				string patientName = textBox22.Text; // TextBox для вводу імені пацієнта
				string doctorName = textBox23.Text; // TextBox для вводу імені лікаря
				DateTime appointmentDate = GetAppointmentDateTime(dateTimePicker3, textBox24); // Метод GetAppointmentDateTime об'єднує дату і час в один DateTime
				string roomDescription = textBox25.Text; // TextBox для вводу опису кабінету

				UpdateAppointment(scheduleId, patientName, doctorName, appointmentDate, roomDescription);
			}
			catch (FormatException ex)
			{
				MessageBox.Show("Помилка формату: " + ex.Message);
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Помилка бази даних: " + ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Сталася невідома помилка: " + ex.Message);
			}
			LoadScheduleData();
		}

		private void LoadScheduleData()
		{
			string query = @"
        SELECT s.ScheduleID, p.PatientName + ' ' + p.PatientLastname as Patient, 
               ms.StaffName + ' ' + ms.StaffLastname as Doctor, 
               s.Reception, pl.Description as Place
        FROM Schedules s
        JOIN Patients p ON s.PatientID = p.PatientID
        JOIN MedicalStaff ms ON s.StaffID = ms.StaffID
        JOIN Places pl ON s.PlaceID = pl.PlaceID";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					DataTable table = new DataTable();
					adapter.Fill(table);
					dataGridView5.DataSource = table; // Виберіть назву вашого DataGridView
				}
			}

			// Налаштування заголовків стовпців, якщо потрібно
			dataGridView5.Columns["ScheduleID"].HeaderText = "ID Запису";
			dataGridView5.Columns["Patient"].HeaderText = "Пацієнт";
			dataGridView5.Columns["Doctor"].HeaderText = "Лікар";
			dataGridView5.Columns["Reception"].HeaderText = "Дата і час";
			dataGridView5.Columns["Place"].HeaderText = "Кабінет";
		}


		private void AddMedicine(int medicineId, string name)
		{
			string query = "INSERT INTO Medicines (MedicineID, Name) VALUES (@MedicineID, @Name)";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@MedicineID", medicineId);
					cmd.Parameters.AddWithValue("@Name", name);
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
			LoadMedicinesData();
		}
		private void UpdateMedicine(int medicineId, string name)
		{
			string query = "UPDATE Medicines SET Name = @Name WHERE MedicineID = @MedicineID";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@MedicineID", medicineId);
					cmd.Parameters.AddWithValue("@Name", name);
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
			LoadMedicinesData();
		}
		private void DeleteMedicine(int medicineId)
		{
			string query = "DELETE FROM Medicines WHERE MedicineID = @MedicineID";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@MedicineID", medicineId);
					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
			LoadMedicinesData();
		}
		private void LoadMedicinesData()
		{
			string query = "SELECT MedicineID, Name FROM Medicines";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					DataTable table = new DataTable();
					adapter.Fill(table);
					dataGridView6.DataSource = table; // Замініть на назву вашого DataGridView
				}
			}
		}


		private void button18_Click(object sender, EventArgs e)
		{
			try
			{
				int medicineId = int.Parse(textBox26.Text); // Припустимо, що у вас є TextBox для ID
				string name = textBox27.Text; // Припустимо, що у вас є TextBox для назви лікарського засобу
				AddMedicine(medicineId, name);
			}
			catch (FormatException ex)
			{
				MessageBox.Show("Помилка форматування: " + ex.Message);
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Помилка бази даних: " + ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Сталася помилка: " + ex.Message);
			}
			LoadMedicinesData();
		}

		// Обробник події для кнопки оновлення інформації про лікарський засіб
		private void button19_Click(object sender, EventArgs e)
		{
			try
			{
				int medicineId = int.Parse(textBox26.Text); // Припустимо, що використовується той самий TextBox для ID
				string name = textBox27.Text; // Припустимо, що використовується той самий TextBox для назви
				UpdateMedicine(medicineId, name);
			}
			catch (FormatException ex)
			{
				MessageBox.Show("Помилка форматування: " + ex.Message);
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Помилка бази даних: " + ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Сталася помилка: " + ex.Message);
			}
			LoadMedicinesData();
		}

		// Обробник події для кнопки видалення лікарського засобу
		private void button20_Click(object sender, EventArgs e)
		{
			try
			{
				int medicineId = int.Parse(textBox26.Text); // Знову ж таки, використовується TextBox для ID
				DeleteMedicine(medicineId);
			}
			catch (FormatException ex)
			{
				MessageBox.Show("Помилка форматування: " + ex.Message);
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Помилка бази даних: " + ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Сталася помилка: " + ex.Message);
			}
			LoadMedicinesData();
		}
		
		private void button21_Click(object sender, EventArgs e)
		{
			try
			{
				int recordId = int.Parse(textBox28.Text);
				string patientFullName = textBox29.Text;
				string healthDetails = textBox30.Text;
				string treatmentHistory = textBox31.Text;

				int patientId = FindPatientIdByFullName(patientFullName); // Пошук PatientID за повним іменем

				if (patientId == 0)
				{
					MessageBox.Show("Пацієнта з таким іменем не знайдено.");
					return;
				}

				AddMedicalRecord(recordId, patientId, healthDetails, treatmentHistory);
				LoadMedicalRecordsData(); // Оновлення datagrid
			}
			catch (FormatException ex)
			{
				MessageBox.Show("Помилка форматування: " + ex.Message);
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Помилка бази даних: " + ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Сталася помилка: " + ex.Message);
			}
		}

		private void button22_Click(object sender, EventArgs e)
		{
			try
			{
				int recordId = int.Parse(textBox28.Text);
				string patientFullName = textBox29.Text;
				string healthDetails = textBox30.Text;
				string treatmentHistory = textBox31.Text;

				int patientId = FindPatientIdByFullName(patientFullName); // Пошук PatientID за повним іменем

				if (patientId == 0)
				{
					MessageBox.Show("Пацієнта з таким іменем не знайдено.");
					return;
				}

				UpdateMedicalRecord(recordId, patientId, healthDetails, treatmentHistory);
				LoadMedicalRecordsData(); // Оновлення datagrid
			}
			catch (FormatException ex)
			{
				MessageBox.Show("Помилка форматування: " + ex.Message);
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Помилка бази даних: " + ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Сталася помилка: " + ex.Message);
			}
		}

		private void button23_Click(object sender, EventArgs e)
		{
			try
			{
				int recordId = int.Parse(textBox28.Text);

				DeleteMedicalRecord(recordId);
				LoadMedicalRecordsData(); // Оновлення datagrid
			}
			catch (FormatException ex)
			{
				MessageBox.Show("Помилка форматування: " + ex.Message);
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Помилка бази даних: " + ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Сталася помилка: " + ex.Message);
			}
		}

		// Метод для пошуку PatientID за повним іменем
		private int FindPatientIdByFullName(string fullName)
		{
			string[] nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (nameParts.Length < 2)
			{
				return 0; // Повернути 0, якщо не вдалося розділити на ім'я та прізвище
			}

			string query = "SELECT PatientID FROM Patients WHERE PatientName = @Name AND PatientLastname = @Lastname";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@Name", nameParts[0]);
					cmd.Parameters.AddWithValue("@Lastname", nameParts[1]);

					conn.Open();
					object result = cmd.ExecuteScalar();

					return result != null ? Convert.ToInt32(result) : 0;
				}
			}
		}

		private void LoadMedicalRecordsData()
		{
			// SQL-запит для виведення даних
			string query = "SELECT * FROM MedicalRecords";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					DataTable table = new DataTable();
					adapter.Fill(table);
					dataGridView7.DataSource = table; // Підставте назву вашого DataGridView
				}
			}
		}
		// Метод для додавання нового медичного запису
		private void AddMedicalRecord(int recordId, int patientId, string healthDetails, string treatmentHistory)
		{
			string query = "INSERT INTO MedicalRecords (RecordID, PatientID, HealthDetails, TreatmentHistory) VALUES (@RecordID, @PatientID, @HealthDetails, @TreatmentHistory)";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@RecordID", recordId);
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("@HealthDetails", healthDetails);
					cmd.Parameters.AddWithValue("@TreatmentHistory", treatmentHistory);

					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}

		// Метод для оновлення існуючого медичного запису
		private void UpdateMedicalRecord(int recordId, int patientId, string healthDetails, string treatmentHistory)
		{
			string query = "UPDATE MedicalRecords SET PatientID = @PatientID, HealthDetails = @HealthDetails, TreatmentHistory = @TreatmentHistory WHERE RecordID = @RecordID";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@RecordID", recordId);
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("@HealthDetails", healthDetails);
					cmd.Parameters.AddWithValue("@TreatmentHistory", treatmentHistory);

					conn.Open();
					int result = cmd.ExecuteNonQuery();
					if (result == 0)
					{
						MessageBox.Show("Не вдалося оновити запис. Переконайтеся, що запис існує.");
					}
				}
			}
		}

		// Метод для видалення медичного запису
		private void DeleteMedicalRecord(int recordId)
		{
			string query = "DELETE FROM MedicalRecords WHERE RecordID = @RecordID";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@RecordID", recordId);

					conn.Open();
					int result = cmd.ExecuteNonQuery();
					if (result == 0)
					{
						MessageBox.Show("Не вдалося видалити запис. Переконайтеся, що запис існує.");
					}
				}
			}
		}


		private void AddVisitLog(int visitId, int patientId, int staffId, int medicineId, int recordId, int scheduleId)
		{
			string query = "INSERT INTO VisitLogs (VisitID, PatientID, StaffID, MedicineID, RecordID, ScheduleID) VALUES (@VisitID, @PatientID, @StaffID, @MedicineID, @RecordID, @ScheduleID)";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@VisitID", visitId);
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("@StaffID", staffId);
					cmd.Parameters.AddWithValue("@MedicineID", medicineId);
					cmd.Parameters.AddWithValue("@RecordID", recordId);
					cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);

					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}

		// Метод для оновлення існуючого запису візиту
		private void UpdateVisitLog(int visitId, int patientId, int staffId, int medicineId, int recordId, int scheduleId)
		{
			string query = "UPDATE VisitLogs SET PatientID = @PatientID, StaffID = @StaffID, MedicineID = @MedicineID, RecordID = @RecordID, ScheduleID = @ScheduleID WHERE VisitID = @VisitID";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@VisitID", visitId);
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("@StaffID", staffId);
					cmd.Parameters.AddWithValue("@MedicineID", medicineId);
					cmd.Parameters.AddWithValue("@RecordID", recordId);
					cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);

					conn.Open();
					int result = cmd.ExecuteNonQuery();
					if (result == 0)
					{
						MessageBox.Show("Не вдалося оновити запис. Переконайтеся, що запис існує.");
					}
				}
			}
		}

		// Метод для видалення запису візиту
		private void DeleteVisitLog(int visitId)
		{
			string query = "DELETE FROM VisitLogs WHERE VisitID = @VisitID";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@VisitID", visitId);

					conn.Open();
					int result = cmd.ExecuteNonQuery();
					if (result == 0)
					{
						MessageBox.Show("Не вдалося видалити запис. Переконайтеся, що запис існує.");
					}
				}
			}
			LoadVisitLogsData();
		}

		// Метод для завантаження даних візитів у DataGridView
		private void LoadVisitLogsData()
		{
			string query = "SELECT * FROM VisitLogs"; // Або конкретизуйте стовпці, які ви хочете відобразити
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					DataTable table = new DataTable();
					adapter.Fill(table);
					dataGridView8.DataSource = table; // Виберіть назву вашого DataGridView
				}
			}
		}
		private void button24_Click(object sender, EventArgs e)
		{
			try
			{
				int visitId = int.Parse(textBox32.Text);
				int patientId = int.Parse(textBox33.Text);
				int staffId = int.Parse(textBox34.Text);
				int medicineId = int.Parse(textBox35.Text);
				int recordId = int.Parse(textBox36.Text);
				int scheduleId = int.Parse(textBox37.Text);

				AddVisitLog(visitId, patientId, staffId, medicineId, recordId, scheduleId);
				MessageBox.Show("Запис додано успішно.");
				LoadVisitLogsData();
			}
			catch (FormatException)
			{
				MessageBox.Show("Неправильний формат введених даних.");
			}
			catch (SqlException ex)
			{
				MessageBox.Show($"Помилка бази даних: {ex.Message}");
			}
			
		}

		private void button25_Click(object sender, EventArgs e)
		{
			try
			{
				int visitId = int.Parse(textBox32.Text);
				int patientId = int.Parse(textBox33.Text);
				int staffId = int.Parse(textBox34.Text);
				int medicineId = int.Parse(textBox35.Text);
				int recordId = int.Parse(textBox36.Text);
				int scheduleId = int.Parse(textBox37.Text);

				UpdateVisitLog(visitId, patientId, staffId, medicineId, recordId, scheduleId);
				MessageBox.Show("Запис оновлено успішно.");
				LoadVisitLogsData();
			}
			catch (FormatException)
			{
				MessageBox.Show("Неправильний формат введених даних.");
			}
			catch (SqlException ex)
			{
				MessageBox.Show($"Помилка бази даних: {ex.Message}");
			}
		}

		private void button26_Click(object sender, EventArgs e)
		{
			try
			{
				int visitId = int.Parse(textBox32.Text);

				DeleteVisitLog(visitId);
				MessageBox.Show("Запис видалено успішно.");
				LoadVisitLogsData();
			}
			catch (FormatException)
			{
				MessageBox.Show("Неправильний формат введеного ID.");
			}
			catch (SqlException ex)
			{
				MessageBox.Show($"Помилка бази даних: {ex.Message}");
			}
		}

		private void button27_Click(object sender, EventArgs e)
		{
			try
			{
				int medicationId = int.Parse(textBox38.Text);
				int medicineId = int.Parse(textBox39.Text);
				int patientId = int.Parse(textBox40.Text);
				string dosage = textBox41.Text;
				string frequency = textBox42.Text;
				int visitId = int.Parse(textBox43.Text);

				AddMedication(medicationId, medicineId, patientId, dosage, frequency, visitId);
				MessageBox.Show("Ліки додано успішно.");
				LoadMedicationsData();
			}
			catch (FormatException)
			{
				MessageBox.Show("Неправильний формат введених даних.");
			}
			catch (SqlException ex)
			{
				MessageBox.Show($"Помилка бази даних: {ex.Message}");
			}
		}

		private void button28_Click(object sender, EventArgs e)
		{
			try
			{
				int medicationId = int.Parse(textBox38.Text);
				int medicineId = int.Parse(textBox39.Text);
				int patientId = int.Parse(textBox40.Text);
				string dosage = textBox41.Text;
				string frequency = textBox42.Text;
				int visitId = int.Parse(textBox43.Text);

				UpdateMedication(medicationId, medicineId, patientId, dosage, frequency, visitId);
				MessageBox.Show("Інформація про ліки оновлена успішно.");
				LoadMedicationsData();
			}
			catch (FormatException)
			{
				MessageBox.Show("Неправильний формат введених даних.");
			}
			catch (SqlException ex)
			{
				MessageBox.Show($"Помилка бази даних: {ex.Message}");
			}
		}

		private void button29_Click(object sender, EventArgs e)
		{
			try
			{
				int medicationId = int.Parse(textBox38.Text);

				DeleteMedication(medicationId);
				MessageBox.Show("Інформація про ліки видалена успішно.");
				LoadMedicationsData();
			}
			catch (FormatException)
			{
				MessageBox.Show("Неправильний формат введеного ID.");
			}
			catch (SqlException ex)
			{
				MessageBox.Show($"Помилка бази даних: {ex.Message}");
			}
		}

		private void AddMedication(int medicationId, int medicineId, int patientId, string dosage, string frequency, int visitId)
		{
			string query = "INSERT INTO Medications (MedicationID, MedicineID, PatientID, Dosage, Frequency, VisitID) VALUES (@MedicationID, @MedicineID, @PatientID, @Dosage, @Frequency, @VisitID)";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@MedicationID", medicationId);
					cmd.Parameters.AddWithValue("@MedicineID", medicineId);
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("@Dosage", dosage);
					cmd.Parameters.AddWithValue("@Frequency", frequency);
					cmd.Parameters.AddWithValue("@VisitID", visitId);

					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}
		private void UpdateMedication(int medicationId, int medicineId, int patientId, string dosage, string frequency, int visitId)
		{
			string query = "UPDATE Medications SET MedicineID = @MedicineID, PatientID = @PatientID, Dosage = @Dosage, Frequency = @Frequency, VisitID = @VisitID WHERE MedicationID = @MedicationID";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@MedicationID", medicationId);
					cmd.Parameters.AddWithValue("@MedicineID", medicineId);
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("@Dosage", dosage);
					cmd.Parameters.AddWithValue("@Frequency", frequency);
					cmd.Parameters.AddWithValue("@VisitID", visitId);

					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}

		private void DeleteMedication(int medicationId)
		{
			string query = "DELETE FROM Medications WHERE MedicationID = @MedicationID";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@MedicationID", medicationId);

					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}
		private void LoadMedicationsData()
		{
			string query = "SELECT MedicationID, MedicineID, PatientID, Dosage, Frequency, VisitID FROM Medications";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					DataTable table = new DataTable();
					adapter.Fill(table);

					// Припускаємо, що у вас є DataGridView з назвою medicationsDataGridView
					dataGridView9.DataSource = table;
				}
			}

			// Налаштування заголовків стовпців, якщо потрібно
			dataGridView9.Columns["MedicationID"].HeaderText = "ID Рецепту";
			dataGridView9.Columns["MedicineID"].HeaderText = "ID Медикаменту";
			dataGridView9.Columns["PatientID"].HeaderText = "ID Пацієнта";
			dataGridView9.Columns["Dosage"].HeaderText = "Дозування";
			dataGridView9.Columns["Frequency"].HeaderText = "Частота";
			dataGridView9.Columns["VisitID"].HeaderText = "ID Візиту";
		}

		private void AddHealthMonitoringRecord(int monitoringId, int patientId, int preassureId, int sugarLevelId, int pulseId)
		{
			string query = "INSERT INTO HealthMonitoring (MonitoringID, PatientID, PreassureID, SugarLevelID, PulseID) VALUES (@MonitoringID, @PatientID, @PreassureID, @SugarLevelID, @PulseID)";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@MonitoringID", monitoringId);
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("@PreassureID", preassureId);
					cmd.Parameters.AddWithValue("@SugarLevelID", sugarLevelId);
					cmd.Parameters.AddWithValue("@PulseID", pulseId);

					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}
		private void UpdateHealthMonitoringRecord(int monitoringId, int patientId, int preassureId, int sugarLevelId, int pulseId)
		{
			string query = "UPDATE HealthMonitoring SET PatientID = @PatientID, PreassureID = @PreassureID, SugarLevelID = @SugarLevelID, PulseID = @PulseID WHERE MonitoringID = @MonitoringID";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@MonitoringID", monitoringId);
					cmd.Parameters.AddWithValue("@PatientID", patientId);
					cmd.Parameters.AddWithValue("@PreassureID", preassureId);
					cmd.Parameters.AddWithValue("@SugarLevelID", sugarLevelId);
					cmd.Parameters.AddWithValue("@PulseID", pulseId);

					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}

		private void DeleteHealthMonitoringRecord(int monitoringId)
		{
			string query = "DELETE FROM HealthMonitoring WHERE MonitoringID = @MonitoringID";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@MonitoringID", monitoringId);

					conn.Open();
					cmd.ExecuteNonQuery();
				}
			}
		}

		private void button30_Click(object sender, EventArgs e)
		{
			// Зчитування даних з форми
			int monitoringId = Convert.ToInt32(textBox44.Text);
			int patientId = Convert.ToInt32(textBox45.Text);
			int preassureId = Convert.ToInt32(textBox46.Text);
			int sugarLevelId = Convert.ToInt32(textBox47.Text);
			int pulseId = Convert.ToInt32(textBox48.Text);

			AddHealthMonitoringRecord(monitoringId, patientId, preassureId, sugarLevelId, pulseId);
		}
		private void button32_Click(object sender, EventArgs e)
		{
			int monitoringId = Convert.ToInt32(textBox44.Text);
			DeleteHealthMonitoringRecord(monitoringId);
		}

		private void button31_Click(object sender, EventArgs e)
		{
			// Зчитування даних з форми
			int monitoringId = Convert.ToInt32(textBox44.Text);
			int patientId = Convert.ToInt32(textBox45.Text);
			int preassureId = Convert.ToInt32(textBox46.Text);
			int sugarLevelId = Convert.ToInt32(textBox47.Text);
			int pulseId = Convert.ToInt32(textBox48.Text);

			// Виклик методу оновлення запису з введеними даними
			UpdateHealthMonitoringRecord(monitoringId, patientId, preassureId, sugarLevelId, pulseId);
		}
		private void LoadHealthMonitoringData()
		{
			string query = "SELECT MonitoringID, PatientID, PreassureID, SugarLevelID, PulseID FROM HealthMonitoring";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					DataTable table = new DataTable();
					adapter.Fill(table);
					dataGridView10.DataSource = table;
				}
			}

			// Налаштування заголовків стовпців, якщо потрібно
			dataGridView10.Columns["MonitoringID"].HeaderText = "ID Моніторингу";
			dataGridView10.Columns["PatientID"].HeaderText = "ID Пацієнта";
			dataGridView10.Columns["PreassureID"].HeaderText = "ID Тиску";
			dataGridView10.Columns["SugarLevelID"].HeaderText = "ID Рівня Цукру";
			dataGridView10.Columns["PulseID"].HeaderText = "ID Пульсу";
		}
	}
}
    
