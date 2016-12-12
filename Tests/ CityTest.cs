using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using AirlinePlanner.Objects;

namespace AirlinePlanner
{
  public class CityTest : IDisposable
  {
    public CityTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=AirlinePlanner_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_CitiesEmptyAtFirst()
    {
      //Arrange, Act
      int result = City.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueForSameName()
    {
      //Arrange, Act
      City firstCity = new City("Houston");
      City secondCity = new City("Houston");

      //Assert
      Assert.Equal(firstCity, secondCity);
    }

    [Fact]
    public void Test_Save_SavesCityToDatabase()
    {
      //Arrange
      City testCity = new City("Houston");
      testCity.Save();

      //Act
      List<City> result = City.GetAll();
      List<City> testList = new List<City>{testCity};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToCityObject()
    {
      //Arrange
      City testCity = new City("Houston");
      testCity.Save();

      //Act
      City savedCity = City.GetAll()[0];

      int result = savedCity.GetId();
      int testId = testCity.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_Find_FindsCityInDatabase()
    {
      //Arrange
      City testCity = new City("Houston");
      testCity.Save();

      //Act
      City foundCity = City.Find(testCity.GetId());

      //Assert
      Assert.Equal(testCity, foundCity);
    }

    [Fact]
    public void Test_Update_UpdatesCityInDatabase()
    {
      //Arrange
      string name = "Houston";
      City testCity = new City(name);
      testCity.Save();
      string newName = "Atlanta";

      //Act
      testCity.Update(newName);
      string result = testCity.GetName();

      //Assert
      Assert.Equal(newName, result);
    }

    [Fact]
    public void Test_Delete_DeletesCityFromDatabase()
    {
      //Arrange
      string name1 = "Houston";
      City testCity1 = new City(name1);
      testCity1.Save();

      string name2 = "Atlanta";
      City testCity2 = new City(name2);
      testCity2.Save();

      //Act
      testCity1.Delete();
      List<City> resultCities = City.GetAll();
      List<City> testCityList = new List<City> {testCity2};

      //Assert
      Assert.Equal(testCityList, resultCities);
    }

    [Fact]
    public void Test_AddFlight_AddsFlightToCity()
    {
      //Arrange
      City testCity = new City("Houston");
      testCity.Save();

      Flight testFlight = new Flight("On Time");
      testFlight.Save();

      Flight testFlight2 = new Flight("Delayed");
      testFlight2.Save();

      //Act
      testCity.AddFlight(testFlight);
      testCity.AddFlight(testFlight2);

      List<Flight> result = testCity.GetFlights();
      List<Flight> testList = new List<Flight>{testFlight, testFlight2};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetFlights_ReturnsAllCityFlights()
    {
      //Arrange
      City testCity = new City("Houston");
      testCity.Save();

      Flight testFlight1 = new Flight("On Time");
      testFlight1.Save();

      Flight testFlight2 = new Flight("On Time");
      testFlight2.Save();

      //Act
      testCity.AddFlight(testFlight1);
      List<Flight> savedFlights = testCity.GetFlights();
      List<Flight> testList = new List<Flight> {testFlight1};

      //Assert
      Assert.Equal(testList, savedFlights);
    }

    [Fact]
    public void Test_Delete_DeletesCityAssociationsFromDatabase()
    {
      //Arrange
      Flight testFlight = new Flight("On Time");
      testFlight.Save();

      string testName = "Houston";
      City testCity = new City(testName);
      testCity.Save();

      //Act
      testCity.AddFlight(testFlight);
      testCity.Delete();

      List<City> resultFlightCities = testFlight.GetCities();
      List<City> testFlightCities = new List<City> {};

      //Assert
      Assert.Equal(testFlightCities, resultFlightCities);
    }

    public void Dispose()
    {
      Flight.DeleteAll();
      City.DeleteAll();
    }
  }
}
