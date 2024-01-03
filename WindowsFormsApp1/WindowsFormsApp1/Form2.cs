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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
	public partial class Form2 : Form
	{

		private string connectionString = @"Data Source=DESKTOP-C85P3IS\SQLTRPZ; Initial Catalog=MedicalCenter; Integrated Security=True;";
		public string CurrentUser { get; set; } // Змінна для зберігання поточного користувача

		public Form2()
		{
			InitializeComponent();
			dataGridView4.ScrollBars = ScrollBars.Both; // Включити горизонтальну прокрутку
			dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; // Автоматично регулювати ширину всіх стовпців
		}

		private void Form2_Load_1(object sender, EventArgs e)
		{

			string query = @"
       SELECT p.PatientName, p.PatientLastname, p.PatientSurname, p.Gender, p.BirthDate, p.Contact, a.Street, a.City
       FROM Patients p
       INNER JOIN Addresses a ON p.AddressID = a.AddressID
       INNER JOIN Users u ON p.UserID = u.UserID
       WHERE u.Username = @Username;";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);

				try
				{
					conn.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							// Припускаємо, що у вас є текстові бокси з такими іменами на формі
							textBox2.Text = reader["PatientName"].ToString();
							textBox1.Text = reader["PatientLastname"].ToString();
							textBox3.Text = reader["PatientSurname"].ToString();
							textBox8.Text = reader["Gender"].ToString(); // Припускаємо, що Gender містить значення, яке вже є в comboBox
							dateTimePicker1.Value = Convert.ToDateTime(reader["BirthDate"]);
							textBox5.Text = reader["Contact"].ToString();
							textBox4.Text = $"{reader["Street"].ToString()}, {reader["City"].ToString()}"; // Припускаємо, що адреса знаходиться в одному полі
						}
						else
						{
							MessageBox.Show("Дані про пацієнта не знайдено.");
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при отриманні даних про пацієнта: " + ex.Message);
				}
				LoadVisitsData();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string query = @"
        UPDATE Patients
        SET PatientName = @PatientName,
            PatientLastname = @PatientLastname,
            PatientSurname = @PatientSurname,
            Gender = @Gender,
            BirthDate = @BirthDate,
            Contact = @Contact
        WHERE UserID = (SELECT UserID FROM Users WHERE Username = @Username);

        UPDATE Addresses
        SET Street = @Street,
            City = @City
        FROM Patients p
        INNER JOIN Addresses a ON p.AddressID = a.AddressID
        WHERE p.UserID = (SELECT UserID FROM Users WHERE Username = @Username);
    ";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);

				// Параметри для Patients
				cmd.Parameters.AddWithValue("@PatientName", textBox1.Text);
				cmd.Parameters.AddWithValue("@PatientLastname", textBox2.Text);
				cmd.Parameters.AddWithValue("@PatientSurname", textBox3.Text);
				cmd.Parameters.AddWithValue("@Gender", textBox8.Text); // Ви повинні змінити це на відповідний контроль, можливо comboBox
				cmd.Parameters.AddWithValue("@BirthDate", dateTimePicker1.Value);
				cmd.Parameters.AddWithValue("@Contact", textBox5.Text);
				// Параметри для Addresses
				cmd.Parameters.AddWithValue("@Street", textBox4.Text.Split(',')[0].Trim()); // Припускаємо, що ви використовуєте кому для розділення вулиці та міста
				cmd.Parameters.AddWithValue("@City", textBox4.Text.Split(',')[1].Trim());
				cmd.Parameters.AddWithValue("@Username", CurrentUser);

				try
				{
					conn.Open();
					int result = cmd.ExecuteNonQuery();

					if (result > 0)
					{
						MessageBox.Show("Дані пацієнта оновлено.");
					}
					else
					{
						MessageBox.Show("Дані про пацієнта не знайдено для оновлення.");
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при оновленні даних про пацієнта: " + ex.Message);
				}
			}
		}
		private void LoadVisitsData()
		{
			string query = @"
    SELECT 
    sch.Reception AS [Date], 
    ms.StaffName + ' ' + ms.StaffLastname AS [Doctor], 
    MAX(mr.HealthDetails) AS [Complaints], 
    MAX(mr.TreatmentHistory) AS [VisitDetails]
FROM Schedules sch
INNER JOIN VisitLogs v ON sch.ScheduleID = v.ScheduleID
INNER JOIN MedicalRecords mr ON v.PatientID = mr.PatientID
INNER JOIN MedicalStaff ms ON sch.StaffID = ms.StaffID
INNER JOIN Patients p ON v.PatientID = p.PatientID
INNER JOIN Users u ON p.UserID = u.UserID
WHERE u.Username = 'patient'
GROUP BY sch.Reception, ms.StaffName, ms.StaffLastname, v.VisitID
ORDER BY sch.Reception DESC;";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);

				try
				{
					conn.Open();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						DataTable visitsTable = new DataTable();
						adapter.Fill(visitsTable);
						dataGridView4.DataSource = visitsTable;

						// Змінити назви колонок
						dataGridView4.Columns["Date"].HeaderText = "Дата візиту";
						dataGridView4.Columns["Doctor"].HeaderText = "Лікар";
						dataGridView4.Columns["Complaints"].HeaderText = "Скарги";
						dataGridView4.Columns["VisitDetails"].HeaderText = "Деталі візиту";
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при завантаженні історії візитів: " + ex.Message);
				}
			}
			LoadAppointmentData();
		}
		private void LoadAppointmentData()
		{
			// Запит для минулих візитів
			string pastAppointmentsQuery = @"
		SELECT 
    sch.Reception AS [Date], 
    ms.StaffName + ' ' + ms.StaffLastname AS [Doctor], 
    pl.Description AS [Place],
    m.Name AS [Medication],
    med.Dosage AS [Dosage],
    med.Frequency AS [Fre]
FROM Schedules sch
INNER JOIN VisitLogs v ON sch.ScheduleID = v.ScheduleID
INNER JOIN MedicalRecords mr ON v.PatientID = mr.PatientID
INNER JOIN MedicalStaff ms ON sch.StaffID = ms.StaffID
INNER JOIN Patients p ON v.PatientID = p.PatientID
INNER JOIN Users u ON p.UserID = u.UserID
LEFT JOIN Medications med ON v.VisitID = med.VisitID
LEFT JOIN Medicines m ON med.MedicineID = m.MedicineID
LEFT JOIN Places pl ON sch.PlaceID = pl.PlaceID
WHERE u.Username = @Username
GROUP BY sch.Reception, ms.StaffName, ms.StaffLastname, pl.Description, m.Name, med.Dosage, med.Frequency
ORDER BY sch.Reception DESC";

			// Запит для майбутніх візитів
			string futureAppointmentsQuery = @"

	SELECT 
        sch.Reception AS 'Date', 
        ms.StaffName + ' ' + ms.StaffLastname AS 'Doctor', 
        pl.Description AS 'Place'
    FROM Schedules sch 
    INNER JOIN MedicalStaff ms ON sch.StaffID = ms.StaffID
    INNER JOIN Patients p ON sch.PatientID = p.PatientID
    INNER JOIN Users u ON p.UserID = u.UserID
    LEFT JOIN Places pl ON sch.PlaceID = pl.PlaceID
    WHERE u.Username = @Username AND sch.Reception > GETDATE()
    ORDER BY sch.Reception;";

			string DietRecQuery = @"
SELECT TOP 1 
DietDetails
FROM Dietology 
INNER JOIN Patients ON Dietology.PatientID=Patients.PatientID
INNER JOIN Users ON Patients.UserID=Users.UserID
WHERE Users.Username=@Username
ORDER BY DietDetails DESC;";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(pastAppointmentsQuery, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);

				try
				{
					conn.Open();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						DataTable visitsTable = new DataTable();
						adapter.Fill(visitsTable);
						dataGridView1.DataSource = visitsTable;

						// Змінити назви колонок
						dataGridView1.Columns["Date"].HeaderText = "Дата візиту";
						dataGridView1.Columns["Doctor"].HeaderText = "Лікар";
						dataGridView1.Columns["Place"].HeaderText = "Кабінет";
						dataGridView1.Columns["Medication"].HeaderText = "Призначені ліки";
						dataGridView1.Columns["Dosage"].HeaderText = "Дозування";
						dataGridView1.Columns["Fre"].HeaderText = "Частота";
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при завантаженні історії візитів: " + ex.Message);
				}
			}
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(futureAppointmentsQuery, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);

				try
				{
					conn.Open();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						DataTable visitsTable = new DataTable();
						adapter.Fill(visitsTable);
						dataGridView2.DataSource = visitsTable;

						// Змінити назви колонок
						dataGridView2.Columns["Date"].HeaderText = "Дата візиту";
						dataGridView2.Columns["Doctor"].HeaderText = "Лікар";
						dataGridView2.Columns["Place"].HeaderText = "Кабінет";
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при завантаженні історії візитів: " + ex.Message);
				}
			}

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(DietRecQuery, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);

				try
				{
					conn.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							// Припускаємо, що у вас є текстові бокси з такими іменами на формі
							textBox6.Text = reader["DietDetails"].ToString();

						}
						else
						{
							MessageBox.Show("Дані про пацієнта не знайдено.");
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при отриманні даних про пацієнта: " + ex.Message);
				}
			}

			SetupPulseChart();
			SetupPreassureChart();
		}
		private void SetupSugarChart()
		{
			// Налаштування лінійного графіка
			chart1.Series["Series1"].ChartType = SeriesChartType.Line;

			string query = @"
SELECT SugarLevel, ReadingDate AS Date
FROM SugarLevel sl
INNER JOIN HealthMonitoring hm ON sl.SugarLevelID = hm.SugarLevelID
INNER JOIN Patients p ON hm.PatientID = p.PatientID
INNER JOIN Users u ON p.UserID = u.UserID
WHERE u.Username = @Username;";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);

				try
				{
					conn.Open();
					SqlDataReader reader = cmd.ExecuteReader();

					Series series = chart1.Series["Series1"];
					series.Points.Clear();
					// Отримання доступу до серії
					series.ChartType = SeriesChartType.Line;

					// Налаштування товщини лінії
					series.BorderWidth = 3;

					// Налаштування стилю маркерів
					series.MarkerStyle = MarkerStyle.Circle;

					// Налаштування розміру маркерів
					series.MarkerSize = 7;

					// Налаштування кольору маркерів
					series.MarkerColor = Color.Red;

					while (reader.Read())
					{
						double sugarlevel = reader.GetDouble(reader.GetOrdinal("SugarLevel"));
						DateTime date = reader.GetDateTime(reader.GetOrdinal("Date"));

						// Додавання точки на графік
						series.Points.AddXY(date, sugarlevel);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при отриманні даних: " + ex.Message);
				}
			}
		}
		private void SetupPulseChart()
		{
			// Налаштування лінійного графіка
			chart2.Series["Series1"].ChartType = SeriesChartType.Line;

			string query = @"
SELECT PulseRate AS Pulse, ReadingDate AS Date
FROM Pulse pul
INNER JOIN HealthMonitoring hm ON pul.PulseID = hm.PulseID
INNER JOIN Patients p ON hm.PatientID = p.PatientID
INNER JOIN Users u ON p.UserID = u.UserID
WHERE u.Username = @Username;";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);

				try
				{
					conn.Open();
					SqlDataReader reader = cmd.ExecuteReader();

					Series series = chart2.Series["Series1"];
					series.Points.Clear();
					// Отримання доступу до серії
					series.ChartType = SeriesChartType.Line;

					// Налаштування товщини лінії
					series.BorderWidth = 3;

					// Налаштування стилю маркерів
					series.MarkerStyle = MarkerStyle.Circle;

					// Налаштування розміру маркерів
					series.MarkerSize = 7;

					// Налаштування кольору маркерів
					series.MarkerColor = Color.Red;

					while (reader.Read())
					{
						DateTime date = reader.GetDateTime(reader.GetOrdinal("Date"));
						int pulse = reader.GetInt32(reader.GetOrdinal("Pulse"));

						// Додавання точки на графік
						series.Points.AddXY(date, pulse);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при отриманні даних: " + ex.Message);
				}
				SetupSugarChart();
			}
		}
		private void SetupPreassureChart()
		{
			// Налаштування лінійного графіка
			chart3.Series["Series1"].ChartType = SeriesChartType.Line;

			string query = @"
SELECT Preassure, ReadingDate AS Date
FROM Preassure pre
INNER JOIN HealthMonitoring hm ON pre.PreassureID = hm.PreassureID
INNER JOIN Patients p ON hm.PatientID = p.PatientID
INNER JOIN Users u ON p.UserID = u.UserID
WHERE u.Username = @Username;";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);

				try
				{
					conn.Open();
					SqlDataReader reader = cmd.ExecuteReader();

					Series series = chart3.Series["Series1"];
					series.Points.Clear();
					// Отримання доступу до серії
					series.ChartType = SeriesChartType.Line;

					// Налаштування товщини лінії
					series.BorderWidth = 3;

					// Налаштування стилю маркерів
					series.MarkerStyle = MarkerStyle.Circle;

					// Налаштування розміру маркерів
					series.MarkerSize = 7;

					// Налаштування кольору маркерів
					series.MarkerColor = Color.Red;

					while (reader.Read())
					{
						DateTime date = reader.GetDateTime(reader.GetOrdinal("Date"));
						double preassure = reader.GetDouble(reader.GetOrdinal("Preassure"));

						// Додавання точки на графік
						series.Points.AddXY(date, preassure);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при отриманні даних: " + ex.Message);
				}
				RefreshPainLogs();
			}
		}
		private int GetNextPainLogID()
		{
			int nextID = 0;
			string query = "SELECT MAX(PainLogID) FROM PainLogs;";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				try
				{
					conn.Open();
					object result = cmd.ExecuteScalar();
					nextID = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при отриманні наступного PainLogID: " + ex.Message);
				}
			}
			return nextID;
		}
		private void AddPainLogEntry(int painLevel)
		{
			int patientID = GetPatientIDFromUsername(CurrentUser);
			int nextPainLogID = GetNextPainLogID(); // Отримуємо наступний ID для PainLog

			if (patientID == -1)
			{
				MessageBox.Show("PatientID не знайдено.");
				return;
			}

			string insertQuery = @"
INSERT INTO PainLogs (PainLogID, PatientID, PainLevel, LogDate)
VALUES (@PainLogID, @PatientID, @PainLevel, @LogDate);"; // Додаємо PainLogID до запиту

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(insertQuery, conn);
				cmd.Parameters.AddWithValue("@PainLogID", nextPainLogID); // Додаємо PainLogID як параметр
				cmd.Parameters.AddWithValue("@PatientID", patientID);
				cmd.Parameters.AddWithValue("@PainLevel", painLevel);
				cmd.Parameters.AddWithValue("@LogDate", DateTime.Now); // Використовуємо поточний час

				try
				{
					conn.Open();
					cmd.ExecuteNonQuery();
					MessageBox.Show("Запис успішно додано.");
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при додаванні запису: " + ex.Message);
				}
			}
		}
		private int GetPatientIDFromUsername(string username)
		{
			int patientID = -1;
			string query = "SELECT PatientID FROM Patients INNER JOIN Users ON Patients.UserID = Users.UserID WHERE Username = @Username;";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@Username", username);
				try
				{
					conn.Open();
					object result = cmd.ExecuteScalar();
					if (result != null && result != DBNull.Value)
					{
						patientID = Convert.ToInt32(result);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при отриманні PatientID: " + ex.Message);
				}
			}
			return patientID;
		}
		private void RefreshPainLogs()
		{
			string query = "SELECT PainLevel, LogDate FROM PainLogs WHERE PatientID = @PatientID ORDER BY LogDate DESC;";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@PatientID", GetPatientIDFromUsername(CurrentUser));

				try
				{
					conn.Open();
					DataTable dt = new DataTable();
					using (SqlDataAdapter da = new SqlDataAdapter(cmd))
					{
						da.Fill(dt);
						dataGridView3.DataSource = dt;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при завантаженні журналу болю: " + ex.Message);
				}
			}
		}
		private void button3_Click_1(object sender, EventArgs e)
		{
			if (int.TryParse(textBox7.Text, out int painLevel))
			{
				AddPainLogEntry(painLevel);
			}
			else
			{
				MessageBox.Show("Будь ласка, введіть коректне числове значення для рівня болю.");
			}
			RefreshPainLogs();
		}

	}
}