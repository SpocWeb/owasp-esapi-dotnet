namespace Owasp.Esapi.Swingset
{
	public class Account
	{
		public Account(int _id, string _name, double _amt)
		{
			Id = _id;
			Name = _name;
			Amt = _amt;
		}

		public int Id { get; set; }

		public string Name { get; set; }

		public double Amt { get; set; }
	}
}