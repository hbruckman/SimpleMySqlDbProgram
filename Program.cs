namespace SimpleMySqlDbProgram;

public class Program
{
	public static async Task Main(string[] args)
	{
		SimpleMySqlDbProgram sp = new SimpleMySqlDbProgram();

		if (sp.OpenDbConnection())
		{
			string user = "popo";
			string pass = "pupu";

			if (sp.CheckUsernameAndPassword(user, pass))
			{
				Console.WriteLine("### Login Successful! ###");
			}
			else
			{
				Console.WriteLine("### Login Unsuccessful! ###");
			}
		}

		sp.CloseDbConnection();
	}
}
