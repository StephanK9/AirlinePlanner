using Xunit;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AirlinePlanner.Objects;
using System;

namespace AirlinePlanner
{
  public class FlightTest : IDisposable
  {
    public FlightTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=AirlinePlanner_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      int result = Flight.GetAll().Count;

      Assert.Equal(0, result);
    }
    [Fact]
    public void Test_Equal_ReturnsTrueIfStatussAreTheSame()
    {
      Flight firstFlight = new Flight("On Time");
      Flight secondFlight = new Flight("On Time");

      Assert.Equal(firstFlight, secondFlight);
    }
    [Fact]
    public void Test_Save_SavesToDatabase()
    {
      Flight testFlight = new Flight("On Time");

      testFlight.Save();
      List<Flight> result = Flight.GetAll();
      List<Flight> testList = new List<Flight>{testFlight};

      Assert.Equal(testList, result);
    }

    public void Dispose()
    {
      Flight.DeleteAll();
    }

    [Fact]
    public void Test_Save_AssignsIdToObject()
    {
      Flight testFlight = new Flight("On Time");

      testFlight.Save();
      Flight savedFlight = Flight.GetAll()[0];

      int result = savedFlight.GetId();
      int testId = testFlight.GetId();

      Assert.Equal(testId, result);
    }
    [Fact]
    public void Test_Find_FindsFlightInDatabase()
    {
      Flight testFlight = new Flight("On Time");
      testFlight.Save();

      Flight foundFlight = Flight.Find(testFlight.GetId());

      Assert.Equal(testFlight, foundFlight);
    }

    [Fact]
    public void Test_AddCity_AddsCityToFlight()
    {
      //Arrange
      Flight testFlight = new Flight("On Time");
      testFlight.Save();

      City testCity = new City("Houston");
      testCity.Save();

      //Act
      testFlight.AddCity(testCity);

      List<City> result = testFlight.GetCities();
      List<City> testList = new List<City>{testCity};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetCities_ReturnsAllFlightCities()
    {
      //Arrange
      Flight testFlight = new Flight("On Time");
      testFlight.Save();

      City testCity1 = new City("Houston");
      testCity1.Save();

      City testCity2 = new City("Houston");
      testCity2.Save();

      //Act
      testFlight.AddCity(testCity1);
      List<City> result = testFlight.GetCities();
      List<City> testList = new List<City> {testCity1};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Delete_DeletesFlightAssociationsFromDatabase()
    {
      //Arrange
      City testCity = new City("Houston");
      testCity.Save();

      string testStatus = "On Time";
      Flight testFlight = new Flight(testStatus);
      testFlight.Save();

      //Act
      testFlight.AddCity(testCity);
      testFlight.Delete();

      List<Flight> resultCityFlights = testCity.GetFlights();
      List<Flight> testCityFlights = new List<Flight> {};

      //Assert
      Assert.Equal(testCityFlights, resultCityFlights);
    }
  }
}
