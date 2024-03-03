using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Icao;

public class Program
{
    public static void SelectVillains(MySqlConnection con)
    {
        using var cmd = new MySqlCommand("SELECT v.Name, COUNT(m.Id) Minions FROM Minions m JOIN MinionsVillains mv ON m.Id = mv.MinionId JOIN Villains v ON mv.VillainId = v.Id GROUP BY v.Id; ", con);
        using MySqlDataReader rdr = cmd.ExecuteReader();

        while (rdr.Read())
        {
            Console.WriteLine($"{rdr["Name"]} - {rdr["Minions"]}");
        }
    }

    public static void GetVillainMinions(string cs, int villainId)
    {
        using var con = new MySqlConnection(cs);
        con.Open();

        try
        {
            using var cmd = new MySqlCommand("SELECT Name FROM Villains WHERE Id = @id; ", con);
            cmd.Parameters.AddWithValue("@id", villainId);
            cmd.Prepare();

            using MySqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                Console.WriteLine($"Villain: {rdr["Name"]}");
                GetMinions(cs, villainId);
            }
            else
            {
                Console.WriteLine($"No villain with ID {villainId} exists in the database.");
            }
        }
        finally
        {
            con.Close();
        }
    }

    public static void GetMinions(string cs, int villainId)
    {
        using var con = new MySqlConnection(cs);
        con.Open();

        try
        {
            using var cmd = new MySqlCommand("SELECT Name, Age FROM Minions WHERE Id IN (SELECT MinionId FROM MinionsVillains WHERE VillainId = @villainId ); ", con);
            cmd.Parameters.AddWithValue("@villainId", villainId);
            cmd.Prepare();
            using MySqlDataReader minionRdr = cmd.ExecuteReader();

            if (minionRdr.HasRows)
            {
                int i = 1;
                while (minionRdr.Read())
                {
                    Console.WriteLine($"{i++}: {minionRdr["Name"]} {minionRdr["Age"]}");
                }

            }
            else
            {
                Console.WriteLine("( No minions )");
            }
        }
        finally
        {
            con.Close();
        }
    }

    public static void AddMinion(string cs)
    {
        Console.Write("Minion: ");
        string[] minion = Console.ReadLine().Split(" ");

        Console.Write("Villain: ");
        string villain = Console.ReadLine();

        string town = minion[2];

        int townId = GetTown(cs, town);
        int villainId = GetVillain(cs, villain);

        using var con = new MySqlConnection(cs);
        con.Open();

        try
        {
            using var cmd = new MySqlCommand("INSERT INTO Minions(Name,Age,TownId) VALUES (@name,@age,@town);", con);
            cmd.Parameters.AddWithValue("@name", minion[0]);
            cmd.Parameters.AddWithValue("@age", int.Parse(minion[1]));
            cmd.Parameters.AddWithValue("@town", townId);

            cmd.Prepare();
            cmd.ExecuteNonQuery();
            int minionId = (int)cmd.LastInsertedId;

            AddMinionVillain(cs, minionId, villainId);
            Console.WriteLine($"Successfully added {minion[0]} to be minion of {villain}.");
        }
        finally
        {
            con.Close();
        }


    }

    public static int GetTown(string cs, string town)
    {
        using var con = new MySqlConnection(cs);
        con.Open();
        int townId = -1;

        try
        {
            using var cmd = new MySqlCommand("SELECT Id FROM Towns WHERE Name = @town ;", con);
            cmd.Parameters.AddWithValue("@town", town);
            cmd.Prepare();
            using MySqlDataReader townRdr = cmd.ExecuteReader();

            if (townRdr.HasRows)
            {
                townRdr.Read();
                townId = (int)townRdr["Id"];
            }
            else
            {
                townId = AddTown(cs, town);
            }

        }
        finally
        {
            con.Close();
        }

        return townId;
    }

    public static int AddTown(string cs, string town)
    {
        using var con = new MySqlConnection(cs);
        con.Open();
        int townId = -1;

        try
        {
            using var cmd = new MySqlCommand("INSERT INTO Towns(Name) VALUES (@town);", con);
            cmd.Parameters.AddWithValue("@town", town);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Town {town} was added to the database.");
            townId = (int)cmd.LastInsertedId;
        }
        finally
        {
            con.Close();
        }
        return townId;
    }

    public static int GetVillain(string cs, string villain)
    {
        using var con = new MySqlConnection(cs);
        con.Open();
        int villainId = -1;

        try
        {
            using var cmd = new MySqlCommand("SELECT Id FROM Villains WHERE Name = @villain ;", con);
            cmd.Parameters.AddWithValue("@villain", villain);
            cmd.Prepare();
            using MySqlDataReader villainRdr = cmd.ExecuteReader();

            if (villainRdr.HasRows)
            {
                villainRdr.Read();
                villainId = (int)villainRdr["Id"];
            }
            else
            {
                villainId = AddVillain(cs, villain);
            }

        }
        finally
        {
            con.Close();
        }

        return villainId;
    }
    public static int AddVillain(string cs, string villain)
    {
        using var con = new MySqlConnection(cs);
        con.Open();
        int villainId = -1;

        try
        {
            using var cmd = new MySqlCommand("INSERT INTO Villains(Name,EvilnessFactorId) VALUES (@villain,4);", con);
            cmd.Parameters.AddWithValue("@villain", villain);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Villain {villain} was added to the database.");
            villainId = (int)cmd.LastInsertedId;
        }
        finally
        {
            con.Close();
        }
        return villainId;
    }

    public static void AddMinionVillain(string cs, int minionId, int villainId)
    {
        using var con = new MySqlConnection(cs);
        con.Open();

        try
        {
            using var cmd = new MySqlCommand("INSERT INTO MinionsVillains VALUES (@minion,@villain);", con);
            cmd.Parameters.AddWithValue("@minion", minionId);
            cmd.Parameters.AddWithValue("@villain", villainId);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }
        finally
        {
            con.Close();
        }
    }

    public static void UpdateTownsToUpper(string cs, string countryName)
    {
        int countryId = GetCountry(cs, countryName);

        if (countryId == -1)
        {
            Console.WriteLine("No town names were affected.");
            return;
        }
        using var con = new MySqlConnection(cs);
        con.Open();

        try
        {
            using var cmd = new MySqlCommand("UPDATE Towns SET Name = UPPER(Name) WHERE CountryCode = @country;", con);
            cmd.Parameters.AddWithValue("@country", countryId);
            cmd.Prepare();
            int affectedTowns = cmd.ExecuteNonQuery();
            if (affectedTowns == 0)
            {
                Console.WriteLine("No town names were affected.");
            }
            else
            {
                Console.WriteLine($"{affectedTowns} town names were affected.");
                PrintTownNames(cs, countryId);
            }

        }
        finally
        {
            con.Close();
        }
    }

    public static int GetCountry(string cs, string country)
    {
        using var con = new MySqlConnection(cs);
        con.Open();
        int countryId = -1;

        try
        {
            using var cmd = new MySqlCommand("SELECT Id FROM Countries WHERE Name = @country ;", con);
            cmd.Parameters.AddWithValue("@country", country);
            cmd.Prepare();
            using MySqlDataReader countryRdr = cmd.ExecuteReader();

            if (countryRdr.HasRows)
            {
                countryRdr.Read();
                countryId = (int)countryRdr["Id"];
            }
        }
        finally
        {
            con.Close();
        }

        return countryId;
    }

    public static void PrintTownNames(string cs, int countryId)
    {
        using var con = new MySqlConnection(cs);
        con.Open();
        try
        {
            using var cmd = new MySqlCommand("SELECT Name FROM Towns WHERE CountryCode = @country ;", con);
            cmd.Parameters.AddWithValue("@country", countryId);
            cmd.Prepare();
            using MySqlDataReader townRdr = cmd.ExecuteReader();

            List<string> towns = [];
            while (townRdr.Read())
            {
                towns.Add((string)townRdr["Name"]);
            }
            Console.WriteLine($"[{String.Join(", ", towns)}]");

        }
        finally
        {
            con.Close();
        }
    }

    public static void DeleteVillain(string cs, int villainId)
    {

        int freedMinions = FreeMinions(cs, villainId);

        using var con = new MySqlConnection(cs);
        con.Open();
        try
        {
            string villainName = GetVillainById(cs, villainId);

            if (villainName.Equals("No such villain was found."))
            {
                Console.WriteLine(villainName);
            }
            else
            {
                using var cmd = new MySqlCommand("DELETE FROM Villains WHERE Id = @villain;", con);
                cmd.Parameters.AddWithValue("@villain", villainId);
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                Console.WriteLine($"{villainName} was deleted.");
                Console.WriteLine($"{freedMinions} minions were released.");
            }

        }
        finally
        {
            con.Close();
        }
    }

    public static int FreeMinions(string cs, int villainId)
    {
        using var con = new MySqlConnection(cs);
        con.Open();
        int freedMinions = 0;

        try
        {
            using var cmd = new MySqlCommand("DELETE FROM MinionsVillains WHERE VillainId = @villain;", con);
            cmd.Parameters.AddWithValue("@villain", villainId);
            cmd.Prepare();
            freedMinions = cmd.ExecuteNonQuery();
        }
        finally
        {
            con.Close();
        }

        return freedMinions;

    }

    public static string GetVillainById(string cs, int villainId)
    {
        using var con = new MySqlConnection(cs);
        con.Open();
        string villainName = "";

        try
        {
            using var cmd = new MySqlCommand("SELECT Name FROM Villains WHERE Id = @villain ;", con);
            cmd.Parameters.AddWithValue("@villain", villainId);
            cmd.Prepare();
            using MySqlDataReader villainRdr = cmd.ExecuteReader();

            if (villainRdr.HasRows)
            {
                villainRdr.Read();
                villainName = (string)villainRdr["Name"];
            }
            else
            {
                villainName = "No such villain was found.";
            }
        }
        finally
        {
            con.Close();
        }

        return villainName;
    }


    public static void Main()
    {
        string cs = @"server=localhost;userid=yordan;password=1234;database=MinionsDB";

        //SelectVillains(con);
        //GetVillainMinions(cs, 5);
        //GetMinions(con, 5);
        //Console.WriteLine(GetVillain(cs, "Tony"));

        //AddMinion(cs);
        //UpdateTownsToUpper(cs, "Poland");
        DeleteVillain(cs, 101);
    }
}