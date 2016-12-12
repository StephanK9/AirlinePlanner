using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System;

namespace AirlinePlanner.Objects
{
  public class Flight
  {
    private int _id;
    private string _status;

    public Flight(string Status, int Id = 0)
    {
      _id = Id;
      _status = Status;
    }

    public override bool Equals(System.Object otherFlight)
    {
      if (!(otherFlight is Flight))
      {
        return false;
      }
      else
      {
        Flight newFlight = (Flight) otherFlight;
        bool idEquality = (this.GetId() == newFlight.GetId());
        bool statusEquality = (this.GetStatus() == newFlight.GetStatus());
        return (idEquality && statusEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetStatus().GetHashCode();
    }

    public int GetId()
    {
      return _id;
    }

    public string GetStatus()
    {
      return _status;
    }
    public void SetStatus(string newStatus)
    {
      _status = newStatus;
    }

    public static List<Flight> GetAll()
    {
      List<Flight> AllFlights = new List<Flight>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM flights;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int flightId = rdr.GetInt32(0);
        string flightStatus = rdr.GetString(1);
        Flight newFlight = new Flight(flightStatus, flightId);
        AllFlights.Add(newFlight);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return AllFlights;
    }
    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO flights (status) OUTPUT INSERTED.id VALUES (@FlightStatus);", conn);

      SqlParameter statusParameter = new SqlParameter();
      statusParameter.ParameterName = "@FlightStatus";
      statusParameter.Value = this.GetStatus();

      cmd.Parameters.Add(statusParameter);

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM flights WHERE id = @FlightId; DELETE FROM cities_flights WHERE flight_id = @FlightId;", conn);
      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();

      cmd.Parameters.Add(flightIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM flights;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }
    public static Flight Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM flights WHERE id = @FlightId;", conn);
      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = id.ToString();
      cmd.Parameters.Add(flightIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundFlightId = 0;
      string foundFlightStatus = null;


      while(rdr.Read())
      {
        foundFlightId = rdr.GetInt32(0);
        foundFlightStatus = rdr.GetString(1);
      }
      Flight foundFlight = new Flight (foundFlightStatus, foundFlightId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundFlight;
    }

    public void AddCity(City newCity)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO cities_flights (city_id, flight_id) VALUES (@CityId, @FlightId);", conn);

      SqlParameter cityIdParameter = new SqlParameter();
      cityIdParameter.ParameterName = "@CityId";
      cityIdParameter.Value = newCity.GetId();
      cmd.Parameters.Add(cityIdParameter);

      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();
      cmd.Parameters.Add(flightIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<City> GetCities()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT city_id FROM cities_flights WHERE flight_id = @FlightId;", conn);

      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();
      cmd.Parameters.Add(flightIdParameter);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<int> cityIds = new List<int> {};

      while (rdr.Read())
      {
        int cityId = rdr.GetInt32(0);
        cityIds.Add(cityId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<City> cities = new List<City> {};

      foreach (int cityId in cityIds)
      {
        SqlCommand cityQuery = new SqlCommand("SELECT * FROM cities WHERE id = @CityId;", conn);

        SqlParameter cityIdParameter = new SqlParameter();
        cityIdParameter.ParameterName = "@CityId";
        cityIdParameter.Value = cityId;
        cityQuery.Parameters.Add(cityIdParameter);

        SqlDataReader queryReader = cityQuery.ExecuteReader();
        while (queryReader.Read())
        {
          int thisCityId = queryReader.GetInt32(0);
          string cityName = queryReader.GetString(1);
          City foundCity = new City(cityName, thisCityId);
          cities.Add(foundCity);
        }
        if (queryReader != null)
        {
          queryReader.Close();
        }
      }
      if (conn != null)
      {
        conn.Close();
      }
      return cities;
    }
  }
}
