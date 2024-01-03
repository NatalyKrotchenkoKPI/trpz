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
using System.Xml.Linq;

namespace WindowsFormsApp1
{
	public partial class Form3 : Form
	{
		private string connectionString = @"Data Source=DESKTOP-C85P3IS\SQLTRPZ; Initial Catalog=MedicalCenter; Integrated Security=True;";
		public string CurrentUser { get; set; }

		public Form3()
		{
			InitializeComponent();


		}

		private void Form3_Load(object sender, EventArgs e)
		{
			string query = @"
            SELECT s.StaffName, s.StaffSurname, s.StaffLastname, s.Position, s.BirthDate, a.Street, a.City
            FROM MedicalStaff s
            INNER JOIN Addresses a ON s.AddressID = a.AddressID
            INNER JOIN Users u ON s.UserID = u.UserID
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
							DoctorSurname.Text = reader["StaffSurname"].ToString();
							DoctorName.Text = reader["StaffName"].ToString();
							DoctorLastname.Text = reader["StaffLastname"].ToString();
							DoctorPosition.Text = reader["Position"].ToString();
							dateTimePicker2.Value = Convert.ToDateTime(reader["BirthDate"]);
							DoctorAdress.Text = $"{reader["Street"].ToString()}, {reader["City"].ToString()}";
						}
						else
						{
							MessageBox.Show("Дані про лікаря не знайдено.");
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при отриманні даних про лікаря: " + ex.Message);
				}
			}
			LoadMyPatientsData();
		}

		private void SaveButton_Click_1(object sender, EventArgs e)
		{

			string query = @"
            UPDATE MedicalStaff
            SET StaffName = @StaffName,
                StaffSurname = @StaffSurname,
                StaffLastname = @StaffLastname,
                Position = @Position,
                BirthDate = @BirthDate
            WHERE UserID = (SELECT UserID FROM Users WHERE Username = @Username);

            UPDATE Addresses
            SET Street = @Street,
                City = @City
            FROM MedicalStaff s
            INNER JOIN Addresses a ON s.AddressID = a.AddressID
            WHERE s.UserID = (SELECT UserID FROM Users WHERE Username = @Username);
        ";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);

				// Параметри для Staff
				cmd.Parameters.AddWithValue("@StaffName", DoctorName.Text);
				cmd.Parameters.AddWithValue("@StaffSurname", DoctorSurname.Text);
				cmd.Parameters.AddWithValue("@StaffLastname", DoctorLastname.Text);
				cmd.Parameters.AddWithValue("@Position", DoctorPosition.Text);
				cmd.Parameters.AddWithValue("@BirthDate", dateTimePicker2.Value);
				// Параметри для Addresses
				string[] addressParts = DoctorAdress.Text.Split(',');
				cmd.Parameters.AddWithValue("@Street", addressParts[0].Trim());
				cmd.Parameters.AddWithValue("@City", addressParts[1].Trim());
				cmd.Parameters.AddWithValue("@Username", CurrentUser);

				try
				{
					conn.Open();
					int result = cmd.ExecuteNonQuery();

					if (result > 0)
					{
						MessageBox.Show("Дані лікаря оновлено.");
					}
					else
					{
						MessageBox.Show("Дані про лікаря не знайдено для оновлення.");
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при оновленні даних про лікаря: " + ex.Message);
				}
			}
		}
		private void LoadMyPatientsData()
		{
			// Запит для отримання інформації про пацієнтів
			string myPatientsQuery = @"
SELECT  
    p.PatientName + ' ' + p.PatientLastname AS 'FullName', 
    p.Gender AS 'Gender', 
    sch.Reception AS 'Visit Date', 
    mr.HealthDetails AS 'Complaints'
FROM Patients p
INNER JOIN Schedules sch ON p.PatientID = sch.PatientID
INNER JOIN MedicalRecords mr ON p.PatientID = mr.PatientID
INNER JOIN MedicalStaff ms ON sch.StaffID = ms.StaffID
INNER JOIN Users u ON ms.UserID = u.UserID
WHERE u.Username = @Username
GROUP BY p.PatientName, p.PatientLastname, p.Gender, sch.Reception, mr.HealthDetails
ORDER BY sch.Reception DESC";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(myPatientsQuery, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);

				try
				{
					conn.Open();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						DataTable myPatientsTable = new DataTable();
						adapter.Fill(myPatientsTable);
						dataGridView1.DataSource = myPatientsTable;

						// Adjust column headers as necessary
						dataGridView1.Columns["FullName"].HeaderText = "Повне ім'я";
						dataGridView1.Columns["Gender"].HeaderText = "Стать";
						dataGridView1.Columns["Visit Date"].HeaderText = "Дата візиту";
						dataGridView1.Columns["Complaints"].HeaderText = "Скарги";
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при завантаженні даних моїх пацієнтів: " + ex.Message);
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			// Отримати ім'я та прізвище з текстового поля
			string fullName = textBox1.Text;
			string[] nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			// Перевірка чи введені обидва ім'я та прізвище
			if (nameParts.Length < 2)
			{
				MessageBox.Show("Будь ласка, введіть ім'я та прізвище пацієнта.");
				return;
			}

			string firstName = nameParts[0];
			string lastName = nameParts[1];

			// Формування SQL запиту з умовою WHERE, яка використовує ім'я та прізвище
			string query = @"
SELECT  
    p.PatientName + ' ' + p.PatientLastname AS 'FullName', 
    p.Gender AS 'Gender', 
    s.Reception AS 'Visit Date', 
    mr.HealthDetails AS 'Complaints'
FROM Patients p
INNER JOIN Schedules s ON p.PatientID = s.PatientID
INNER JOIN VisitLogs v ON s.ScheduleID = v.ScheduleID
INNER JOIN MedicalRecords mr ON p.PatientID = mr.PatientID
INNER JOIN MedicalStaff ms ON s.StaffID = ms.StaffID
INNER JOIN Users u ON ms.UserID = u.UserID
WHERE u.Username = @Username AND p.PatientName LIKE @FirstName + '%' AND p.PatientLastname LIKE @LastName + '%'
GROUP BY p.PatientName, p.PatientLastname, p.Gender, s.Reception, mr.HealthDetails
ORDER BY s.Reception DESC;";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);
				cmd.Parameters.AddWithValue("@FirstName", firstName);
				cmd.Parameters.AddWithValue("@LastName", lastName);

				DataTable myPatientsTable = new DataTable();
				try
				{
					conn.Open();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(myPatientsTable);
					}
					dataGridView1.DataSource = myPatientsTable;

					// Adjust column headers as necessary
					dataGridView1.Columns["FullName"].HeaderText = "Повне ім'я";
					dataGridView1.Columns["Gender"].HeaderText = "Стать";
					dataGridView1.Columns["Visit Date"].HeaderText = "Дата візиту";
					dataGridView1.Columns["Complaints"].HeaderText = "Скарги";
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при пошуку пацієнта: " + ex.Message);
				}
			}
		}
		private void dateTimePicker_ValueChanged(object sender, EventArgs e)
		{
			// Ваш код для оновлення dataGridView
			LoadAppointmentsByDate(dateTimePicker1.Value);
		}

		private void LoadAppointmentsByDate(DateTime selectedDate)
		{
			string query = @"
SELECT 
    p.PatientName + ' ' + p.PatientLastname AS [Patient Name], 
    p.Gender AS [Gender], 
    sch.Reception AS [Appointment Time], 
    pl.Description AS [Place]
FROM Schedules sch
INNER JOIN Patients p ON sch.PatientID = p.PatientID
INNER JOIN Places pl ON sch.PlaceID = pl.PlaceID
INNER JOIN MedicalStaff ms ON sch.StaffID = ms.StaffID
INNER JOIN Users u ON ms.UserID = u.UserID
WHERE u.Username = @Username AND CAST(sch.Reception AS DATE) = CAST(@SelectedDate AS DATE)
ORDER BY sch.Reception;";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);
				cmd.Parameters.AddWithValue("@SelectedDate", selectedDate);

				try
				{
					conn.Open();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						DataTable appointmentsTable = new DataTable();
						adapter.Fill(appointmentsTable);
						dataGridView2.DataSource = appointmentsTable;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при завантаженні розкладу: " + ex.Message);
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			LoadAppointmentsByDate(dateTimePicker1.Value);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			DateTime startDate = dateTimePicker3.Value.Date;
			DateTime endDate = dateTimePicker4.Value.Date;

			if (startDate > endDate)
			{
				MessageBox.Show("Дата початку повинна бути меншою або рівною даті закінчення.");
				return;
			}

			LoadServiceJournal(startDate, endDate);
		}
		private void LoadServiceJournal(DateTime startDate, DateTime endDate)
		{
			string query = @"
SELECT 
    p.PatientName + ' ' + p.PatientLastname AS [Patient Name], 
    p.Gender, 
    sch.Reception AS [Appointment Time], 
    pl.Description AS [Place],
    Medications = STUFF(
        (SELECT DISTINCT ', ' + m.Name
         FROM Medications med
         INNER JOIN Medicines m ON med.MedicineID = m.MedicineID
         WHERE med.VisitID = v.VisitID
         FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)')
    , 1, 2, ''),
    Dosages = STUFF(
        (SELECT DISTINCT ', ' + med.Dosage
         FROM Medications med
         WHERE med.VisitID = v.VisitID
         FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)')
    , 1, 2, '')
FROM Schedules sch
INNER JOIN Patients p ON sch.PatientID = p.PatientID
INNER JOIN Places pl ON sch.PlaceID = pl.PlaceID
INNER JOIN MedicalStaff ms ON sch.StaffID = ms.StaffID
INNER JOIN Users u ON ms.UserID = u.UserID
INNER JOIN VisitLogs v ON sch.ScheduleID = v.ScheduleID
WHERE u.Username = @Username
AND CAST(sch.Reception AS DATE) BETWEEN @StartDate AND @EndDate
GROUP BY p.PatientName, p.PatientLastname, p.Gender, sch.Reception, pl.Description, v.VisitID
ORDER BY sch.Reception";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@Username", CurrentUser);
				cmd.Parameters.AddWithValue("@StartDate", startDate);
				cmd.Parameters.AddWithValue("@EndDate", endDate);

				try
				{
					conn.Open();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						DataTable journalTable = new DataTable();
						adapter.Fill(journalTable);
						dataGridView3.DataSource = journalTable; // dataGridView3 - це ваш DataGridView на формі
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при завантаженні журналу обслуговування: " + ex.Message);
				}
			}
		}

		private void label20_Click(object sender, EventArgs e)
		{

		}

		private void button5_Click(object sender, EventArgs e)
		{
			UpdatePatientInfo();

		}

		private void UpdatePatientInfo()
		{
			// Parse the date from the DatePicker and time from the TextBox
			DateTime selectedDate = dateTimePicker6.Value.Date;
			TimeSpan timeSpan;
			if (TimeSpan.TryParse(textBox9.Text, out timeSpan))
			{
				DateTime combinedDateTime = selectedDate.Add(timeSpan);

				// SQL query to get the patient's name and last name based on the selected date and time
				string query = @"
            SELECT p.PatientName + ' ' + p.PatientLastname AS FullName
            FROM Schedules sch
            INNER JOIN Patients p ON sch.PatientID = p.PatientID
            WHERE sch.Reception = @SelectedDateTime;";

				using (SqlConnection conn = new SqlConnection(connectionString))
				{
					SqlCommand cmd = new SqlCommand(query, conn);
					cmd.Parameters.AddWithValue("@SelectedDateTime", combinedDateTime);

					try
					{
						conn.Open();
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								textBox2.Text = reader["FullName"].ToString();
							}
							else
							{
								textBox2.Text = "";
								textBox3.Text = "";
								MessageBox.Show("No appointments found for the selected time.");
							}
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show("Error retrieving patient information: " + ex.Message);
					}
				}
			}
			else
			{
				MessageBox.Show("Please enter a valid time in HH:mm format.");
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			string patientName = textBox2.Text;
			string complaints = textBox3.Text;
			string medicationName = textBox4.Text;
			string dosage = textBox5.Text;
			string frequency = textBox8.Text;
			string treatmentHistory = textBox6.Text;
			string recommendations = textBox7.Text;

			DateTime appointmentTime = GetAppointmentDateTime();
			int patientId = GetPatientIdByName(patientName, connectionString);
			int doctorId = GetDoctorIdByUsername("doctor", connectionString);
			int medicineId = GetMedicineIdByName(medicationName, connectionString);

			// Запис в MedicalRecords тепер завжди створює новий запис
			int recordId = InsertNewMedicalRecord(patientId, complaints, treatmentHistory, connectionString);

			int scheduleId; // Ініціалізація змінної
			int visitId = HandleVisitLog(patientId, doctorId, appointmentTime, medicineId, recordId, connectionString, out scheduleId);

			// Отримання нового ID для Medications
			int newMedicationId = GetNextMedicationId(connectionString);

			// Вставка запису в Medications
			InsertMedication(newMedicationId, medicineId, patientId, dosage, frequency, visitId, connectionString);

			MessageBox.Show("Дані записано успішно");
		}

		private DateTime GetAppointmentDateTime()
		{
			DateTime selectedDate = dateTimePicker6.Value.Date;
			TimeSpan selectedTime;
			if (!TimeSpan.TryParse(textBox9.Text, out selectedTime))
			{
				MessageBox.Show("Невірний формат часу.");
				return DateTime.MinValue; // Повертаємо якесь значення, яке вказує на помилку
			}
			return selectedDate.Add(selectedTime);
		}

		private int GetNextVisitId(SqlConnection conn)
		{
			string query = "SELECT ISNULL(MAX(VisitID), 0) + 1 FROM VisitLogs";
			using (SqlCommand cmd = new SqlCommand(query, conn))
			{
				return (int)cmd.ExecuteScalar();
			}
		}
		private int GetNextRecordId(SqlConnection conn)
		{
			string query = "SELECT ISNULL(MAX(RecordID), 0) + 1 FROM MedicalRecords";
			using (SqlCommand cmd = new SqlCommand(query, conn))
			{
				return (int)cmd.ExecuteScalar();
			}
		}

		private int InsertVisitLog(int patientId, int doctorId, int scheduleId, int medicineId, int recordId, SqlConnection conn)
		{
			int visitId = GetNextVisitId(conn); // Отримання наступного VisitID

			string insertQuery = @"
        INSERT INTO VisitLogs (VisitID, PatientID, StaffID, MedicineID, RecordID, ScheduleID)
        VALUES (@VisitID, @PatientID, @DoctorID, @MedicineID, @RecordID, @ScheduleID)";
			using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
			{
				insertCmd.Parameters.AddWithValue("@VisitID", visitId);
				insertCmd.Parameters.AddWithValue("@PatientID", patientId);
				insertCmd.Parameters.AddWithValue("@DoctorID", doctorId);
				insertCmd.Parameters.AddWithValue("@MedicineID", medicineId);
				insertCmd.Parameters.AddWithValue("@RecordID", recordId);
				insertCmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
				insertCmd.ExecuteNonQuery();
			}

			return visitId;
		}

		public int HandleVisitLog(int patientId, int doctorId, DateTime appointmentTime, int medicineId, int recordId, string connectionString, out int scheduleId)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();

				// Отримання ScheduleID
				string scheduleQuery = "SELECT ScheduleID FROM Schedules WHERE Reception = @AppointmentTime AND PatientID = @PatientID";
				SqlCommand scheduleCmd = new SqlCommand(scheduleQuery, conn);
				scheduleCmd.Parameters.AddWithValue("@AppointmentTime", appointmentTime);
				scheduleCmd.Parameters.AddWithValue("@PatientID", patientId);
				object scheduleResult = scheduleCmd.ExecuteScalar();

				if (scheduleResult == null)
				{
					MessageBox.Show("Не знайдено запису в розкладі для даного часу та пацієнта.");
					scheduleId = -1; // Встановлення помилкового значення
					return -1; // Повернення помилкового значення для visitId
				}

				scheduleId = Convert.ToInt32(scheduleResult);
				return InsertVisitLog(patientId, doctorId, scheduleId, medicineId, recordId, conn);
			}
		}

		public int GetPatientIdByName(string fullName, string connectionString)
		{
			string[] names = fullName.Split(' ');
			string firstName = names[0];
			string lastName = names[1];

			string query = "SELECT PatientID FROM Patients WHERE PatientName = @FirstName AND PatientLastname = @LastName";

			using (SqlConnection conn = new SqlConnection(connectionString))
			using (SqlCommand cmd = new SqlCommand(query, conn))
			{
				cmd.Parameters.AddWithValue("@FirstName", firstName);
				cmd.Parameters.AddWithValue("@LastName", lastName);

				conn.Open();
				return (int)cmd.ExecuteScalar();
			}
		}
		public int GetDoctorIdByUsername(string username, string connectionString)
		{
			string query = "SELECT StaffID FROM MedicalStaff JOIN Users ON MedicalStaff.UserID = Users.UserID WHERE Username = @Username";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@Username", username);
					var result = cmd.ExecuteScalar();
					return result != null ? (int)result : 0;
				}
			}
		}
		public int GetMedicineIdByName(string medicineName, string connectionString)
		{
			string query = "SELECT MedicineID FROM Medicines WHERE Name = @MedicineName";

			using (SqlConnection conn = new SqlConnection(connectionString))
			using (SqlCommand cmd = new SqlCommand(query, conn))
			{
				cmd.Parameters.AddWithValue("@MedicineName", medicineName);

				conn.Open();
				return (int)cmd.ExecuteScalar();
			}
		}
		private int InsertNewMedicalRecord(int patientId, string healthDetails, string treatmentHistory, string connectionString)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				int newRecordId = GetNextRecordId(conn);

				string insertQuery = @"
        INSERT INTO MedicalRecords (RecordID, PatientID, HealthDetails, TreatmentHistory)
        VALUES (@NewRecordID, @PatientID, @HealthDetails, @TreatmentHistory)";
				using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
				{
					insertCmd.Parameters.AddWithValue("@NewRecordID", newRecordId);
					insertCmd.Parameters.AddWithValue("@PatientID", patientId);
					insertCmd.Parameters.AddWithValue("@HealthDetails", healthDetails);
					insertCmd.Parameters.AddWithValue("@TreatmentHistory", treatmentHistory);
					insertCmd.ExecuteNonQuery();
				}
				return newRecordId;
			}
		}

		public void InsertMedication(int newMedicationId, int medicineId, int patientId, string dosage, string frequency, int visitId, string connectionString)
		{
			string query = @"
INSERT INTO Medications (MedicationID, MedicineID, PatientID, Dosage, Frequency, VisitID)
VALUES (@NewMedicationID, @MedicineID, @PatientID, @Dosage, @Frequency, @VisitID)";

			using (SqlConnection conn = new SqlConnection(connectionString))
			using (SqlCommand cmd = new SqlCommand(query, conn))
			{
				cmd.Parameters.AddWithValue("@NewMedicationID", newMedicationId);
				cmd.Parameters.AddWithValue("@MedicineID", medicineId);
				cmd.Parameters.AddWithValue("@PatientID", patientId);
				cmd.Parameters.AddWithValue("@Dosage", dosage);
				cmd.Parameters.AddWithValue("@Frequency", frequency);
				cmd.Parameters.AddWithValue("@VisitID", visitId);

				conn.Open();
				cmd.ExecuteNonQuery();
			}
		}

		public int GetNextMedicationId(string connectionString)
		{
			string query = "SELECT ISNULL(MAX(MedicationID), 0) + 1 FROM Medications";
			using (SqlConnection conn = new SqlConnection(connectionString))
			using (SqlCommand cmd = new SqlCommand(query, conn))
			{
				conn.Open();
				return (int)cmd.ExecuteScalar();
			}
		}

		private void button6_Click(object sender, EventArgs e)
		{
			// Отримати ім'я та прізвище з текстового поля
			string fullName = textBox10.Text;
			string[] nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			// Перевірка чи введені обидва ім'я та прізвище
			if (nameParts.Length < 2)
			{
				MessageBox.Show("Будь ласка, введіть ім'я та прізвище пацієнта.");
				return;
			}
			string firstName = nameParts[0];
			string lastName = nameParts[1];

			string painLogsQuery = @"
             SELECT PainLevel, LogDate
FROM PainLogs pl
JOIN Patients pat ON pl.PatientID = pat.PatientID
WHERE pat.PatientName LIKE @FirstName + '%' AND pat.PatientLastname LIKE @LastName + '%'
ORDER BY pl.LogDate DESC";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(painLogsQuery, conn);
				cmd.Parameters.AddWithValue("@FirstName", firstName);
				cmd.Parameters.AddWithValue("@LastName", lastName);

				DataTable painLogsTable = new DataTable();
				try
				{
					conn.Open();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(painLogsTable);
					}
					dataGridView5.DataSource = painLogsTable;

					// Adjust column headers as necessary
					dataGridView5.Columns["PainLevel"].HeaderText = "Рівень болю";
					dataGridView5.Columns["LogDate"].HeaderText = "Дата, час";

				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при пошуку пацієнта: " + ex.Message);
				}
			}

			// Формування SQL запиту з умовою WHERE, яка використовує ім'я та прізвище
			string healthMonitoringQuery = @"
    SELECT p.Preassure AS Preassure, sl.SugarLevel AS SugarLevel, pl.PulseRate AS Pulse 
    FROM HealthMonitoring hm
    JOIN Preassure p ON hm.PreassureID = p.PreassureID
    JOIN SugarLevel sl ON hm.SugarLevelID = sl.SugarLevelID
    JOIN Pulse pl ON hm.PulseID = pl.PulseID
    JOIN Patients pat ON hm.PatientID = pat.PatientID
    WHERE pat.PatientName LIKE @FirstName + '%' AND pat.PatientLastname LIKE @LastName + '%'
    ORDER BY p.ReadingDate DESC";

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(healthMonitoringQuery, conn);
				cmd.Parameters.AddWithValue("@FirstName", firstName);
				cmd.Parameters.AddWithValue("@LastName", lastName);

				DataTable healthMonitoringTable = new DataTable();
				try
				{
					conn.Open();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(healthMonitoringTable);
					}
					dataGridView4.DataSource = healthMonitoringTable;

					// Adjust column headers as necessary
					dataGridView4.Columns["Preassure"].HeaderText = "Тиск";
					dataGridView4.Columns["SugarLevel"].HeaderText = "Рівень цукру";
					dataGridView4.Columns["Pulse"].HeaderText = "Пульс";
				}
				catch (Exception ex)
				{
					MessageBox.Show("Помилка при пошуку пацієнта: " + ex.Message);
				}
			}
		}
	}
}



