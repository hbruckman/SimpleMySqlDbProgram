namespace SimpleMySqlDbProgram;

using MySql.Data.MySqlClient;
using System.Data.Common;

public class SimpleMySqlDbProgram
{
	private DbConnection dbc;

	public SimpleMySqlDbProgram()
	{
		Console.WriteLine("### Initializing DB Connection ###");
		dbc = new MySqlConnection("server=localhost;port=3306;uid=root;pwd=12345;");
	}

	public bool OpenDbConnection()
	{
		Console.WriteLine("### Opening DB Connection ###");

		string dbName = "mydb";

		dbc.Open();

		if (DatabaseExists(dbName))
		{
			DestroyDatabase(dbName);

			return false;

			UseDatabase(dbName);

			return true;
		}
		else
		{
			CreateDatabase(dbName);
			UseDatabase(dbName);
			CreateTables();
			PopulateTables();

			return true;
		}
	}

	public void CloseDbConnection()
	{
		Console.WriteLine("### Closing DB Connection ###");
		dbc.Close();
	}

	private bool DatabaseExists(string dbName)
	{
		Console.WriteLine($"### Checking if DB {dbName} Exists ###");

		using DbCommand cmd = dbc.CreateCommand();

		cmd.CommandText = string.Format(@"
		SELECT SCHEMA_NAME
		FROM INFORMATION_SCHEMA.SCHEMATA
		WHERE SCHEMA_NAME = '{0}'
		", dbName);

		using DbDataReader dr = cmd.ExecuteReader();

		bool exists = dr.Read();

		Console.WriteLine("### It does {0}exists! ###", exists ? "" : "not ");

		return exists;
	}

	private void CreateDatabase(string dbName)
	{
		Console.WriteLine($"### Creating DB {dbName} ###");

		DbCommand cmd = dbc.CreateCommand();

		cmd.CommandText = string.Format(@"
		CREATE DATABASE {0}
		", dbName);

		cmd.ExecuteNonQuery();
	}

	private void DestroyDatabase(string dbName)
	{
		Console.WriteLine($"### Destroying DB {dbName} ###");

		DbCommand cmd = dbc.CreateCommand();

		cmd.CommandText = string.Format(@"
		DROP DATABASE {0};
		", dbName);

		cmd.ExecuteNonQuery();
	}

	private void UseDatabase(string dbName)
	{
		Console.WriteLine($"### Using DB {dbName} ###");

		DbCommand cmd = dbc.CreateCommand();

		cmd.CommandText = string.Format(@"
		USE {0}
		", dbName);

		cmd.ExecuteNonQuery();
	}

	private void CreateTables()
	{
		Console.WriteLine("### Creating Tables ###");
		CreateAccountsTable();
	}

	private void CreateAccountsTable()
	{
		Console.WriteLine("### Creating Accounts Table ###");

		DbCommand cmd = dbc.CreateCommand();

		cmd.CommandText = string.Format(@"
		CREATE TABLE Accounts
		(
			id int unsigned NOT NULL AUTO_INCREMENT,
			username varchar(32) NOT NULL,
			password binary(16) NOT NULL,

			PRIMARY KEY (`id`),
			UNIQUE KEY `account_id_unique` (`id`),
			UNIQUE KEY `account_username_unique` (`username`)
		)
		");

		cmd.ExecuteNonQuery();
	}

	private void PopulateTables()
	{
		Console.WriteLine("### Populating Tables ###");
		PopulateAccountsTable();
	}

	private void PopulateAccountsTable()
	{
		Console.WriteLine("### Populating Accounts Table ###");

		DbCommand cmd = dbc.CreateCommand();

		cmd.CommandText = string.Format(@"
		INSERT INTO Accounts (username, password)
		VALUES (@user, UNHEX(MD5(@pass)))
		");

		string user = "popo";

		DbParameter username = cmd.CreateParameter();
		username.ParameterName = "@user";
		username.Value = user;

		cmd.Parameters.Add(username);

		string pass = "pepe";

		DbParameter password = cmd.CreateParameter();
		password.ParameterName = "@pass";
		password.Value = pass;

		cmd.Parameters.Add(password);

		int rowCount = cmd.ExecuteNonQuery();

		Console.WriteLine($"### Inserted Rows: {rowCount} ###");
	}

	public bool CheckUsernameAndPassword(string user, string pass)
	{
		Console.WriteLine($"### Checking Username ({user}) and Password ({pass}) ###");

		DbCommand cmd = dbc.CreateCommand();

		cmd.CommandText = string.Format(@"
		SELECT COUNT(*)
		FROM Accounts
		WHERE username=@user AND password=UNHEX(MD5(@pass))
		");

		DbParameter username = cmd.CreateParameter();
		username.ParameterName = "@user";
		username.Value = user;

		cmd.Parameters.Add(username);

		DbParameter password = cmd.CreateParameter();
		password.ParameterName = "@pass";
		password.Value = pass;

		cmd.Parameters.Add(password);

		long count = (long) cmd.ExecuteScalar();

		return (count > 0);
 	}
}
